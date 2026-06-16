using System;
using System.Threading.Tasks;
using Web.HttpClient;
using Web.HttpClientApi.DeepSeek.Dto;
using Web.HttpClientApi.DeepSeek.Service;

namespace Web.Manager
{
    public class SequenceDiagramSessionManager
    {
        private readonly ISqlSugarClient _sqlSugarClient;
        private readonly ICurrentUser _currentUser;
        private readonly IDeepSeekService _deepSeekService;

        public SequenceDiagramSessionManager(ISqlSugarClient sqlSugarClient, ICurrentUser currentUser, IDeepSeekService deepSeekService)
        {
            _sqlSugarClient = sqlSugarClient;
            _currentUser = currentUser;
            _deepSeekService = deepSeekService;
        }

        public async Task<PagedReuslt<SequenceDiagramSessionDto>> ListAsync(GetSequenceDiagramSessionPagedInput input)
        {
            var userId = _currentUser.GetUserId();
            var result = await _sqlSugarClient.Queryable<SequenceDiagramSession>()
                .Where(x => x.CreatorId == userId)
                .WhereIF(input.Name.IsNotNullOrNotWhiteSpace(), x => x.Name.Contains(input.Name))
                .WhereIF(input.CodeManagementId.HasValue, x => x.CodeManagementId == input.CodeManagementId)
                .WhereIF(input.IsActive.HasValue, x => x.IsActive == input.IsActive)
                .OrderByDescending(x => x.IsActive)
                .OrderByDescending(x => x.Id)
                .Select<SequenceDiagramSessionDto>()
                .ToPageListAsync(input.Page, input.Size);

            return new PagedReuslt<SequenceDiagramSessionDto>(result, result.Count);
        }

        public async Task<SequenceDiagramSessionDto> GetByIdAsync(Guid id)
        {
            var userId = _currentUser.GetUserId();
            return await _sqlSugarClient.Queryable<SequenceDiagramSession>()
                .Where(x => x.Id == id)
                .Where(x => x.CreatorId == userId)
                .Select<SequenceDiagramSessionDto>()
                .FirstAsync();
        }

        public async Task<SequenceDiagramSessionDto> GetActiveSequenceDiagramSessionAsync(Guid? codeManagementId = null)
        {
            var userId = _currentUser.GetUserId();
            return await _sqlSugarClient.Queryable<SequenceDiagramSession>()
                .Where(x => x.CreatorId == userId)
                .Where(x => x.IsActive == true)
                .WhereIF(codeManagementId.HasValue, x => x.CodeManagementId == codeManagementId)
                .OrderByDescending(x => x.Id)
                .Select<SequenceDiagramSessionDto>()
                .FirstAsync();
        }

        public async Task<SequenceDiagramSessionDto> GetOrCreateActiveSequenceDiagramSessionAsync(GetOrCreateActiveSequenceDiagramSessionInput input)
        {
            var userId = _currentUser.GetUserId();

            var activeSequenceDiagram = await _sqlSugarClient.Queryable<SequenceDiagramSession>()
                .Where(x => x.CreatorId == userId)
                .Where(x => x.IsActive == true)
                .WhereIF(input.CodeManagementId.HasValue, x => x.CodeManagementId == input.CodeManagementId)
                .OrderByDescending(x => x.Id)
                .Select<SequenceDiagramSessionDto>()
                .FirstAsync();

            if (activeSequenceDiagram != null)
            {
                return activeSequenceDiagram;
            }

            var entity = new SequenceDiagramSession
            {
                Name = "未命名",
                CodeManagementId = input.CodeManagementId,
                IsActive = true,
                SequenceDiagramContent = null,
                Remark = null
            };

            await _sqlSugarClient.Insertable(entity).ExecuteCommandAsync();
            return await GetByIdAsync(entity.Id);
        }

        public async Task<Guid> CreateAsync(CreateSequenceDiagramSessionInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Name))
            {
                throw new Exception("会话名称不能为空");
            }

            var userId = _currentUser.GetUserId();

            if (input.IsActive)
            {
                var activeSessions = await _sqlSugarClient.Queryable<SequenceDiagramSession>()
                    .Where(x => x.CreatorId == userId)
                    .Where(x => x.IsActive == true)
                    .WhereIF(input.CodeManagementId.HasValue, x => x.CodeManagementId == input.CodeManagementId)
                    .ToListAsync();

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

            var entity = new SequenceDiagramSession
            {
                Name = input.Name,
                CodeManagementId = input.CodeManagementId,
                IsActive = input.IsActive,
                SequenceDiagramContent = input.SequenceDiagramContent,
                Remark = input.Remark
            };

            await _sqlSugarClient.Insertable(entity).ExecuteCommandAsync();
            return entity.Id;
        }

        public async Task UpdateAsync(UpdateSequenceDiagramSessionInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Name))
            {
                throw new Exception("会话名称不能为空");
            }

            var userId = _currentUser.GetUserId();
            var entity = await _sqlSugarClient.Queryable<SequenceDiagramSession>()
                .Where(x => x.Id == input.Id)
                .Where(x => x.CreatorId == userId)
                .FirstAsync();

            if (entity == null)
            {
                throw new Exception("时序图会话不存在");
            }

            if (input.IsActive && !entity.IsActive)
            {
                var activeSessions = await _sqlSugarClient.Queryable<SequenceDiagramSession>()
                    .Where(x => x.CreatorId == userId)
                    .Where(x => x.Id != input.Id)
                    .Where(x => x.IsActive == true)
                    .ToListAsync();

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
            entity.SequenceDiagramContent = input.SequenceDiagramContent;
            entity.Remark = input.Remark;

            await _sqlSugarClient.Updateable(entity).ExecuteCommandAsync();
        }

        public async Task SetActiveSequenceDiagramSessionAsync(SetActiveSequenceDiagramSessionInput input)
        {
            var userId = _currentUser.GetUserId();
            var entity = await _sqlSugarClient.Queryable<SequenceDiagramSession>()
                .Where(x => x.Id == input.Id)
                .Where(x => x.CreatorId == userId)
                .FirstAsync();

            if (entity == null)
            {
                throw new Exception("时序图会话不存在");
            }

            var activeSessions = await _sqlSugarClient.Queryable<SequenceDiagramSession>()
                .Where(x => x.CreatorId == userId)
                .Where(x => x.Id != input.Id)
                .Where(x => x.IsActive == true)
                .WhereIF(entity.CodeManagementId.HasValue, x => x.CodeManagementId == entity.CodeManagementId)
                .ToListAsync();

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

            entity.IsActive = true;
            await _sqlSugarClient.Updateable(entity)
                .UpdateColumns(x => new { x.IsActive })
                .ExecuteCommandAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var userId = _currentUser.GetUserId();
            await _sqlSugarClient.Updateable<SequenceDiagramSession>()
                .SetColumns(x => x.IsDeleted == true)
                .Where(x => x.Id == id)
                .Where(x => x.CreatorId == userId)
                .ExecuteCommandAsync();
        }

        public async Task<string> GenerateSequenceDiagramByAIAsync(GenerateSequenceDiagramByAIInput input)
        {
            if (input == null || string.IsNullOrWhiteSpace(input.Question))
            {
                throw new Exception("请输入AI提问");
            }

            var systemPrompt = _deepSeekService.GetSystemPrompt(DeepSeekSystemPrompt.PlantUMLSequenceDiagramGeneration);
            var result = await _deepSeekService.ChatCompletionAsync<SequenceDiagramGenerationResultDto>(
                systemPrompt,
                input.Question.Trim(),
                4000
            );

            return result.PlantUMLCode;
        }
    }
}
