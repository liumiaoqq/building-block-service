using Web.Manager;

namespace Web.Service
{
    public class FlowsheetSessionService
    {
        private readonly ISqlSugarClient _sqlSugarClient;
        private readonly ICurrentUser _currentUser;
        private readonly FlowsheetSessionManager _flowsheetSessionManager;

        public FlowsheetSessionService(
            ISqlSugarClient sqlSugarClient,
            ICurrentUser currentUser,
            FlowsheetSessionManager flowsheetSessionManager)
        {
            _sqlSugarClient = sqlSugarClient;
            _currentUser = currentUser;
            _flowsheetSessionManager = flowsheetSessionManager;
        }

        /// <summary>
        /// 获取流程图会话分页列表
        /// </summary>
        public async Task<PagedReuslt<FlowsheetSessionDto>> ListAsync(GetFlowsheetSessionPagedInput input)
        {
            return await _flowsheetSessionManager.ListAsync(input);
        }

        /// <summary>
        /// 根据ID获取流程图会话
        /// </summary>
        public async Task<FlowsheetSessionDto> GetByIdAsync(Guid id)
        {
            return await _flowsheetSessionManager.GetByIdAsync(id);
        }

        /// <summary>
        /// 获取当前激活的流程图会话
        /// </summary>
        public async Task<FlowsheetSessionDto> GetActiveFlowsheetSessionAsync(Guid? moduleId = null)
        {
            return await _flowsheetSessionManager.GetActiveFlowsheetSessionAsync(moduleId);
        }

        /// <summary>
        /// 获取或创建激活的流程图会话
        /// </summary>
        public async Task<FlowsheetSessionDto> GetOrCreateActiveFlowsheetSessionAsync(GetOrCreateActiveFlowsheetSessionInput input)
        {
            return await _flowsheetSessionManager.GetOrCreateActiveFlowsheetSessionAsync(input);
        }

        /// <summary>
        /// 创建流程图会话
        /// </summary>
        public async Task<Guid> CreateAsync(CreateFlowsheetSessionInput input)
        {
            return await _flowsheetSessionManager.CreateAsync(input);
        }

        /// <summary>
        /// 更新流程图会话
        /// </summary>
        public async Task UpdateAsync(UpdateFlowsheetSessionInput input)
        {
            await _flowsheetSessionManager.UpdateAsync(input);
        }

        /// <summary>
        /// 设置激活的流程图会话
        /// </summary>
        public async Task SetActiveFlowsheetSessionAsync(SetActiveFlowsheetSessionInput input)
        {
            await _flowsheetSessionManager.SetActiveFlowsheetSessionAsync(input);
        }

        /// <summary>
        /// 删除流程图会话
        /// </summary>
        public async Task DeleteAsync(Guid id)
        {
            await _flowsheetSessionManager.DeleteAsync(id);
        }

        /// <summary>
        /// AI生成PlantUML流程图
        /// </summary>
        public async Task<string> GeneratePlantUMLByAIAsync(GeneratePlantUMLByAIInput input)
        {
            return await _flowsheetSessionManager.GeneratePlantUMLByAIAsync(input);
        }
    }
}

