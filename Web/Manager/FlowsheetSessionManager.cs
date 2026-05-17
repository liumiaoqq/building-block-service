using System;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using Web.HttpClientApi.DeepSeek.Service;
using Web.HttpClient;
using Web.HttpClientApi.DeepSeek.Dto;

namespace Web.Manager
{
    public class FlowsheetSessionManager
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        private readonly ICurrentUser _currentUser;

        private readonly IDeepSeekService _deepSeekService;

        public FlowsheetSessionManager(ISqlSugarClient sqlSugarClient, ICurrentUser currentUser, IDeepSeekService deepSeekService)
        {
            _sqlSugarClient = sqlSugarClient;
            _currentUser = currentUser;
            _deepSeekService = deepSeekService;
        }

        /// <summary>
        /// 获取流程图会话分页列表
        /// </summary>
        public async Task<PagedReuslt<FlowsheetSessionDto>> ListAsync(GetFlowsheetSessionPagedInput input)
        {
            var userId = _currentUser.GetUserId();
            var result = await _sqlSugarClient.Queryable<FlowsheetSession>()

                .Where(x => x.CreatorId == userId)
                .WhereIF(input.Name.IsNotNullOrNotWhiteSpace(), x => x.Name.Contains(input.Name))
                .WhereIF(input.CodeManagementId.HasValue, x => x.CodeManagementId == input.CodeManagementId)
                .WhereIF(input.IsActive.HasValue, x => x.IsActive == input.IsActive)
                .OrderByDescending(x => x.IsActive)
                .OrderByDescending(x => x.Id)
                .Select<FlowsheetSessionDto>()
                .ToPageListAsync(input.Page, input.Size);

            return new PagedReuslt<FlowsheetSessionDto>(result, result.Count);
        }

        /// <summary>
        /// 根据ID获取流程图会话
        /// </summary>
        public async Task<FlowsheetSessionDto> GetByIdAsync(Guid id)
        {
            var userId = _currentUser.GetUserId();
            return await _sqlSugarClient.Queryable<FlowsheetSession>()
                .Where(x => x.Id == id)
                .Where(x => x.CreatorId == userId)
                .Select<FlowsheetSessionDto>()
                .FirstAsync();
        }

        /// <summary>
        /// 获取当前激活的流程图会话
        /// </summary>
        public async Task<FlowsheetSessionDto> GetActiveFlowsheetSessionAsync(Guid? codeManagementId = null)
        {
            var userId = _currentUser.GetUserId();
            return await _sqlSugarClient.Queryable<FlowsheetSession>()

                .Where(x => x.CreatorId == userId)
                .Where(x => x.IsActive == true)
                .WhereIF(codeManagementId.HasValue, x => x.CodeManagementId == codeManagementId)
                .OrderByDescending(x => x.Id)
                .Select<FlowsheetSessionDto>()
                .FirstAsync();
        }

        /// <summary>
        /// 获取或创建激活的流程图会话
        /// </summary>
        public async Task<FlowsheetSessionDto> GetOrCreateActiveFlowsheetSessionAsync(GetOrCreateActiveFlowsheetSessionInput input)
        {
            var userId = _currentUser.GetUserId();

            // 先查询是否存在激活的流程图会话
            var activeFlowsheet = await _sqlSugarClient.Queryable<FlowsheetSession>()

                .Where(x => x.CreatorId == userId)
                .Where(x => x.IsActive == true)
                .WhereIF(input.CodeManagementId.HasValue, x => x.CodeManagementId == input.CodeManagementId)
                .OrderByDescending(x => x.Id)
                .Select<FlowsheetSessionDto>()
                .FirstAsync();

            // 如果存在，直接返回
            if (activeFlowsheet != null)
            {
                return activeFlowsheet;
            }

            // 如果不存在，创建一个新的激活流程图会话
            var entity = new FlowsheetSession
            {
                Name = "未命名",
                CodeManagementId = input.CodeManagementId,
                IsActive = true,
                FlowsheetContent = null,
                Remark = null
            };

            await _sqlSugarClient.Insertable(entity).ExecuteCommandAsync();

            return await GetByIdAsync(entity.Id);
        }

