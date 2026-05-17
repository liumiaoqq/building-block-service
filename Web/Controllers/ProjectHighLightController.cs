using Web.Extensions;
using Web.Service;
using Web.Tables;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProjectHighLightController : YouJuController<ProjectHighlight, ProjectHighlightDto, ProjectHighlightPagedInput>
    {
        private readonly ProjectHighLightService _projectHighLightService;
        public ProjectHighLightController(IServiceProvider serviceProvider, ProjectHighLightService projectHighLightService) : base(serviceProvider)
        {
            _projectHighLightService = projectHighLightService;
        }

        [HttpPost("ListAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async override Task<PagedReuslt<ProjectHighlightDto>> ListAsync(ProjectHighlightPagedInput input)
        {
            return await _projectHighLightService.ListAsync(input);
        }
        /// <summary>
        /// 用户端获取项目亮点列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("UserListAsync")]
        public async Task<PagedReuslt<UserProjectHighLightDto>> UserListAsync(ProjectHighlightPagedInput input)
        {
            return await _projectHighLightService.UserListAsync(input);
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
        public override async Task<ProjectHighlightDto> CreateOrEditAsync(ProjectHighlightDto input)
        {
            return await base.CreateOrEditAsync(input);

        }





    }


}
