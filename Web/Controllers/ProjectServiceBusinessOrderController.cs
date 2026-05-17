using Web.Extensions;
using Web.Service;
using Web.Tables;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProjectServiceBusinessOrderController : YouJuController<ProjectServiceBusinessOrder, ProjectServiceBusinessOrderDto, ProjectServiceBusinessOrderPagedInput>
    {
        private readonly ProjectServiceBusinessOrderService _projectServiceBusinessOrderService;
        public ProjectServiceBusinessOrderController(IServiceProvider serviceProvider, ProjectServiceBusinessOrderService projectServiceBusinessOrderService) : base(serviceProvider)
        {
            _projectServiceBusinessOrderService = projectServiceBusinessOrderService;
        }

        [HttpPost("ListAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async override Task<PagedReuslt<ProjectServiceBusinessOrderDto>> ListAsync(ProjectServiceBusinessOrderPagedInput input)
        {
            return await _projectServiceBusinessOrderService.ListAsync(input);
        }
        /// <summary>
        /// 用户端获取项目服务业务列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("UserListAsync")]
        [CustomAuthorization(RoleType.用户)]

        public async Task<PagedReuslt<UserProjectServiceBusinessOrderDto>> UserListAsync(ProjectServiceBusinessOrderPagedInput input)
        {
            return await _projectServiceBusinessOrderService.UserListAsync(input);
        }

        [HttpPost("DeleteAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public override async Task DeleteAsync(IdInput<Guid> input)
        {
            var model = await GetAsync(input);

            await base.DeleteAsync(input);
        }

        [HttpPost("CreateOrEditAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public override async Task<ProjectServiceBusinessOrderDto> CreateOrEditAsync(ProjectServiceBusinessOrderDto input)
        {
            return await base.CreateOrEditAsync(input);

        }





    }


}
