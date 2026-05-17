using Web.Dto.Components;
using Web.Extensions;
using Web.Service;
using Web.Tables;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SystemComponentRuleController : YouJuController<SystemComponentRule, SystemComponentRuleDto, SystemComponentRulePagedInput>
    {

        private readonly SystemComponentRuleService _SystemComponentRuleService;

        public SystemComponentRuleController(IServiceProvider serviceProvider, SystemComponentRuleService SystemComponentRuleService) : base(serviceProvider)
        {
            _SystemComponentRuleService = SystemComponentRuleService;
        }

        [HttpPost("ListAsync")]
        [CustomAuthorization(RoleType.系统管理员)]

        public async override Task<PagedReuslt<SystemComponentRuleDto>> ListAsync(SystemComponentRulePagedInput input)
        {
            return await _SystemComponentRuleService.ListAsync(input);
        }
    



        //[HttpPost("GetAsync")]
        //[CustomAuthorization(RoleType.系统管理员)]
        //public async override Task<SystemComponentRuleDto> GetAsync(IdInput<Guid> input)
        //{
        //    return await _SystemComponentRuleService.GetAsync(input.Id);
        //}

        [HttpPost("DeleteAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public override async Task DeleteAsync(IdInput<Guid> input)
        {
            var model = await GetAsync(input);

            await base.DeleteAsync(input);
        }

     


    }


}
