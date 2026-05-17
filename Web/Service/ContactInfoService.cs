using Web.Manager;

namespace Web.Service
{
    public class ContactInfoService
    {
        private readonly ContactInfoManager _contactInfoManager;

        public ContactInfoService(ContactInfoManager contactInfoManager)
        {
            _contactInfoManager = contactInfoManager;
        }

        /// <summary>
        /// 获取联系方式列表
        /// </summary>
        public async Task<PagedReuslt<ContactInfoDto>> ListAsync(ContactInfoPagedInput input)
        {
            return await _contactInfoManager.ListAsync(input);
        }

        /// <summary>
        /// 获取有效的联系方式列表（前台使用）
        /// </summary>
        public async Task<List<ContactInfoDto>> GetValidListAsync()
        {
            return await _contactInfoManager.GetValidListAsync();
        }

        /// <summary>
        /// 获取当前时间有效的联系方式列表（前台使用，精确匹配时间段）
        /// </summary>
        public async Task<List<ContactInfoDto>> GetCurrentValidListAsync()
        {
            return await _contactInfoManager.GetCurrentValidListAsync();
        }

        /// <summary>
        /// 获取单个对象
        /// </summary>
        public async Task<ContactInfoDto> GetAsync(Guid id)
        {
            return await _contactInfoManager.GetDtoByIdAsync(id);
        }

        /// <summary>
        /// 创建或编辑联系方式
        /// </summary>
        public async Task<ContactInfoDto> CreateOrEditAsync(ContactInfoDto input)
        {
            return await _contactInfoManager.CreateOrEditAsync(input);
        }

        /// <summary>
        /// 删除联系方式
        /// </summary>
        public async Task DeleteAsync(Guid id)
        {
            await _contactInfoManager.DeleteAsync(id);
        }
    }
}
