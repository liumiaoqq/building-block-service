using Web.Extensions;
using Web.Service;
using Web.Tables;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SequenceDiagramSessionController : YouJuController<SequenceDiagramSession, SequenceDiagramSessionDto, GetSequenceDiagramSessionPagedInput>
    {
        private readonly SequenceDiagramSessionService _sequenceDiagramSessionService;

        public SequenceDiagramSessionController(
            IServiceProvider serviceProvider,
            SequenceDiagramSessionService sequenceDiagramSessionService) : base(serviceProvider)
        {
            _sequenceDiagramSessionService = sequenceDiagramSessionService;
        }

        /// <summary>
        /// 获取时序图会话分页列表
        /// </summary>
        [HttpPost("ListAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async override Task<PagedReuslt<SequenceDiagramSessionDto>> ListAsync(GetSequenceDiagramSessionPagedInput input)
        {
            return await _sequenceDiagramSessionService.ListAsync(input);
        }

        /// <summary>
        /// 根据ID获取时序图会话
        /// </summary>
        [HttpPost("GetByIdAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<SequenceDiagramSessionDto> GetByIdAsync(IdInput<Guid> input)
        {
            return await _sequenceDiagramSessionService.GetByIdAsync(input.Id);
        }

        /// <summary>
        /// 获取当前激活的时序图会话
        /// </summary>
        [HttpPost("GetActiveSequenceDiagramSessionAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<SequenceDiagramSessionDto> GetActiveSequenceDiagramSessionAsync(GetOrCreateActiveSequenceDiagramSessionInput input)
        {
            return await _sequenceDiagramSessionService.GetActiveSequenceDiagramSessionAsync();
        }

        /// <summary>
        /// 获取或创建激活的时序图会话
        /// </summary>
        [HttpPost("GetOrCreateActiveSequenceDiagramSessionAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<SequenceDiagramSessionDto> GetOrCreateActiveSequenceDiagramSessionAsync(GetOrCreateActiveSequenceDiagramSessionInput input)
        {
            return await _sequenceDiagramSessionService.GetOrCreateActiveSequenceDiagramSessionAsync(input);
        }

        /// <summary>
        /// 创建时序图会话
        /// </summary>
        [HttpPost("CreateAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<Guid> CreateAsync(CreateSequenceDiagramSessionInput input)
        {
            return await _sequenceDiagramSessionService.CreateAsync(input);
        }

        /// <summary>
        /// 更新时序图会话
        /// </summary>
        [HttpPost("UpdateAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task UpdateAsync(UpdateSequenceDiagramSessionInput input)
        {
            await _sequenceDiagramSessionService.UpdateAsync(input);
        }

        /// <summary>
        /// 设置激活的时序图会话
        /// </summary>
        [HttpPost("SetActiveSequenceDiagramSessionAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task SetActiveSequenceDiagramSessionAsync(SetActiveSequenceDiagramSessionInput input)
        {
            await _sequenceDiagramSessionService.SetActiveSequenceDiagramSessionAsync(input);
        }

        /// <summary>
        /// 删除时序图会话
        /// </summary>
        [HttpPost("DeleteAsync")]
        [CustomAuthorization(RoleType.用户)]
        public override async Task DeleteAsync(IdInput<Guid> input)
        {
            await _sequenceDiagramSessionService.DeleteAsync(input.Id);
        }

        /// <summary>
        /// AI生成PlantUML时序图
        /// </summary>
        [HttpPost("GenerateSequenceDiagramByAIAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<string> GenerateSequenceDiagramByAIAsync(GenerateSequenceDiagramByAIInput input)
        {
            return await _sequenceDiagramSessionService.GenerateSequenceDiagramByAIAsync(input);
        }
    }
}

