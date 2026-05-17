using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Web.HttpClient;
using Web.HttpClientApi.DeepSeek.Service;

namespace Web.Manager
{
    public class CodeManagementManager
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        private readonly ICurrentUser _currentUser;

        private readonly IDeepSeekService _deepSeekService;

        private readonly CodeSnippetManager _codeSnippetManager;

        public CodeManagementManager(
            ISqlSugarClient sqlSugarClient,
            ICurrentUser currentUser,
            IDeepSeekService deepSeekService,
            CodeSnippetManager codeSnippetManager)
        {
            _sqlSugarClient = sqlSugarClient;
            _currentUser = currentUser;
            _deepSeekService = deepSeekService;
            _codeSnippetManager = codeSnippetManager;
        }

        /// <summary>
        /// 获取代码管理分页列表
        /// </summary>
        public async Task<PagedReuslt<CodeManagementDto>> ListAsync(GetCodeManagementPagedInput input)
        {
            var userId = _currentUser.GetUserId();
            var result = await _sqlSugarClient.Queryable<CodeManagement>()

                .Where(x => x.CreatorId == userId)
                .WhereIF(input.ProjectName.IsNotNullOrNotWhiteSpace(), x => x.ProjectName.Contains(input.ProjectName))
                .OrderByDescending(x => x.Id)
                .Select<CodeManagementDto>()
                .ToPageListAsync(input.Page, input.Size);

            return new PagedReuslt<CodeManagementDto>(result, result.Count);
        }

        /// <summary>
        /// 根据ID获取代码管理
        /// </summary>
        public async Task<CodeManagementDto> GetByIdAsync(Guid id)
        {
            var userId = _currentUser.GetUserId();
            return await _sqlSugarClient.Queryable<CodeManagement>()
                .Where(x => x.Id == id)
                .Where(x => x.CreatorId == userId)
                .Select<CodeManagementDto>()
                .FirstAsync();
        }

        /// <summary>
        /// 创建代码管理
        /// </summary>
        public async Task<Guid> CreateAsync(CreateCodeManagementInput input)
        {
            if (string.IsNullOrWhiteSpace(input.ProjectName))
            {
                throw new Exception("项目名称不能为空");
            }
            //查询用户名下有多少个项目了
            var projectCount = await _sqlSugarClient.Queryable<CodeManagement>()
                .Where(x => x.CreatorId == _currentUser.GetUserId())
                .CountAsync();
            if (projectCount >= 100)
            {
                throw new Exception("您名下已创建100个项目，无法继续创建");
            }

            var entity = new CodeManagement
            {
                ProjectName = input.ProjectName,
                Description = input.Description,
                TotalFileCount = 0,
                TotalSize = 0,
                Remark = input.Remark
            };

            await _sqlSugarClient.Insertable(entity).ExecuteCommandAsync();
            return entity.Id;
        }

        /// <summary>
        /// 更新代码管理
        /// </summary>
        public async Task UpdateAsync(UpdateCodeManagementInput input)
        {
            if (string.IsNullOrWhiteSpace(input.ProjectName))
            {
                throw new Exception("项目名称不能为空");
            }

            var entity = await _sqlSugarClient.Queryable<CodeManagement>()
                .Where(x => x.Id == input.Id)
                .FirstAsync();

            if (entity == null)
            {
                throw new Exception("代码管理不存在");
            }

            entity.ProjectName = input.ProjectName;
            entity.Description = input.Description;
            entity.Remark = input.Remark;

            await _sqlSugarClient.Updateable(entity).ExecuteCommandAsync();
        }

