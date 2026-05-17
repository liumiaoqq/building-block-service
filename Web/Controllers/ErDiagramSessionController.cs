using Web.Extensions;
using Web.Service;
using Web.Tables;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ErDiagramSessionController : YouJuController<ErDiagramSession, ErDiagramSessionDto, GetErDiagramSessionPagedInput>
    {
        private readonly ErDiagramSessionService _erDiagramSessionService;

        public ErDiagramSessionController(
            IServiceProvider serviceProvider,
            ErDiagramSessionService erDiagramSessionService) : base(serviceProvider)
        {
            _erDiagramSessionService = erDiagramSessionService;
        }

        /// <summary>
        /// 获取ER图会话分页列表
        /// </summary>
        [HttpPost("ListAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async override Task<PagedReuslt<ErDiagramSessionDto>> ListAsync(GetErDiagramSessionPagedInput input)
        {
            return await _erDiagramSessionService.ListAsync(input);
        }

        /// <summary>
        /// 根据ID获取ER图会话
        /// </summary>
        [HttpPost("GetByIdAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<ErDiagramSessionDto> GetByIdAsync(IdInput<Guid> input)
        {
            return await _erDiagramSessionService.GetByIdAsync(input.Id);
        }

        /// <summary>
        /// 获取当前激活的ER图会话
        /// </summary>
        [HttpPost("GetActiveErDiagramSessionAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<ErDiagramSessionDto> GetActiveErDiagramSessionAsync(GetOrCreateActiveErDiagramSessionInput input)
        {
            return await _erDiagramSessionService.GetActiveErDiagramSessionAsync(input.CodeManagementId);
        }

        /// <summary>
        /// 获取或创建激活的ER图会话
        /// </summary>
        [HttpPost("GetOrCreateActiveErDiagramSessionAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<ErDiagramSessionDto> GetOrCreateActiveErDiagramSessionAsync(GetOrCreateActiveErDiagramSessionInput input)
        {
            return await _erDiagramSessionService.GetOrCreateActiveErDiagramSessionAsync(input);
        }

        /// <summary>
        /// 创建ER图会话
        /// </summary>
        [HttpPost("CreateAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<Guid> CreateAsync(CreateErDiagramSessionInput input)
        {
            return await _erDiagramSessionService.CreateAsync(input);
        }

        /// <summary>
        /// 更新ER图会话
        /// </summary>
        [HttpPost("UpdateAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task UpdateAsync(UpdateErDiagramSessionInput input)
        {
            await _erDiagramSessionService.UpdateAsync(input);
        }

        /// <summary>
        /// 设置激活的ER图会话
        /// </summary>
        [HttpPost("SetActiveErDiagramSessionAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task SetActiveErDiagramSessionAsync(SetActiveErDiagramSessionInput input)
        {
            await _erDiagramSessionService.SetActiveErDiagramSessionAsync(input);
        }

        /// <summary>
        /// 删除ER图会话
        /// </summary>
        [HttpPost("DeleteAsync")]
        [CustomAuthorization(RoleType.用户)]
        public override async Task DeleteAsync(IdInput<Guid> input)
        {
            await _erDiagramSessionService.DeleteAsync(input.Id);
        }

        /// <summary>
        /// AI生成ER图相关数据
        /// </summary>
        [HttpPost("GenerateErDiagramByAIAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<ErDiagramSessionDto> GenerateErDiagramByAIAsync(GenerateErDiagramByAIInput input)
        {
            return await _erDiagramSessionService.GenerateErDiagramByAIAsync(input);
        }
    }
}
