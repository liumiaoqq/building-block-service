using Web.Manager;

namespace Web.Service
{
    public class ErDiagramSessionService
    {
        private readonly ISqlSugarClient _sqlSugarClient;
        private readonly ICurrentUser _currentUser;
        private readonly ErDiagramSessionManager _erDiagramSessionManager;

        public ErDiagramSessionService(
            ISqlSugarClient sqlSugarClient,
            ICurrentUser currentUser,
            ErDiagramSessionManager erDiagramSessionManager)
        {
            _sqlSugarClient = sqlSugarClient;
            _currentUser = currentUser;
            _erDiagramSessionManager = erDiagramSessionManager;
        }

        /// <summary>
        /// 获取ER图会话分页列表
        /// </summary>
        public async Task<PagedReuslt<ErDiagramSessionDto>> ListAsync(GetErDiagramSessionPagedInput input)
        {
            return await _erDiagramSessionManager.ListAsync(input);
        }

        /// <summary>
        /// 根据ID获取ER图会话
        /// </summary>
        public async Task<ErDiagramSessionDto> GetByIdAsync(Guid id)
        {
            return await _erDiagramSessionManager.GetByIdAsync(id);
        }

        /// <summary>
        /// 获取当前激活的ER图会话
        /// </summary>
        public async Task<ErDiagramSessionDto> GetActiveErDiagramSessionAsync(Guid? codeManagementId = null)
        {
            return await _erDiagramSessionManager.GetActiveErDiagramSessionAsync(codeManagementId);
        }

        /// <summary>
        /// 获取或创建激活的ER图会话
        /// </summary>
        public async Task<ErDiagramSessionDto> GetOrCreateActiveErDiagramSessionAsync(GetOrCreateActiveErDiagramSessionInput input)
        {
            return await _erDiagramSessionManager.GetOrCreateActiveErDiagramSessionAsync(input);
        }

        /// <summary>
        /// 创建ER图会话
        /// </summary>
        public async Task<Guid> CreateAsync(CreateErDiagramSessionInput input)
        {
            return await _erDiagramSessionManager.CreateAsync(input);
        }

        /// <summary>
        /// 更新ER图会话
        /// </summary>
        public async Task UpdateAsync(UpdateErDiagramSessionInput input)
        {
            await _erDiagramSessionManager.UpdateAsync(input);
        }

        /// <summary>
        /// 设置激活的ER图会话
        /// </summary>
        public async Task SetActiveErDiagramSessionAsync(SetActiveErDiagramSessionInput input)
        {
            await _erDiagramSessionManager.SetActiveErDiagramSessionAsync(input);
        }

        /// <summary>
        /// 删除ER图会话
        /// </summary>
        public async Task DeleteAsync(Guid id)
        {
            await _erDiagramSessionManager.DeleteAsync(id);
        }

        /// <summary>
        /// AI生成ER图相关数据
        /// </summary>
        public async Task<ErDiagramSessionDto> GenerateErDiagramByAIAsync(GenerateErDiagramByAIInput input)
        {
            return await _erDiagramSessionManager.GenerateErDiagramByAIAsync(input);
        }
    }
}
