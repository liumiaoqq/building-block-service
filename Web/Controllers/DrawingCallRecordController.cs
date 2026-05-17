using Web.Extensions;
using Web.Service;
using Web.Tables;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DrawingCallRecordController : YouJuController<DrawingCallRecord, DrawingCallRecordDto, DrawingCallRecordPagedInput>
    {
        private readonly DrawingCallRecordService _drawingCallRecordService;

        public DrawingCallRecordController(IServiceProvider serviceProvider, DrawingCallRecordService drawingCallRecordService) : base(serviceProvider)
        {
            _drawingCallRecordService = drawingCallRecordService;
        }

        [HttpPost("ListAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async override Task<PagedReuslt<DrawingCallRecordDto>> ListAsync(DrawingCallRecordPagedInput input)
        {
            return await _drawingCallRecordService.ListAsync(input);
        }

        /// <summary>
        /// 获取用户自己的调用记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("UserListAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<PagedReuslt<DrawingCallRecordDto>> UserListAsync(DrawingCallRecordPagedInput input)
        {
            return await _drawingCallRecordService.UserListAsync(input);
        }
    }
}