        /// <summary>
        /// 创建流程图会话
        /// </summary>
        public async Task<Guid> CreateAsync(CreateFlowsheetSessionInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Name))
            {
                throw new Exception("会话名称不能为空");
            }

            var userId = _currentUser.GetUserId();

            // 如果新建的流程图会话设置为激活，需要将其他激活的流程图会话设置为非激活
            if (input.IsActive)
            {
                // 先查询出需要修改的会话
                var activeSessions = await _sqlSugarClient.Queryable<FlowsheetSession>()

                    .Where(x => x.CreatorId == userId)
                    .Where(x => x.IsActive == true)
                    .WhereIF(input.CodeManagementId.HasValue, x => x.CodeManagementId == input.CodeManagementId)
                    .ToListAsync();

                // 再修改这些会话的激活状态
                if (activeSessions != null && activeSessions.Count > 0)
                {
                    foreach (var session in activeSessions)
                    {
                        session.IsActive = false;
                    }
                    await _sqlSugarClient.Updateable(activeSessions)
                        .UpdateColumns(x => new { x.IsActive })
                        .ExecuteCommandAsync();
                }
            }

            var entity = new FlowsheetSession
            {
                Name = input.Name,
                CodeManagementId = input.CodeManagementId,
                IsActive = input.IsActive,
                FlowsheetContent = input.FlowsheetContent,
                Remark = input.Remark
            };

            await _sqlSugarClient.Insertable(entity).ExecuteCommandAsync();
            return entity.Id;
        }

        /// <summary>
        /// 更新流程图会话
        /// </summary>
        public async Task UpdateAsync(UpdateFlowsheetSessionInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Name))
            {
                throw new Exception("会话名称不能为空");
            }

            var userId = _currentUser.GetUserId();
            var entity = await _sqlSugarClient.Queryable<FlowsheetSession>()
                .Where(x => x.Id == input.Id)
                .Where(x => x.CreatorId == userId)
                .FirstAsync();

            if (entity == null)
            {
                throw new Exception("流程图会话不存在");
            }

            // 如果将此流程图会话设置为激活，需要将其他激活的流程图会话设置为非激活
            if (input.IsActive && !entity.IsActive)
            {
                // 先查询出需要修改的会话
                var activeSessions = await _sqlSugarClient.Queryable<FlowsheetSession>()

                    .Where(x => x.CreatorId == userId)
                    .Where(x => x.Id != input.Id)
                    .Where(x => x.IsActive == true)

                    .ToListAsync();

                // 再修改这些会话的激活状态
                if (activeSessions != null && activeSessions.Count > 0)
                {
                    foreach (var session in activeSessions)
                    {
                        session.IsActive = false;
                    }
                    await _sqlSugarClient.Updateable(activeSessions)
                        .UpdateColumns(x => new { x.IsActive })
                        .ExecuteCommandAsync();
                }
            }

            entity.Name = input.Name;
            entity.CodeManagementId = input.CodeManagementId;
            entity.IsActive = input.IsActive;
            entity.FlowsheetContent = input.FlowsheetContent;
            entity.Remark = input.Remark;

            await _sqlSugarClient.Updateable(entity).ExecuteCommandAsync();
        }

        /// <summary>
        /// 设置激活的流程图会话
        /// </summary>
        public async Task SetActiveFlowsheetSessionAsync(SetActiveFlowsheetSessionInput input)
        {
            var userId = _currentUser.GetUserId();
            var entity = await _sqlSugarClient.Queryable<FlowsheetSession>()
                .Where(x => x.Id == input.Id)
                .Where(x => x.CreatorId == userId)
                .FirstAsync();

            if (entity == null)
            {
                throw new Exception("流程图会话不存在");
            }

            // 先查询出其他激活的流程图会话
            var activeSessions = await _sqlSugarClient.Queryable<FlowsheetSession>()

                .Where(x => x.CreatorId == userId)
                .Where(x => x.Id != input.Id)
                .Where(x => x.IsActive == true)
                .WhereIF(entity.CodeManagementId.HasValue, x => x.CodeManagementId == entity.CodeManagementId)
                .ToListAsync();

            // 再修改这些会话的激活状态
            if (activeSessions != null && activeSessions.Count > 0)
            {
                foreach (var session in activeSessions)
                {
                    session.IsActive = false;
                }
                await _sqlSugarClient.Updateable(activeSessions)
                    .UpdateColumns(x => new { x.IsActive })
                    .ExecuteCommandAsync();
            }

            // 设置当前流程图会话为激活
            entity.IsActive = true;
            await _sqlSugarClient.Updateable(entity)
                .UpdateColumns(x => new { x.IsActive })
                .ExecuteCommandAsync();
        }

        /// <summary>
        /// 删除流程图会话
        /// </summary>
        public async Task DeleteAsync(Guid id)
        {
            var userId = _currentUser.GetUserId();
            await _sqlSugarClient.Updateable<FlowsheetSession>()
                .SetColumns(x => x.IsDeleted == true)
                .Where(x => x.Id == id)
                .Where(x => x.CreatorId == userId)
                .ExecuteCommandAsync();
        }

        /// <summary>
        /// AI生成PlantUML流程图
        /// </summary>
        public async Task<string> GeneratePlantUMLByAIAsync(GeneratePlantUMLByAIInput input)
        {
            var userId = _currentUser.GetUserId();

            // 获取代码项目信息
            var codeManagement = await _sqlSugarClient.Queryable<CodeManagement>()
                .Where(x => x.Id == input.CodeManagementId)
                .Where(x => x.CreatorId == userId)
                .FirstAsync();

            if (codeManagement == null)
            {
                throw new Exception("代码项目不存在");
            }

            // 获取代码项目的所有代码片段（只获取文件，不包括文件夹）
            var codeSnippets = await _sqlSugarClient.Queryable<CodeSnippet>()
                .Where(x => x.CodeManagementId == input.CodeManagementId)
                .Where(x => x.IsFolder == false)
                .OrderBy(x => x.FilePath)
                .ToListAsync();

            if (codeSnippets == null || codeSnippets.Count == 0)
            {
                throw new Exception("该代码项目暂无代码文件");
            }

            // 构建提示词
            var promptBuilder = new StringBuilder();
            promptBuilder.AppendLine($"项目名称：{codeManagement.ProjectName}");
            promptBuilder.AppendLine($"项目描述：{codeManagement.Description ?? "无"}");
            promptBuilder.AppendLine($"文件总数：{codeSnippets.Count}");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("用户需求：");
            promptBuilder.AppendLine(input.Question);
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("代码文件列表：");
            promptBuilder.AppendLine();

            foreach (var snippet in codeSnippets)
            {
                promptBuilder.AppendLine($"=== 文件：{snippet.FilePath} ===");
                promptBuilder.AppendLine($"文件名：{snippet.FileName}");
                promptBuilder.AppendLine($"文件类型：{snippet.FileType ?? "未知"}");
                promptBuilder.AppendLine($"文件大小：{snippet.FileSize} 字节");

                // 压缩文件内容（移除多余空行、注释等）
                var compressedContent = CompressCode(snippet.FileContent, snippet.FileType);

                // 如果文件内容过大，只保留关键部分
                if (compressedContent.Length > 2000)
                {
                    compressedContent = compressedContent.Substring(0, 2000) + "\n... (内容已截断)";
                }

                promptBuilder.AppendLine("文件内容：");
                promptBuilder.AppendLine(compressedContent);
                promptBuilder.AppendLine();
            }

            // 调用DeepSeek API
            var systemPrompt = _deepSeekService.GetSystemPrompt(DeepSeekSystemPrompt.PlantUMLGeneration);
            var result = await _deepSeekService.ChatCompletionAsync<PlantUMLGenerationResultDto>(
                systemPrompt,
                promptBuilder.ToString(),
                4000 // 增加max_tokens以支持更复杂的流程图
            );

            return result.PlantUMLCode;
        }

        /// <summary>
        /// 压缩代码内容
        /// </summary>
        private string CompressCode(string content, string fileType)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return string.Empty;
            }

            var lines = content.Split('\n');
            var compressedLines = new StringBuilder();

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();

                // 跳过空行
                if (string.IsNullOrWhiteSpace(trimmedLine))
                {
                    continue;
                }

                // 根据文件类型跳过注释
                if (fileType == "JavaScript" || fileType == "Vue" || fileType == "TypeScript" || fileType == "C#")
                {
                    // 跳过单行注释
                    if (trimmedLine.StartsWith("//") || trimmedLine.StartsWith("///"))
                    {
                        continue;
                    }
                }

                if (fileType == "CSS" || fileType == "SCSS")
                {
                    // 跳过CSS注释（简单处理）
                    if (trimmedLine.StartsWith("/*") || trimmedLine.StartsWith("*"))
                    {
                        continue;
                    }
                }

                // 保留有效代码行
                compressedLines.AppendLine(trimmedLine);
            }

            return compressedLines.ToString();
        }
    }
}

