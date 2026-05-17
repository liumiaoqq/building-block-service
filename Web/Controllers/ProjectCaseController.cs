using Web.Dto.Plans;
using Web.Extensions;
using Web.Manager;
using Web.Service;
using YouJu.Infrastructure.Extensions;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProjectCaseController : YouJuController<ProjectCase, ProjectCaseDto, ProjectCasePagedInput>
    {

        private readonly ProjectCaseService _projectCaseService;
        public ProjectCaseController(IServiceProvider serviceProvider, ProjectCaseService projectCaseService) : base(serviceProvider)
        {
            _projectCaseService = projectCaseService;
        }

        [HttpPost("UserListAsync")]

        public async Task<PagedReuslt<UserProjectCaseDto>> UserListAsync(ProjectCasePagedInput input)
        {
            var rs = await _projectCaseService.UserListAsync(input);
            return rs;
        }


        [HttpPost("ListAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public override async Task<PagedReuslt<ProjectCaseDto>> ListAsync(ProjectCasePagedInput input)
        {

            var rs = await _projectCaseService.ListAsync(input);

            return rs;
        }

        [HttpPost("UserGetAsync")]
        public async Task<UserProjectCaseDto> UserGetAsync(ProjectCasePagedInput input)
        {
            return await _projectCaseService.UserGetAsync(input);
        }

        [HttpPost("DeleteAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public override async Task DeleteAsync(IdInput<Guid> input)
        {
            await _projectCaseService.DeleteAsync(input);
        }


        [HttpPost("FullCreateOrEditAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async Task FullCreateOrEditAsync(ProjectCaseDto input)
        {
            await _projectCaseService.CreateOrEditAsync(input);
        }

        /// <summary>
        /// 得到方案类型
        /// </summary>
        /// <returns></returns>

        [HttpPost("GetCaseType")]

        public PagedReuslt<SelectResult> GetCaseType()
        {
            var roles = new List<SelectResult>();

            roles = typeof(ProjectCaseType).GetEnumList().Select(x => new SelectResult() { Name = x.Key, Value = x.Value }).ToList();
            return new PagedReuslt<SelectResult>(roles, roles.Count);

        }

        [HttpPost("UserProjectCaseSubmitAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task UserProjectCaseSubmitAsync(UserProjectCaseSubmitInput input)
        {
            await _projectCaseService.UserProjectCaseSubmitAsync(input);
        }
        /**
        添加浏览次数
        */
        [HttpPost("AddViewCount")]

        public async Task AddViewCount(IdInput<Guid> input)
        {
            await _projectCaseService.AddViewCount(input);
        }
    }
}
