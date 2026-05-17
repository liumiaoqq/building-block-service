using Web.Extensions;
using Web.Service;
using Web.Tables;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InviteRecordController : YouJuController<InviteRecord, InviteRecordDto, InviteRecordPagedInput>
    {
        private readonly InviteRecordService _InviteRecordService;


        public InviteRecordController(IServiceProvider serviceProvider, InviteRecordService InviteRecordService) : base(serviceProvider)
        {
            _InviteRecordService = InviteRecordService;
        }

        [HttpPost("ListAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async override Task<PagedReuslt<InviteRecordDto>> ListAsync(InviteRecordPagedInput input)
        {
            return await _InviteRecordService.ListAsync(input);
        }
        /// <summary>
        /// 用户端获取邀请记录列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("UserListAsync")]
        [CustomAuthorization(RoleType.用户)]


        public async Task<PagedReuslt<UserInviteRecordDto>> UserListAsync(InviteRecordPagedInput input)
        {
            return await _InviteRecordService.UserListAsync(input);
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
        public override async Task<InviteRecordDto> CreateOrEditAsync(InviteRecordDto input)
        {
            return await base.CreateOrEditAsync(input);

        }
        /// <summary>
        /// 获取用户邀请信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("GetUserInviteInfoAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<object> GetUserInviteInfoAsync()
        {

            return await _InviteRecordService.GetUserInviteInfoAsync();
        }




    }


}
