using Web.Extensions;
using Web.Service;
using Web.Tables;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProjectServiceBusinessController : YouJuController<ProjectServiceBusiness, ProjectServiceBusinessDto, ProjectServiceBusinessPagedInput>
    {
        private readonly ProjectServiceBusinessService _projectServiceBusinessService;
        public ProjectServiceBusinessController(IServiceProvider serviceProvider, ProjectServiceBusinessService projectServiceBusinessService) : base(serviceProvider)
        {
            _projectServiceBusinessService = projectServiceBusinessService;
        }

        [HttpPost("ListAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async override Task<PagedReuslt<ProjectServiceBusinessDto>> ListAsync(ProjectServiceBusinessPagedInput input)
        {
            return await _projectServiceBusinessService.ListAsync(input);   
        }
        /// <summary>
        /// 用户端获取项目服务业务列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("UserListAsync")]

        public async Task<PagedReuslt<UserProjectServiceBusinessDto>> UserListAsync(ProjectServiceBusinessPagedInput input)
        {
            return await _projectServiceBusinessService.UserListAsync(input);
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
        public override async Task<ProjectServiceBusinessDto> CreateOrEditAsync(ProjectServiceBusinessDto input)
        {
            return await base.CreateOrEditAsync(input);

        }
        /// <summary>
        /// 用户端获取项目服务业务详情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("UserDetailAsync")]
        public async Task<UserProjectServiceBusinessDetailDto> UserDetailAsync(IdInput<Guid> input)
        {
            return await _projectServiceBusinessService.UserDetailAsync(input.Id);
        }






    }


}
