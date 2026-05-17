using Web.Manager;

namespace Web.Service
{
    public class UseCaseDiagramSessionService
    {
        private readonly ISqlSugarClient _sqlSugarClient;
        private readonly ICurrentUser _currentUser;
        private readonly UseCaseDiagramSessionManager _useCaseDiagramSessionManager;

        public UseCaseDiagramSessionService(
            ISqlSugarClient sqlSugarClient,
            ICurrentUser currentUser,
            UseCaseDiagramSessionManager useCaseDiagramSessionManager)
        {
            _sqlSugarClient = sqlSugarClient;
            _currentUser = currentUser;
            _useCaseDiagramSessionManager = useCaseDiagramSessionManager;
        }

        /// <summary>
        /// 获取用例图会话分页列表
        /// </summary>
        public async Task<PagedReuslt<UseCaseDiagramSessionDto>> ListAsync(GetUseCaseDiagramSessionPagedInput input)
        {
            return await _useCaseDiagramSessionManager.ListAsync(input);
        }

        /// <summary>
        /// 根据ID获取用例图会话
        /// </summary>
        public async Task<UseCaseDiagramSessionDto> GetByIdAsync(Guid id)
        {
            return await _useCaseDiagramSessionManager.GetByIdAsync(id);
        }

        /// <summary>
        /// 获取当前激活的用例图会话
        /// </summary>
        public async Task<UseCaseDiagramSessionDto> GetActiveUseCaseDiagramSessionAsync()
        {
            return await _useCaseDiagramSessionManager.GetActiveUseCaseDiagramSessionAsync();
        }

        /// <summary>
        /// 获取或创建激活的用例图会话
        /// </summary>
        public async Task<UseCaseDiagramSessionDto> GetOrCreateActiveUseCaseDiagramSessionAsync(GetOrCreateActiveUseCaseDiagramSessionInput input)
        {
            return await _useCaseDiagramSessionManager.GetOrCreateActiveUseCaseDiagramSessionAsync(input);
        }

        /// <summary>
        /// 创建用例图会话
        /// </summary>
        public async Task<Guid> CreateAsync(CreateUseCaseDiagramSessionInput input)
        {
            return await _useCaseDiagramSessionManager.CreateAsync(input);
        }

        /// <summary>
        /// 更新用例图会话
        /// </summary>
        public async Task UpdateAsync(UpdateUseCaseDiagramSessionInput input)
        {
            await _useCaseDiagramSessionManager.UpdateAsync(input);
        }

        /// <summary>
        /// 设置激活的用例图会话
        /// </summary>
        public async Task SetActiveUseCaseDiagramSessionAsync(SetActiveUseCaseDiagramSessionInput input)
        {
            await _useCaseDiagramSessionManager.SetActiveUseCaseDiagramSessionAsync(input);
        }

        /// <summary>
        /// 删除用例图会话
        /// </summary>
        public async Task DeleteAsync(Guid id)
        {
            await _useCaseDiagramSessionManager.DeleteAsync(id);
        }

        /// <summary>
        /// AI生成用例图相关数据
        /// </summary>
        public async Task<UseCaseDiagramSessionDto> GenerateUseCaseDiagramByAIAsync(GenerateUseCaseDiagramByAIInput input)
        {
            return await _useCaseDiagramSessionManager.GenerateUseCaseDiagramByAIAsync(input);
        }
    }
}
