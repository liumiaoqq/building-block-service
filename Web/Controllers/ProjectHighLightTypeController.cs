using Web.Extensions;
using Web.Service;
using Web.Tables;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProjectHighLightTypeController : YouJuController<ProjectHighlightType, ProjectHighlightTypeDto, ProjectHighlightTypePagedInput>
    {
        private readonly ProjectHighLightTypeService _projectHighLightTypeService;
        public ProjectHighLightTypeController(IServiceProvider serviceProvider, ProjectHighLightTypeService projectHighLightTypeService) : base(serviceProvider)
        {
            _projectHighLightTypeService = projectHighLightTypeService;
        }

        [HttpPost("ListAsync")]
         [CustomAuthorization(RoleType.系统管理员)]
        public async override Task<PagedReuslt<ProjectHighlightTypeDto>> ListAsync(ProjectHighlightTypePagedInput input)
        {
            return await _projectHighLightTypeService.ListAsync(input);
        }
        [HttpPost("UserListAsync")]
        public async  Task<PagedReuslt<ProjectHighlightTypeDto>> UserListAsync(ProjectHighlightTypePagedInput input)
        {
            return await _projectHighLightTypeService.ListAsync(input);
        }
        [HttpPost("DeleteAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public override async Task DeleteAsync(IdInput<Guid> input)
        {

            await base.DeleteAsync(input);
        }




    }


}
