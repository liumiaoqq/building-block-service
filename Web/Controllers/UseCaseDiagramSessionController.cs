using Web.Extensions;
using Web.Service;
using Web.Tables;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UseCaseDiagramSessionController : YouJuController<UseCaseDiagramSession, UseCaseDiagramSessionDto, GetUseCaseDiagramSessionPagedInput>
    {
        private readonly UseCaseDiagramSessionService _useCaseDiagramSessionService;

        public UseCaseDiagramSessionController(
            IServiceProvider serviceProvider,
            UseCaseDiagramSessionService useCaseDiagramSessionService) : base(serviceProvider)
        {
            _useCaseDiagramSessionService = useCaseDiagramSessionService;
        }

        /// <summary>
        /// 获取用例图会话分页列表
        /// </summary>
        [HttpPost("ListAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async override Task<PagedReuslt<UseCaseDiagramSessionDto>> ListAsync(GetUseCaseDiagramSessionPagedInput input)
        {
            return await _useCaseDiagramSessionService.ListAsync(input);
        }

        /// <summary>
        /// 根据ID获取用例图会话
        /// </summary>
        [HttpPost("GetByIdAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<UseCaseDiagramSessionDto> GetByIdAsync(IdInput<Guid> input)
        {
            return await _useCaseDiagramSessionService.GetByIdAsync(input.Id);
        }

        /// <summary>
        /// 获取当前激活的用例图会话
        /// </summary>
        [HttpPost("GetActiveUseCaseDiagramSessionAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<UseCaseDiagramSessionDto> GetActiveUseCaseDiagramSessionAsync()
        {
            return await _useCaseDiagramSessionService.GetActiveUseCaseDiagramSessionAsync();
        }

        /// <summary>
        /// 获取或创建激活的用例图会话
        /// </summary>
        [HttpPost("GetOrCreateActiveUseCaseDiagramSessionAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<UseCaseDiagramSessionDto> GetOrCreateActiveUseCaseDiagramSessionAsync(GetOrCreateActiveUseCaseDiagramSessionInput input)
        {
            return await _useCaseDiagramSessionService.GetOrCreateActiveUseCaseDiagramSessionAsync(input);
        }

        /// <summary>
        /// 创建用例图会话
        /// </summary>
        [HttpPost("CreateAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<Guid> CreateAsync(CreateUseCaseDiagramSessionInput input)
        {
            return await _useCaseDiagramSessionService.CreateAsync(input);
        }

        /// <summary>
        /// 更新用例图会话
        /// </summary>
        [HttpPost("UpdateAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task UpdateAsync(UpdateUseCaseDiagramSessionInput input)
        {
            await _useCaseDiagramSessionService.UpdateAsync(input);
        }

        /// <summary>
        /// 设置激活的用例图会话
        /// </summary>
        [HttpPost("SetActiveUseCaseDiagramSessionAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task SetActiveUseCaseDiagramSessionAsync(SetActiveUseCaseDiagramSessionInput input)
        {
            await _useCaseDiagramSessionService.SetActiveUseCaseDiagramSessionAsync(input);
        }

        /// <summary>
        /// 删除用例图会话
        /// </summary>
        [HttpPost("DeleteAsync")]
        [CustomAuthorization(RoleType.用户)]
        public override async Task DeleteAsync(IdInput<Guid> input)
        {
            await _useCaseDiagramSessionService.DeleteAsync(input.Id);
        }

        /// <summary>
        /// AI生成用例图相关数据
        /// </summary>
        [HttpPost("GenerateUseCaseDiagramByAIAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<UseCaseDiagramSessionDto> GenerateUseCaseDiagramByAIAsync(GenerateUseCaseDiagramByAIInput input)
        {
            return await _useCaseDiagramSessionService.GenerateUseCaseDiagramByAIAsync(input);
        }
    }
}
