using Web.Extensions;
using Web.Service;
using Web.Tables;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CourseController : YouJuController<Course, CourseDto, CoursePagedInput>
    {
        private readonly CourseService _courseService;
        public CourseController(IServiceProvider serviceProvider, CourseService CourseService) : base(serviceProvider)
        {
            _courseService = CourseService;
        }

        [HttpPost("ListAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async override Task<PagedReuslt<CourseDto>> ListAsync(CoursePagedInput input)
        {
            return await _courseService.ListAsync(input);
        }
        [HttpPost("UserListAsync")]
        [CustomAuthorization(RoleType.用户)]

        public async Task<PagedReuslt<UserCourseDto>> UserListAsync(CoursePagedInput input)
        {
            return await _courseService.UserListAsync(input);
        }
        [HttpPost("UserCourseSummaryAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<UserCourseSummaryDto> UserCourseSummaryAsync()
        {
            return await _courseService.UserCourseSummaryAsync();
        }
        [HttpPost("DeleteAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public override async Task DeleteAsync(IdInput<Guid> input)
        {

            await base.DeleteAsync(input);
        }




    }


}
