using Web.Extensions;
using Web.Service;
using Web.Tables;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FlowsheetSessionController : YouJuController<FlowsheetSession, FlowsheetSessionDto, GetFlowsheetSessionPagedInput>
    {
        private readonly FlowsheetSessionService _flowsheetSessionService;

        public FlowsheetSessionController(
            IServiceProvider serviceProvider,
            FlowsheetSessionService flowsheetSessionService) : base(serviceProvider)
        {
            _flowsheetSessionService = flowsheetSessionService;
        }

        /// <summary>
        /// 获取流程图会话分页列表
        /// </summary>
        [HttpPost("ListAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async override Task<PagedReuslt<FlowsheetSessionDto>> ListAsync(GetFlowsheetSessionPagedInput input)
        {
            return await _flowsheetSessionService.ListAsync(input);
        }

        /// <summary>
        /// 根据ID获取流程图会话
        /// </summary>
        [HttpPost("GetByIdAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<FlowsheetSessionDto> GetByIdAsync(IdInput<Guid> input)
        {
            return await _flowsheetSessionService.GetByIdAsync(input.Id);
        }

        /// <summary>
        /// 获取当前激活的流程图会话
        /// </summary>
        [HttpPost("GetActiveFlowsheetSessionAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<FlowsheetSessionDto> GetActiveFlowsheetSessionAsync(GetOrCreateActiveFlowsheetSessionInput input)
        {
            return await _flowsheetSessionService.GetActiveFlowsheetSessionAsync();
        }

        /// <summary>
        /// 获取或创建激活的流程图会话
        /// </summary>
        [HttpPost("GetOrCreateActiveFlowsheetSessionAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<FlowsheetSessionDto> GetOrCreateActiveFlowsheetSessionAsync(GetOrCreateActiveFlowsheetSessionInput input)
        {
            return await _flowsheetSessionService.GetOrCreateActiveFlowsheetSessionAsync(input);
        }

        /// <summary>
        /// 创建流程图会话
        /// </summary>
        [HttpPost("CreateAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<Guid> CreateAsync(CreateFlowsheetSessionInput input)
        {
            return await _flowsheetSessionService.CreateAsync(input);
        }

        /// <summary>
        /// 更新流程图会话
        /// </summary>
        [HttpPost("UpdateAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task UpdateAsync(UpdateFlowsheetSessionInput input)
        {
            await _flowsheetSessionService.UpdateAsync(input);
        }

        /// <summary>
        /// 设置激活的流程图会话
        /// </summary>
        [HttpPost("SetActiveFlowsheetSessionAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task SetActiveFlowsheetSessionAsync(SetActiveFlowsheetSessionInput input)
        {
            await _flowsheetSessionService.SetActiveFlowsheetSessionAsync(input);
        }

        /// <summary>
        /// 删除流程图会话
        /// </summary>
        [HttpPost("DeleteAsync")]
        [CustomAuthorization(RoleType.用户)]
        public override async Task DeleteAsync(IdInput<Guid> input)
        {
            await _flowsheetSessionService.DeleteAsync(input.Id);
        }

        /// <summary>
        /// AI生成PlantUML流程图
        /// </summary>
        [HttpPost("GeneratePlantUMLByAIAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<string> GeneratePlantUMLByAIAsync(GeneratePlantUMLByAIInput input)
        {
            return await _flowsheetSessionService.GeneratePlantUMLByAIAsync(input);
        }
    }
}