        /// <summary>
        /// 更新项目信息（仅更新项目名称、描述和备注）
        /// </summary>
        public async Task UpdateProjectInfoAsync(UpdateProjectInfoInput input)
        {
            if (string.IsNullOrWhiteSpace(input.ProjectName))
            {
                throw new Exception("项目名称不能为空");
            }

            var entity = await _sqlSugarClient.Queryable<CodeManagement>()
                .Where(x => x.Id == input.Id)
                .FirstAsync();

            if (entity == null)
            {
                throw new Exception("项目不存在");
            }

            // 只更新项目名称、描述和备注，不更新其他字段
            entity.ProjectName = input.ProjectName.Trim();
            entity.Description = string.IsNullOrWhiteSpace(input.Description) ? null : input.Description.Trim();
            entity.Remark = string.IsNullOrWhiteSpace(input.Remark) ? null : input.Remark.Trim();

            await _sqlSugarClient.Updateable(entity)
                .UpdateColumns(x => new { x.ProjectName, x.Description, x.Remark })
                .ExecuteCommandAsync();
        }

        /// <summary>
        /// 删除代码管理（同时删除所有代码片段）
        /// </summary>
        public async Task DeleteAsync(Guid id)
        {
            var userId = _currentUser.GetUserId();
            // 先删除所有代码片段
            await _sqlSugarClient.Updateable<CodeSnippet>()
                .SetColumns(x => x.IsDeleted == true)
                .Where(x => x.CodeManagementId == id && x.CreatorId == userId)
                .ExecuteCommandAsync();

            // 再删除代码管理
            await _sqlSugarClient.Updateable<CodeManagement>()
                .SetColumns(x => x.IsDeleted == true)
                .Where(x => x.Id == id)
                .ExecuteCommandAsync();
        }

        /// <summary>
        /// 批量删除代码管理（同时删除所有相关代码片段）
        /// </summary>
        public async Task BatchDeleteAsync(List<Guid> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                throw new Exception("请选择要删除的项目");
            }

            var userId = _currentUser.GetUserId();

            // 验证所有代码管理是否存在且属于当前用户
            var codeManagements = await _sqlSugarClient.Queryable<CodeManagement>()
                .Where(x => ids.Contains(x.Id) && x.CreatorId == userId)
                .ToListAsync();

            if (codeManagements.Count != ids.Count)
            {
                throw new Exception("部分项目不存在或无权限删除");
            }

            // 批量软删除相关的代码片段
            await _sqlSugarClient.Updateable<CodeSnippet>()
                .SetColumns(x => x.IsDeleted == true)
                .Where(x => ids.Contains(x.CodeManagementId) && x.CreatorId == userId)
                .ExecuteCommandAsync();

