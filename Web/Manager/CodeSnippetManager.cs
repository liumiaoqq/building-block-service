using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Manager
{
    public class CodeSnippetManager
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        public CodeSnippetManager(ISqlSugarClient sqlSugarClient)
        {
            _sqlSugarClient = sqlSugarClient;
        }

        /// <summary>
        /// 获取代码片段树形结构
        /// </summary>
        public async Task<CodeSnippetTreeOutput> GetTreeAsync(Guid codeManagementId)
        {
            var management = await _sqlSugarClient.Queryable<CodeManagement>()
                .Where(x => x.Id == codeManagementId && !x.IsDeleted)
                .FirstAsync();

            if (management == null) return null;

            var allSnippets = await _sqlSugarClient.Queryable<CodeSnippet>()
                .Where(x => x.CodeManagementId == codeManagementId && !x.IsDeleted)
                .OrderBy(x => x.SortOrder)
                .Select(x => new CodeSnippetDto
                {
                    Id = x.Id,
                    CodeManagementId = x.CodeManagementId,
                    FileName = x.FileName,
                    FilePath = x.FilePath,
                    ParentId = x.ParentId,
                    IsFolder = x.IsFolder,
                    FileSize = x.FileSize,
                    FileExtension = x.FileExtension,
                    FileType = x.FileType,
                    Level = x.Level,
                    SortOrder = x.SortOrder,

                })
                .ToListAsync();

            var treeData = BuildTree(allSnippets, null);

            return new CodeSnippetTreeOutput
            {
                CodeManagementId = codeManagementId,
                ProjectName = management.ProjectName,
                TreeData = treeData
            };
        }

        /// <summary>
        /// 构建树形结构
        /// </summary>
        private List<CodeSnippetDto> BuildTree(List<CodeSnippetDto> allSnippets, Guid? parentId)
        {
            return allSnippets
                .Where(x => x.ParentId == parentId)
                .Select(x =>
                {
                    x.Children = BuildTree(allSnippets, x.Id);
                    return x;
                })
                .ToList();
        }

        /// <summary>
        /// 根据ID获取代码片段
        /// </summary>
        public async Task<CodeSnippetDto> GetByIdAsync(Guid id)
        {
            return await _sqlSugarClient.Queryable<CodeSnippet>()
                .Where(x => x.Id == id && !x.IsDeleted)
                .Select<CodeSnippetDto>()
                .FirstAsync();
        }

        /// <summary>
        /// 根据ID获取代码片段内容（只返回文件内容）
        /// </summary>
        public async Task<string> GetFileContentAsync(Guid id)
        {
            var snippet = await _sqlSugarClient.Queryable<CodeSnippet>()
                .Where(x => x.Id == id && !x.IsDeleted && !x.IsFolder)
                .Select(x => new { x.FileContent })
                .FirstAsync();

            return snippet?.FileContent ?? string.Empty;
        }

        /// <summary>
        /// 批量创建代码片段
        /// </summary>
        public async Task<int> BatchCreateAsync(BatchCreateCodeSnippetsInput input)
        {
            var entities = new List<CodeSnippet>();
            var pathToIdMap = new Dictionary<string, Guid>(); // 路径到ID的映射
            var sortOrder = 0;

            // 收集所有需要创建的路径（包括文件夹）
            var allPaths = new HashSet<string>();
            foreach (var file in input.Files)
            {
                var pathParts = file.FilePath.Split('/');
                // 添加所有层级的路径（包括中间文件夹）
                for (int i = 1; i < pathParts.Length; i++)
                {
                    var partialPath = string.Join("/", pathParts.Take(i));
                    allPaths.Add(partialPath);
                }
                // 添加文件本身的路径
                allPaths.Add(file.FilePath);
            }

            // 按路径长度排序，确保先创建父文件夹
            var sortedPaths = allPaths.OrderBy(p => p.Split('/').Length).ThenBy(p => p).ToList();

            // 创建所有节点（文件夹和文件）
            foreach (var path in sortedPaths)
            {
                var pathParts = path.Split('/');
                var fileName = pathParts[pathParts.Length - 1];
                var parentPath = pathParts.Length > 1 ? string.Join("/", pathParts.Take(pathParts.Length - 1)) : null;
                var level = pathParts.Length - 1;

                // 判断是文件还是文件夹
                var fileInfo = input.Files.FirstOrDefault(f => f.FilePath == path);
                var isFolder = fileInfo == null;

                // 获取父节点ID
                Guid? parentId = null;
                if (!string.IsNullOrEmpty(parentPath) && pathToIdMap.ContainsKey(parentPath))
                {
                    parentId = pathToIdMap[parentPath];
                }

                // 创建节点
                var nodeId = Guid.NewGuid();
                var entity = new CodeSnippet
                {
                    Id = nodeId,
                    CodeManagementId = input.CodeManagementId,
                    FileName = fileName,
                    FilePath = path,
                    ParentId = parentId,
                    IsFolder = isFolder,
                    FileContent = isFolder ? null : (fileInfo?.FileContent ?? ""),
                    FileSize = isFolder ? 0 : (fileInfo?.FileSize ?? 0),
                    FileExtension = isFolder ? null : (fileInfo?.FileExtension ?? ""),
                    FileType = isFolder ? "Folder" : (fileInfo?.FileType ?? "Other"),
                    Level = level,
                    SortOrder = sortOrder++
                };

                entities.Add(entity);
                pathToIdMap[path] = nodeId;
            }

            // 批量插入数据库
            await _sqlSugarClient.Insertable(entities).ExecuteCommandAsync();

            // 更新代码管理的统计信息
            var totalFileCount = entities.Count(x => !x.IsFolder);
            var totalSize = entities.Where(x => !x.IsFolder).Sum(x => x.FileSize);

            await _sqlSugarClient.Updateable<CodeManagement>()
                .SetColumns(x => x.TotalFileCount == totalFileCount)
                .SetColumns(x => x.TotalSize == totalSize)
                .Where(x => x.Id == input.CodeManagementId && !x.IsDeleted)
                .ExecuteCommandAsync();

            return entities.Count;
        }

        /// <summary>
        /// 更新代码片段
        /// </summary>
        public async Task UpdateAsync(UpdateCodeSnippetInput input)
        {
            var entity = await _sqlSugarClient.Queryable<CodeSnippet>()
                .Where(x => x.Id == input.Id && !x.IsDeleted)
                .FirstAsync();

            if (entity == null)
            {
                throw new Exception("代码片段不存在");
            }

            entity.FileName = input.FileName;
            entity.FileContent = input.FileContent;

            await _sqlSugarClient.Updateable(entity).ExecuteCommandAsync();
        }

        /// <summary>
        /// 删除代码片段
        /// </summary>
        public async Task DeleteAsync(Guid id)
        {
            var entity = await _sqlSugarClient.Queryable<CodeSnippet>()
                .Where(x => x.Id == id && !x.IsDeleted)
                .FirstAsync();

            if (entity == null)
            {
                throw new Exception("代码片段不存在");
            }

            // 如果是文件夹，递归删除所有子节点
            if (entity.IsFolder)
            {
                await DeleteChildrenAsync(id);
            }

            await _sqlSugarClient.Updateable<CodeSnippet>()
                .SetColumns(x => x.IsDeleted == true)
                .Where(x => x.Id == id && !x.IsDeleted)
                .ExecuteCommandAsync();
        }

        /// <summary>
        /// 递归删除子节点
        /// </summary>
        private async Task DeleteChildrenAsync(Guid parentId)
        {
            var children = await _sqlSugarClient.Queryable<CodeSnippet>()
                .Where(x => x.ParentId == parentId && !x.IsDeleted)
                .ToListAsync();

            foreach (var child in children)
            {
                if (child.IsFolder)
                {
                    await DeleteChildrenAsync(child.Id);
                }
            }

            if (children.Count > 0)
            {
                await _sqlSugarClient.Updateable<CodeSnippet>()
                    .SetColumns(x => x.IsDeleted == true)
                    .Where(x => children.Select(c => c.Id).Contains(x.Id))
                    .ExecuteCommandAsync();
            }
        }

        /// <summary>
        /// 复制到新项目
        /// </summary>
        public async Task<Guid> CopyToNewProjectAsync(CopyToNewProjectInput input)
        {
            // 1. 创建新的代码管理
            var newCodeManagement = new CodeManagement
            {
                ProjectName = input.ProjectName,
                Description = input.Description,
                TotalFileCount = 0,
                TotalSize = 0,
                Remark = input.Remark
            };

            await _sqlSugarClient.Insertable(newCodeManagement).ExecuteCommandAsync();

            // 2. 一次性查询原项目的所有代码片段（减少数据库查询）
            var allSourceSnippets = await _sqlSugarClient.Queryable<CodeSnippet>()
                .Where(x => x.CodeManagementId == input.SourceCodeManagementId && !x.IsDeleted)
                .OrderBy(x => x.Level)
                .ToListAsync();

            // 3. 在内存中构建ID到对象的映射，方便快速查找
            var snippetDict = allSourceSnippets.ToDictionary(x => x.Id, x => x);

            // 4. 收集所有需要复制的节点ID（包括父文件夹）
            var allSnippetIds = new HashSet<Guid>(input.SelectedFileIds);
            foreach (var fileId in input.SelectedFileIds)
            {
                if (snippetDict.TryGetValue(fileId, out var snippet))
                {
                    // 在内存中递归收集父文件夹
                    CollectParentFoldersInMemory(snippet.ParentId, allSnippetIds, snippetDict);
                }
            }

            // 5. 从所有代码片段中筛选出需要复制的节点
            var allSnippets = allSourceSnippets
                .Where(x => allSnippetIds.Contains(x.Id))
                .OrderBy(x => x.Level)
                .ToList();

            // 6. 复制节点（保持层级关系）
            var idMapping = new Dictionary<Guid, Guid>(); // 旧ID -> 新ID 映射
            var newSnippets = new List<CodeSnippet>();
            var sortOrder = 0;

            // 先创建ID映射
            foreach (var snippet in allSnippets)
            {
                var newId = Guid.NewGuid();
                idMapping[snippet.Id] = newId;
            }

            // 构建新的代码片段列表
            foreach (var snippet in allSnippets)
            {
                var newSnippet = new CodeSnippet
                {
                    Id = idMapping[snippet.Id],
                    CodeManagementId = newCodeManagement.Id,
                    FileName = snippet.FileName,
                    FilePath = snippet.FilePath,
                    ParentId = snippet.ParentId.HasValue && idMapping.ContainsKey(snippet.ParentId.Value)
                        ? idMapping[snippet.ParentId.Value]
                        : null,
                    IsFolder = snippet.IsFolder,
                    FileContent = snippet.FileContent,
                    FileSize = snippet.FileSize,
                    FileExtension = snippet.FileExtension,
                    FileType = snippet.FileType,
                    Level = snippet.Level,
                    SortOrder = sortOrder++
                };

                newSnippets.Add(newSnippet);
            }

            // 批量插入
            if (newSnippets.Count > 0)
            {
                await _sqlSugarClient.Insertable(newSnippets).ExecuteCommandAsync();
            }

            // 7. 更新统计信息
            var totalFileCount = allSnippets.Count(x => !x.IsFolder);
            var totalSize = allSnippets.Where(x => !x.IsFolder).Sum(x => x.FileSize);

            await _sqlSugarClient.Updateable<CodeManagement>()
                .SetColumns(x => x.TotalFileCount == totalFileCount)
                .SetColumns(x => x.TotalSize == totalSize)
                .Where(x => x.Id == newCodeManagement.Id && !x.IsDeleted)
                .ExecuteCommandAsync();

            return newCodeManagement.Id;
        }

        /// <summary>
        /// 在内存中递归收集父文件夹ID（优化版：避免多次数据库查询）
        /// </summary>
        private void CollectParentFoldersInMemory(Guid? parentId, HashSet<Guid> collectedIds, Dictionary<Guid, CodeSnippet> snippetDict)
        {
            if (!parentId.HasValue || collectedIds.Contains(parentId.Value))
            {
                return;
            }

            collectedIds.Add(parentId.Value);

            if (snippetDict.TryGetValue(parentId.Value, out var parent) && parent.ParentId.HasValue)
            {
                CollectParentFoldersInMemory(parent.ParentId, collectedIds, snippetDict);
            }
        }
    }
}

