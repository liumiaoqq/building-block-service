using Web.Extensions;
using Web.Service;
using Web.Tables;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CourseSubmitRecordController : YouJuController<CourseSubmitRecord, CourseSubmitRecordDto, CourseSubmitRecordPagedInput>
    {
        private readonly CourseSubmitRecordService _courseSubmitRecordService;
        public CourseSubmitRecordController(IServiceProvider serviceProvider, CourseSubmitRecordService CourseSubmitRecordService) : base(serviceProvider)
        {
            _courseSubmitRecordService = CourseSubmitRecordService;
        }

        [HttpPost("ListAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async override Task<PagedReuslt<CourseSubmitRecordDto>> ListAsync(CourseSubmitRecordPagedInput input)
        {
            return await _courseSubmitRecordService.ListAsync(input);
        }
        [HttpPost("UserListAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<PagedReuslt<UserCourseSubmitRecordDto>> UserListAsync(CourseSubmitRecordPagedInput input)
        {
            return await _courseSubmitRecordService.UserListAsync(input);
        }
        [HttpPost("DeleteAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public override async Task DeleteAsync(IdInput<Guid> input)
        {

            await base.DeleteAsync(input);
        }
        [HttpPost("AuditAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async Task AuditAsync(CourseSubmitRecordDto input)
        {
            await _courseSubmitRecordService.AuditAsync(input);
        }


        [HttpPost("UserUploadAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task UserUploadAsync(CourseSubmitRecordDto input)
        {
            await _courseSubmitRecordService.UserUploadAsync(input);
        }



    }


}