            // 批量软删除代码管理
            await _sqlSugarClient.Updateable<CodeManagement>()
                .SetColumns(x => x.IsDeleted == true)
                .Where(x => ids.Contains(x.Id) && x.CreatorId == userId)
                .ExecuteCommandAsync();
        }

        /// <summary>
        /// 更新统计信息
        /// </summary>
        public async Task UpdateStatisticsAsync(Guid id, int totalFileCount, long totalSize)
        {
            await _sqlSugarClient.Updateable<CodeManagement>()
                .SetColumns(x => x.TotalFileCount == totalFileCount)
                .SetColumns(x => x.TotalSize == totalSize)
                .Where(x => x.Id == id)
                .ExecuteCommandAsync();
        }

        /// <summary>
        /// 获取代码管理统计信息
        /// </summary>
        public async Task<CodeManagementStatisticsOutput> GetStatisticsAsync()
        {
            var userId = _currentUser.GetUserId();
            var statistics = await _sqlSugarClient.Queryable<CodeManagement>()

                .Where(x => x.CreatorId == userId)
                .Select(x => new
                {
                    x.TotalFileCount,
                    x.TotalSize
                })
                .ToListAsync();

            return new CodeManagementStatisticsOutput
            {
                TotalProjects = statistics.Count,
                TotalFiles = statistics.Sum(x => x.TotalFileCount),
                TotalSize = statistics.Sum(x => x.TotalSize)
            };
        }

        /// <summary>
        /// AI分析代码模块
        /// </summary>
        public async Task<AnalyzeCodeModulesOutput> AnalyzeCodeModulesAsync(AnalyzeCodeModulesInput input)
        {
            var userId = _currentUser.GetUserId();

            // 验证代码管理是否存在
            var codeManagement = await _sqlSugarClient.Queryable<CodeManagement>()
                .Where(x => x.Id == input.CodeManagementId && !x.IsDeleted && x.CreatorId == userId)
                .FirstAsync();

            if (codeManagement == null)
            {
                throw new Exception("代码项目不存在");
            }

            // 获取所有代码片段（只获取文件，不包括文件夹）
            var allSnippets = await _sqlSugarClient.Queryable<CodeSnippet>()
                .Where(x => x.CodeManagementId == input.CodeManagementId && !x.IsFolder)
                .Select(x => new
                {
                    x.Id,
                    x.FileName,
                    x.FilePath,
                    x.FileExtension
                })
                .ToListAsync();

            // 过滤掉工具类、配置类等非业务逻辑文件
            var codeSnippets = allSnippets.Where(x => !IsNonBusinessFile(x.FilePath, x.FileName, x.FileExtension)).ToList();

            if (codeSnippets.Count == 0)
            {
                throw new Exception("该项目没有可分析的业务代码文件");
            }

            // 创建 数字ID -> Guid 的映射字典，节省token
            var indexToGuidMap = new Dictionary<int, Guid>();
            var fileListForAI = new List<object>();

            for (int i = 0; i < codeSnippets.Count; i++)
            {
                var snippet = codeSnippets[i];
                var numericId = i + 1; // 从1开始编号

                // 保存映射关系
                indexToGuidMap[numericId] = snippet.Id;

                // 构建发送给AI的数据，使用数字ID（去掉FilePath，节省token）
                fileListForAI.Add(new
                {
                    Id = numericId,
                    FileName = snippet.FileName,
                    FileExtension = snippet.FileExtension
                });
            }

            // 构建文件列表信息，发送给DeepSeek
            var fileListJson = JsonSerializer.Serialize(fileListForAI);

            // 调用DeepSeek API进行分析
            var systemPrompt = _deepSeekService.GetSystemPrompt(DeepSeekSystemPrompt.CodeModuleClassification);
            var analysisResult = await _deepSeekService.ChatCompletionAsync<AnalyzeCodeModulesOutput>(
                systemPrompt,
                fileListJson,
                4000 // 增加token限制，因为可能返回较多模块
            );

            // 验证和处理结果
            if (analysisResult == null || analysisResult.Modules == null || analysisResult.Modules.Count == 0)
            {
                throw new Exception("AI分析失败，未能识别出代码模块");
            }

            // 将AI返回的数字ID还原为Guid，并添加文件路径列表
            foreach (var module in analysisResult.Modules)
            {
                if (module.FileIds != null && module.FileIds.Count > 0)
                {
                    var guidList = new List<string>();
                    var filePathList = new List<string>();

                    foreach (var fileIdStr in module.FileIds)
                    {
                        // 尝试将字符串转为数字ID
                        if (int.TryParse(fileIdStr, out var numericId))
                        {
                            // 根据数字ID找到对应的Guid和文件信息
                            if (indexToGuidMap.TryGetValue(numericId, out var guid))
                            {
                                guidList.Add(guid.ToString());

                                // 查找对应的文件路径
                                var fileInfo = codeSnippets.FirstOrDefault(x => x.Id == guid);
                                if (fileInfo != null)
                                {
                                    filePathList.Add(fileInfo.FilePath);
                                }
                            }
                        }
                    }

                    module.FileIds = guidList;
                    module.FilePaths = filePathList;
                }
            }

            return analysisResult;
        }

        /// <summary>
        /// 批量拆分代码模块（优化版：减少数据库查询和操作）
        /// </summary>
        public async Task BatchSplitModulesAsync(BatchSplitModulesInput input)
        {
            var userId = _currentUser.GetUserId();

            // 验证源代码管理是否存在
            var sourceCodeManagement = await _sqlSugarClient.Queryable<CodeManagement>()
                .Where(x => x.Id == input.SourceCodeManagementId && !x.IsDeleted && x.CreatorId == userId)
                .FirstAsync();

            if (sourceCodeManagement == null)
            {
                throw new Exception("源代码项目不存在");
            }

            if (input.SelectedModules == null || input.SelectedModules.Count == 0)
            {
                throw new Exception("请至少选择一个模块");
            }

            // 预处理模块数据，过滤无效模块
            var validModules = new List<(CodeModuleInfo module, List<Guid> fileIds)>();
            foreach (var module in input.SelectedModules)
            {
                if (module.FileIds == null || module.FileIds.Count == 0)
                {
                    continue;
                }

                // 将字符串ID转换为Guid
                var fileIds = new List<Guid>();
                foreach (var fileIdStr in module.FileIds)
                {
                    if (Guid.TryParse(fileIdStr, out var fileId))
                    {
                        fileIds.Add(fileId);
                    }
                }

                if (fileIds.Count > 0)
                {
                    validModules.Add((module, fileIds));
                }
            }

            if (validModules.Count == 0)
            {
                throw new Exception("没有有效的模块可以拆分");
            }

            // 一次性查询所有源代码片段（避免重复查询）
            var allSourceSnippets = await _sqlSugarClient.Queryable<CodeSnippet>()
                .Where(x => x.CodeManagementId == input.SourceCodeManagementId && !x.IsDeleted)
                .OrderBy(x => x.Level)
                .ToListAsync();

            // 构建ID到对象的映射，方便快速查找
            var snippetDict = allSourceSnippets.ToDictionary(x => x.Id, x => x);

            // 批量创建新的代码管理记录
            var newCodeManagements = new List<CodeManagement>();
            var allNewSnippets = new List<CodeSnippet>();
            var moduleIdMapping = new Dictionary<int, Guid>(); // 模块索引 -> 新项目ID

            for (int i = 0; i < validModules.Count; i++)
            {
                var (module, fileIds) = validModules[i];

                // 创建新的代码管理记录
                var newCodeManagement = new CodeManagement
                {
                    Id = Guid.NewGuid(),
                    ProjectName = sourceCodeManagement.ProjectName,
                    Description = module.ModuleName,
                    Remark = $"模块路径：{module.ModulePath}",
                    TotalFileCount = 0,
                    TotalSize = 0
                };

                newCodeManagements.Add(newCodeManagement);
                moduleIdMapping[i] = newCodeManagement.Id;

                // 收集该模块需要复制的所有节点ID（包括父文件夹）
                var allSnippetIds = new HashSet<Guid>(fileIds);
                foreach (var fileId in fileIds)
                {
                    if (snippetDict.TryGetValue(fileId, out var snippet))
                    {
                        CollectParentFoldersInMemory(snippet.ParentId, allSnippetIds, snippetDict);
                    }
                }

                // 筛选出需要复制的节点
                var moduleSnippets = allSourceSnippets
                    .Where(x => allSnippetIds.Contains(x.Id))
                    .OrderBy(x => x.Level)
                    .ToList();

                // 创建ID映射
                var idMapping = new Dictionary<Guid, Guid>();
                foreach (var snippet in moduleSnippets)
                {
                    idMapping[snippet.Id] = Guid.NewGuid();
                }

                // 构建新的代码片段列表
                var sortOrder = 0;
                var moduleNewSnippets = new List<CodeSnippet>();

                foreach (var snippet in moduleSnippets)
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

                    moduleNewSnippets.Add(newSnippet);
                }

                allNewSnippets.AddRange(moduleNewSnippets);

                // 更新统计信息
                var totalFileCount = moduleSnippets.Count(x => !x.IsFolder);
                var totalSize = moduleSnippets.Where(x => !x.IsFolder).Sum(x => x.FileSize);

                newCodeManagement.TotalFileCount = totalFileCount;
                newCodeManagement.TotalSize = totalSize;
            }

            // 批量插入新的代码管理记录
            if (newCodeManagements.Count > 0)
            {
                await _sqlSugarClient.Insertable(newCodeManagements).ExecuteCommandAsync();
            }

            // 批量插入所有新的代码片段
            if (allNewSnippets.Count > 0)
            {
                await _sqlSugarClient.Insertable(allNewSnippets).ExecuteCommandAsync();
            }
        }

        /// <summary>
        /// 在内存中递归收集父文件夹ID（避免多次数据库查询）
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

        /// <summary>
        /// 判断是否为非业务逻辑文件（工具类、配置类等）
        /// </summary>
        private bool IsNonBusinessFile(string filePath, string fileName, string fileExtension)
        {
            if (string.IsNullOrWhiteSpace(filePath) || string.IsNullOrWhiteSpace(fileName))
            {
                return true;
            }

            var lowerPath = filePath.ToLower();
            var lowerName = fileName.ToLower();
            var lowerExt = (fileExtension ?? "").ToLower();

            // 排除的文件扩展名（样式、配置等）
            var excludedExtensions = new[]
            {
                ".css", ".scss", ".sass", ".less", ".styl",  // 样式文件
                ".json", ".xml", ".yaml", ".yml", ".toml",   // 配置文件
                ".md", ".txt", ".pdf", ".doc", ".docx",      // 文档文件
                ".jpg", ".jpeg", ".png", ".gif", ".svg", ".ico", ".webp",  // 图片文件
                ".woff", ".woff2", ".ttf", ".eot",           // 字体文件
                ".map",                                       // Source Map
                ".config"                                     // 配置文件
            };

            if (excludedExtensions.Contains(lowerExt))
            {
                return true;
            }

            // 排除的文件名（精确匹配）
            var excludedFileNames = new[]
            {
                "package.json", "package-lock.json", "yarn.lock",
                "tsconfig.json", "jsconfig.json",
                "webpack.config.js", "vue.config.js", "vite.config.js",
                "babel.config.js", "jest.config.js", "postcss.config.js",
                ".eslintrc.js", ".prettierrc.js", ".gitignore", ".editorconfig",
                "program.cs", "startup.cs", "appsettings.json", "appsettings.development.json",
                "web.config", "app.config"
            };

            if (excludedFileNames.Contains(lowerName))
            {
                return true;
            }

            // 排除的目录路径关键词
            var excludedPathKeywords = new[]
            {
                "/utils/", "/util/", "/helpers/", "/helper/",           // 工具类目录
                "/extensions/", "/extension/",                          // 扩展类目录
                "/config/", "/configs/", "/configuration/",             // 配置目录
                "/assets/", "/static/", "/public/",                     // 静态资源
                "/styles/", "/css/", "/scss/",                          // 样式目录
                "/fonts/", "/images/", "/img/", "/icons/",              // 资源目录
                "/lib/", "/libs/", "/vendor/", "/third-party/",         // 第三方库
                "/node_modules/", "/dist/", "/build/", "/bin/", "/obj/", // 构建目录
                "/migrations/", "/seeds/",                               // 数据库迁移
                "/tests/", "/test/", "/__tests__/", "/spec/",           // 测试目录
                "/docs/", "/documentation/",                             // 文档目录
                "/components/",                                          // 通用组件目录（排除，保留views、pages等业务页面）
                "/common/", "/shared/", "/core/",                       // 公共/共享目录
                "/layout/", "/layouts/"                                  // 布局目录
            };

            if (excludedPathKeywords.Any(keyword => lowerPath.Contains(keyword)))
            {
                return true;
            }

            // 排除的文件名关键词（包含匹配）
            var excludedNameKeywords = new[]
            {
                "utils", "util", "helper", "helpers",
                "extension", "extensions", "ext",
                "config", "configuration", "settings",
                "constant", "constants", "const",
                "enum", "enums",
                "base", "abstract",
                "interface", "iservice",
                ".min.", ".bundle.",
                "mixin", "mixins",                          // Vue mixins
                "directive", "directives",                  // 指令
                "filter", "filters",                        // 过滤器
                "plugin", "plugins",                        // 插件
                "permission", "auth"                        // 权限认证（通用）
            };

            if (excludedNameKeywords.Any(keyword => lowerName.Contains(keyword)))
            {
                return true;
            }

            return false;
        }
    }
}
