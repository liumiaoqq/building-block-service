using Web.Extensions;
using Web.Service;
using Web.Tables;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContactInfoController : ControllerBase
    {
        private readonly ContactInfoService _contactInfoService;

        public ContactInfoController(ContactInfoService contactInfoService)
        {
            _contactInfoService = contactInfoService;
        }

        /// <summary>
        /// 获取联系方式列表（管理端）
        /// </summary>
        [HttpPost("ListAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async Task<PagedReuslt<ContactInfoDto>> ListAsync(ContactInfoPagedInput input)
        {
            return await _contactInfoService.ListAsync(input);
        }

        /// <summary>
        /// 获取有效的联系方式列表（前台使用，根据有效日期、有效时间段、是否启用筛选）
        /// </summary>
        [HttpPost("GetValidListAsync")]
        public async Task<List<ContactInfoDto>> GetValidListAsync()
        {
            return await _contactInfoService.GetValidListAsync();
        }

        /// <summary>
        /// 获取当前时间有效的联系方式列表（前台使用，精确匹配时间段）
        /// </summary>
        [HttpPost("GetCurrentValidListAsync")]
        public async Task<List<ContactInfoDto>> GetCurrentValidListAsync()
        {
            return await _contactInfoService.GetCurrentValidListAsync();
        }

        /// <summary>
        /// 获取单个对象
        /// </summary>
        [HttpPost("GetAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async Task<ContactInfoDto> GetAsync(IdInput<Guid> input)
        {
            return await _contactInfoService.GetAsync(input.Id);
        }

        /// <summary>
        /// 创建或编辑联系方式
        /// </summary>
        [HttpPost("CreateOrEditAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async Task<ContactInfoDto> CreateOrEditAsync(ContactInfoDto input)
        {
            return await _contactInfoService.CreateOrEditAsync(input);
        }

        /// <summary>
        /// 删除联系方式
        /// </summary>
        [HttpPost("DeleteAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async Task DeleteAsync(IdInput<Guid> input)
        {
            await _contactInfoService.DeleteAsync(input.Id);
        }
    }
}
