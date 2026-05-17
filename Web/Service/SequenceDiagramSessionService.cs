using Web.Manager;

namespace Web.Service
{
    public class SequenceDiagramSessionService
    {
        private readonly ISqlSugarClient _sqlSugarClient;
        private readonly ICurrentUser _currentUser;
        private readonly SequenceDiagramSessionManager _sequenceDiagramSessionManager;

        public SequenceDiagramSessionService(
            ISqlSugarClient sqlSugarClient,
            ICurrentUser currentUser,
            SequenceDiagramSessionManager sequenceDiagramSessionManager)
        {
            _sqlSugarClient = sqlSugarClient;
            _currentUser = currentUser;
            _sequenceDiagramSessionManager = sequenceDiagramSessionManager;
        }

        /// <summary>
        /// 获取时序图会话分页列表
        /// </summary>
        public async Task<PagedReuslt<SequenceDiagramSessionDto>> ListAsync(GetSequenceDiagramSessionPagedInput input)
        {
            return await _sequenceDiagramSessionManager.ListAsync(input);
        }

        /// <summary>
        /// 根据ID获取时序图会话
        /// </summary>
        public async Task<SequenceDiagramSessionDto> GetByIdAsync(Guid id)
        {
            return await _sequenceDiagramSessionManager.GetByIdAsync(id);
        }

        /// <summary>
        /// 获取当前激活的时序图会话
        /// </summary>
        public async Task<SequenceDiagramSessionDto> GetActiveSequenceDiagramSessionAsync(Guid? moduleId = null)
        {
            return await _sequenceDiagramSessionManager.GetActiveSequenceDiagramSessionAsync(moduleId);
        }

        /// <summary>
        /// 获取或创建激活的时序图会话
        /// </summary>
        public async Task<SequenceDiagramSessionDto> GetOrCreateActiveSequenceDiagramSessionAsync(GetOrCreateActiveSequenceDiagramSessionInput input)
        {
            return await _sequenceDiagramSessionManager.GetOrCreateActiveSequenceDiagramSessionAsync(input);
        }

        /// <summary>
        /// 创建时序图会话
        /// </summary>
        public async Task<Guid> CreateAsync(CreateSequenceDiagramSessionInput input)
        {
            return await _sequenceDiagramSessionManager.CreateAsync(input);
        }

        /// <summary>
        /// 更新时序图会话
        /// </summary>
        public async Task UpdateAsync(UpdateSequenceDiagramSessionInput input)
        {
            await _sequenceDiagramSessionManager.UpdateAsync(input);
        }

        /// <summary>
        /// 设置激活的时序图会话
        /// </summary>
        public async Task SetActiveSequenceDiagramSessionAsync(SetActiveSequenceDiagramSessionInput input)
        {
            await _sequenceDiagramSessionManager.SetActiveSequenceDiagramSessionAsync(input);
        }

        /// <summary>
        /// 删除时序图会话
        /// </summary>
        public async Task DeleteAsync(Guid id)
        {
            await _sequenceDiagramSessionManager.DeleteAsync(id);
        }

        /// <summary>
        /// AI生成PlantUML时序图
        /// </summary>
        public async Task<string> GenerateSequenceDiagramByAIAsync(GenerateSequenceDiagramByAIInput input)
        {
            return await _sequenceDiagramSessionManager.GenerateSequenceDiagramByAIAsync(input);
        }
    }
}

