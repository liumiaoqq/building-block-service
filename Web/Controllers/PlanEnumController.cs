using Web.Dto.Plans;
using Web.Extensions;
using Web.Manager;
using Web.Service;
using YouJu.Infrastructure.Extensions;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlanEnumController : YouJuController<PlanEnum, PlanEnumDto, PlanEnumPagedInput>
    {

        private readonly PlanEnumService _planEnumService;
        public PlanEnumController(IServiceProvider serviceProvider, PlanEnumService planEnumService) : base(serviceProvider)
        {
            _planEnumService = planEnumService;
        }
        [HttpPost("ListAsync")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public override async Task<PagedReuslt<PlanEnumDto>> ListAsync(PlanEnumPagedInput input)
        {

            var rs = await _planEnumService.ListAsync(input);

            return rs;
        }
        [HttpPost("DeleteAsync")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public override async Task DeleteAsync(IdInput<Guid> input)
        {
            await _planEnumService.DeleteAsync(input);
        }
        [HttpPost("GetAsync")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public override async Task<PlanEnumDto> GetAsync(IdInput<Guid> input)
        {
            return await _planEnumService.GetAsync(input);
        }


        [HttpPost("CreateOrEditAsync")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public async override Task<PlanEnumDto> CreateOrEditAsync(PlanEnumDto input)
        {
            return await _planEnumService.CreateOrEditAsync(input);
        }
        [HttpPost("BatchSaveEnumProps")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public async Task BatchSaveEnumProps(PlanEnumDto input)
        {
            await _planEnumService.BatchSaveEnumProps(input);
        }


    }
}
