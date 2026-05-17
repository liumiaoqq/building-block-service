using Web.Extensions;
using Web.Service;
using Web.Tables;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CourseTypeController : YouJuController<CourseType, CourseTypeDto, CourseTypePagedInput>
    {
        private readonly CourseTypeService _CourseTypeService;
        public CourseTypeController(IServiceProvider serviceProvider, CourseTypeService CourseTypeService) : base(serviceProvider)
        {
            _CourseTypeService = CourseTypeService;
        }

        [HttpPost("ListAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async override Task<PagedReuslt<CourseTypeDto>> ListAsync(CourseTypePagedInput input)
        {
            return await _CourseTypeService.ListAsync(input);
        }
        [HttpPost("UserListAsync")]
        public async Task<PagedReuslt<CourseTypeDto>> UserListAsync(CourseTypePagedInput input)
        {
            return await _CourseTypeService.ListAsync(input);
        }
        [HttpPost("DeleteAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public override async Task DeleteAsync(IdInput<Guid> input)
        {

            await base.DeleteAsync(input);
        }




    }


}
