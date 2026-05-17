using Web.Extensions;
using Web.Service;
using Web.Tables;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IntegralRecordController : YouJuController<IntegralRecord, IntegralRecordDto, IntegralRecordPagedInput>
    {
        private readonly IntegralRecordService _IntegralRecordService;
        public IntegralRecordController(IServiceProvider serviceProvider, IntegralRecordService IntegralRecordService) : base(serviceProvider)
        {
            _IntegralRecordService = IntegralRecordService;
        }

        [HttpPost("ListAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async override Task<PagedReuslt<IntegralRecordDto>> ListAsync(IntegralRecordPagedInput input)
        {
            return await _IntegralRecordService.ListAsync(input);
        }
        /// <summary>
        /// 用户端获取积分记录列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("UserListAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<PagedReuslt<UserIntegralRecordDto>> UserListAsync(IntegralRecordPagedInput input)
        {
            return await _IntegralRecordService.UserListAsync(input);
        }
        [HttpPost("GetUserIntegralAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<int> GetUserIntegralAsync()
        {
            return await _IntegralRecordService.GetUserIntegralAsync();
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
        public override async Task<IntegralRecordDto> CreateOrEditAsync(IntegralRecordDto input)
        {
            return await base.CreateOrEditAsync(input);

        }
        [HttpPost("DailySignInAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task DailySignInAsync()
        {
            await _IntegralRecordService.DailySignInAsync();
        }




    }


}
