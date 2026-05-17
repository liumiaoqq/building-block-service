namespace Web.Manager
{
    public class DrawingCallRecordManager
    {
        protected ISqlSugarClient _sqlSugarClient;

        public DrawingCallRecordManager(ISqlSugarClient sqlSugarClient)
        {
            _sqlSugarClient = sqlSugarClient;
        }

        public async Task<DrawingCallRecord> CreateAsync(DrawingCallRecord drawingCallRecord)
        {
            var result = await _sqlSugarClient.Insertable(drawingCallRecord).ExecuteReturnEntityAsync();
            return result;
        }

        public async Task<PagedReuslt<DrawingCallRecordDto>> ListAsync(DrawingCallRecordPagedInput input)
        {
            RefAsync<int> totalCount = 0;
            var result = await _sqlSugarClient.Queryable<DrawingCallRecord>()
            .LeftJoin<AppUser>((dcr, u) => dcr.UserId == u.Id)
            .WhereIF(input.UserId != null, (dcr, u) => dcr.UserId == input.UserId)
            .WhereIF(!string.IsNullOrEmpty(input.UserName), (dcr, u) => u.UserName.Contains(input.UserName))
            .WhereIF(!string.IsNullOrEmpty(input.Email), (dcr, u) => u.Email.Contains(input.Email))
            .WhereIF(input.DrawingType != null, (dcr, u) => dcr.DrawingType == input.DrawingType)
            .WhereIF(input.IsSuccess != null, (dcr, u) => dcr.IsSuccess == input.IsSuccess)
            .WhereIF(input.StartTime != null, (dcr, u) => dcr.CallTime >= input.StartTime)
            .WhereIF(input.EndTime != null, (dcr, u) => dcr.CallTime <= input.EndTime)
            .OrderByDescending((dcr, u) => dcr.CallTime)
            .Select((dcr, u) => new DrawingCallRecordDto
            {
                Id = dcr.Id,
                UserId = dcr.UserId,
                UserName = u.UserName,
                Email = u.Email,
                DrawingType = dcr.DrawingType,
                CallTime = dcr.CallTime,
                IsSuccess = dcr.IsSuccess,
                FailReason = dcr.FailReason,
                CreationTime = dcr.CreationTime,
            })
            .ToPageListAsync(input.Page, input.Size, totalCount);

            return new PagedReuslt<DrawingCallRecordDto>(result, totalCount);
        }

        public async Task<DrawingCallRecord> GetByIdAsync(Guid id)
        {
            return await _sqlSugarClient.Queryable<DrawingCallRecord>()
                .FirstAsync(x => x.Id == id);
        }

        /// <summary>
        /// 获取用户今日指定类型的调用成功次数
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="drawingType"></param>
        /// <returns></returns>
        public async Task<int> GetTodaySuccessCountAsync(Guid userId, DrawingType drawingType)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            return await _sqlSugarClient.Queryable<DrawingCallRecord>()
                .Where(x => x.UserId == userId)
                .Where(x => x.DrawingType == drawingType)
                .Where(x => x.IsSuccess == true)
                .Where(x => x.CallTime >= today && x.CallTime < tomorrow)
                .CountAsync();
        }

        /// <summary>
        /// 获取用户今日所有类型的调用成功次数
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Dictionary<DrawingType, int>> GetTodayAllSuccessCountAsync(Guid userId)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var records = await _sqlSugarClient.Queryable<DrawingCallRecord>()
                .Where(x => x.UserId == userId)
                .Where(x => x.IsSuccess == true)
                .Where(x => x.CallTime >= today && x.CallTime < tomorrow)
                .GroupBy(x => x.DrawingType)
                .Select(x => new
                {
                    DrawingType = x.DrawingType,
                    Count = SqlFunc.AggregateCount(x.Id)
                })
                .ToListAsync();

            return records.ToDictionary(x => x.DrawingType, x => x.Count);
        }
    }
}

