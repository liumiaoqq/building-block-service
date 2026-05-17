namespace Web.Manager
{
    public class ContactInfoManager
    {
        protected ISqlSugarClient _sqlSugarClient;

        public ContactInfoManager(ISqlSugarClient sqlSugarClient)
        {
            _sqlSugarClient = sqlSugarClient;
        }

        /// <summary>
        /// 获取联系方式列表
        /// </summary>
        public async Task<PagedReuslt<ContactInfoDto>> ListAsync(ContactInfoPagedInput input)
        {
            RefAsync<int> totalCount = 0;
            var result = await _sqlSugarClient.Queryable<ContactInfo>()
                .Where(x => x.IsDeleted == false)
                .WhereIF(!string.IsNullOrEmpty(input.Title), x => x.Title.Contains(input.Title))
                .WhereIF(!string.IsNullOrEmpty(input.ContactType), x => x.ContactType == input.ContactType)
                .WhereIF(input.IsEnabled != null, x => x.IsEnabled == input.IsEnabled)
                .OrderBy(x => x.Sort)
                .OrderByDescending(x => x.CreationTime)
                .Select(x => new ContactInfoDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    ContactType = x.ContactType,
                    Image = x.Image,
                    ContactNumber = x.ContactNumber,
                    ValidStartTime = x.ValidStartTime,
                    ValidEndTime = x.ValidEndTime,
                    ValidStartDate = x.ValidStartDate,
                    ValidEndDate = x.ValidEndDate,
                    Sort = x.Sort,
                    IsEnabled = x.IsEnabled,
                    CreationTime = x.CreationTime
                })
                .ToPageListAsync(input.Page, input.Size, totalCount);

            return new PagedReuslt<ContactInfoDto>(result, totalCount);
        }

        /// <summary>
        /// 获取有效的联系方式列表（前台使用）
        /// </summary>
        public async Task<List<ContactInfoDto>> GetValidListAsync()
        {
            var now = DateTime.Now;

            var result = await _sqlSugarClient.Queryable<ContactInfo>()
                .Where(x => x.IsDeleted == false && x.IsEnabled == true)
                .Where(x => (x.ValidStartDate == null || x.ValidStartDate <= now))
                .Where(x => (x.ValidEndDate == null || x.ValidEndDate >= now))
                .OrderBy(x => x.Sort)
                .Select(x => new ContactInfoDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    ContactType = x.ContactType,
                    Image = x.Image,
                    ContactNumber = x.ContactNumber,
                    ValidStartTime = x.ValidStartTime,
                    ValidEndTime = x.ValidEndTime,
                    ValidStartDate = x.ValidStartDate,
                    ValidEndDate = x.ValidEndDate,
                    Sort = x.Sort,
                    IsEnabled = x.IsEnabled,
                    CreationTime = x.CreationTime
                })
                .ToListAsync();

            return result;
        }

        /// <summary>
        /// 获取当前时间有效的联系方式列表（精确匹配日期和时间段）
        /// </summary>
        public async Task<List<ContactInfoDto>> GetCurrentValidListAsync()
        {
            var now = DateTime.Now;
            var currentTime = now.ToString("HH:mm");

            var allValid = await _sqlSugarClient.Queryable<ContactInfo>()
                .Where(x => x.IsDeleted == false && x.IsEnabled == true)
                .Where(x => (x.ValidStartDate == null || x.ValidStartDate <= now))
                .Where(x => (x.ValidEndDate == null || x.ValidEndDate >= now))
                .OrderBy(x => x.Sort)
                .Select(x => new ContactInfoDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    ContactType = x.ContactType,
                    Image = x.Image,
                    ContactNumber = x.ContactNumber,
                    ValidStartTime = x.ValidStartTime,
                    ValidEndTime = x.ValidEndTime,
                    ValidStartDate = x.ValidStartDate,
                    ValidEndDate = x.ValidEndDate,
                    Sort = x.Sort,
                    IsEnabled = x.IsEnabled,
                    CreationTime = x.CreationTime
                })
                .ToListAsync();

            // 在内存中过滤时间段
            var result = allValid.Where(x =>
            {
                // 如果没有设置时间段，则全天有效
                if (string.IsNullOrEmpty(x.ValidStartTime) && string.IsNullOrEmpty(x.ValidEndTime))
                    return true;

                // 如果只设置了开始时间
                if (!string.IsNullOrEmpty(x.ValidStartTime) && string.IsNullOrEmpty(x.ValidEndTime))
                    return string.Compare(currentTime, x.ValidStartTime) >= 0;

                // 如果只设置了结束时间
                if (string.IsNullOrEmpty(x.ValidStartTime) && !string.IsNullOrEmpty(x.ValidEndTime))
                    return string.Compare(currentTime, x.ValidEndTime) <= 0;

                // 两个都设置了
                return string.Compare(currentTime, x.ValidStartTime) >= 0 && string.Compare(currentTime, x.ValidEndTime) <= 0;
            }).ToList();

            return result;
        }

        /// <summary>
        /// 根据Id获取实体
        /// </summary>
        public async Task<ContactInfo> GetByIdAsync(Guid id)
        {
            return await _sqlSugarClient.Queryable<ContactInfo>()
                .FirstAsync(x => x.Id == id);
        }

        /// <summary>
        /// 根据Id获取DTO
        /// </summary>
        public async Task<ContactInfoDto> GetDtoByIdAsync(Guid id)
        {
            return await _sqlSugarClient.Queryable<ContactInfo>()
                .Where(x => x.Id == id)
                .Select(x => new ContactInfoDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    ContactType = x.ContactType,
                    Image = x.Image,
                    ContactNumber = x.ContactNumber,
                    ValidStartTime = x.ValidStartTime,
                    ValidEndTime = x.ValidEndTime,
                    ValidStartDate = x.ValidStartDate,
                    ValidEndDate = x.ValidEndDate,
                    Sort = x.Sort,
                    IsEnabled = x.IsEnabled,
                    CreationTime = x.CreationTime
                })
                .FirstAsync();
        }

        /// <summary>
        /// 创建或编辑联系方式
        /// </summary>
        public async Task<ContactInfoDto> CreateOrEditAsync(ContactInfoDto input)
        {
            var entity = await _sqlSugarClient.Queryable<ContactInfo>().FirstAsync(x => x.Id == input.Id);
            if (entity == null)
            {
                entity = new ContactInfo
                {
                    Id = Guid.NewGuid(),
                    Title = input.Title,
                    ContactType = input.ContactType,
                    Image = input.Image,
                    ContactNumber = input.ContactNumber,
                    ValidStartTime = input.ValidStartTime,
                    ValidEndTime = input.ValidEndTime,
                    ValidStartDate = input.ValidStartDate,
                    ValidEndDate = input.ValidEndDate,
                    Sort = input.Sort,
                    IsEnabled = input.IsEnabled,
                    CreationTime = DateTime.Now,
                    IsDeleted = false
                };
                entity = await _sqlSugarClient.Insertable(entity).ExecuteReturnEntityAsync();
            }
            else
            {
                entity.Title = input.Title;
                entity.ContactType = input.ContactType;
                entity.Image = input.Image;
                entity.ContactNumber = input.ContactNumber;
                entity.ValidStartTime = input.ValidStartTime;
                entity.ValidEndTime = input.ValidEndTime;
                entity.ValidStartDate = input.ValidStartDate;
                entity.ValidEndDate = input.ValidEndDate;
                entity.Sort = input.Sort;
                entity.IsEnabled = input.IsEnabled;
                await _sqlSugarClient.Updateable(entity).ExecuteCommandAsync();
            }

            return await GetDtoByIdAsync(entity.Id);
        }

        /// <summary>
        /// 删除联系方式（软删除）
        /// </summary>
        public async Task DeleteAsync(Guid id)
        {
            await _sqlSugarClient.Updateable<ContactInfo>()
                .Where(x => x.Id == id)
                .SetColumns(x => x.IsDeleted == true)
                .ExecuteCommandAsync();
        }
    }
}
