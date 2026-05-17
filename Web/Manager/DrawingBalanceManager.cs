namespace Web.Manager
{
    public class DrawingBalanceManager
    {
        protected ISqlSugarClient _sqlSugarClient;

        public DrawingBalanceManager(ISqlSugarClient sqlSugarClient)
        {
            _sqlSugarClient = sqlSugarClient;
        }

        public async Task<DrawingBalance> CreateAsync(DrawingBalance drawingBalance)
        {
            var result = await _sqlSugarClient.Insertable(drawingBalance).ExecuteReturnEntityAsync();
            return result;
        }

        public async Task<PagedReuslt<DrawingBalanceDto>> ListAsync(DrawingBalancePagedInput input)
        {
            RefAsync<int> totalCount = 0;
            var result = await _sqlSugarClient.Queryable<DrawingBalance>()
            .LeftJoin<AppUser>((db, u) => db.UserId == u.Id)
            .WhereIF(input.UserId != null, (db, u) => db.UserId == input.UserId)
            .WhereIF(!string.IsNullOrEmpty(input.UserName), (db, u) => u.UserName.Contains(input.UserName))
            .WhereIF(!string.IsNullOrEmpty(input.Email), (db, u) => u.Email.Contains(input.Email))
            .WhereIF(input.DrawingType != null, (db, u) => db.DrawingType == input.DrawingType)
            .WhereIF(input.IsEnabled != null, (db, u) => db.IsEnabled == input.IsEnabled)
            .OrderByDescending((db, u) => db.CreationTime)
            .Select((db, u) => new DrawingBalanceDto
            {
                Id = db.Id,
                UserId = db.UserId,
                UserName = u.UserName,
                Email = u.Email,
                DrawingType = db.DrawingType,
                EffectiveTime = db.EffectiveTime,
                ExpirationTime = db.ExpirationTime,
                MaxDailyGenerations = db.MaxDailyGenerations,
                IsEnabled = db.IsEnabled,
                CreationTime = db.CreationTime,

            })
            .ToPageListAsync(input.Page, input.Size, totalCount);

            return new PagedReuslt<DrawingBalanceDto>(result, totalCount);
        }

        public async Task<DrawingBalance> GetByIdAsync(Guid id)
        {
            return await _sqlSugarClient.Queryable<DrawingBalance>()
                .FirstAsync(x => x.Id == id);
        }

        public async Task UpdateAsync(DrawingBalance drawingBalance)
        {
            await _sqlSugarClient.Updateable(drawingBalance).ExecuteCommandAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await _sqlSugarClient.Deleteable<DrawingBalance>().Where(x => x.Id == id).ExecuteCommandAsync();
        }

        /// <summary>
        /// 获取用户有效的画图余额配置
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="drawingType"></param>
        /// <returns></returns>
        public async Task<DrawingBalance> GetEffectiveBalanceAsync(Guid userId, DrawingType drawingType)
        {
            var now = DateTime.Now;
            return await _sqlSugarClient.Queryable<DrawingBalance>()
                .Where(x => x.UserId == userId)
                .Where(x => x.DrawingType == drawingType)
                .Where(x => x.IsEnabled == true)
                .Where(x => x.EffectiveTime <= now && x.ExpirationTime >= now)
                .OrderByDescending(x => x.CreationTime)
                .FirstAsync();
        }

        /// <summary>
        /// 获取用户所有有效的画图余额配置
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<DrawingBalance>> GetAllEffectiveBalancesAsync(Guid userId)
        {
            var now = DateTime.Now;
            return await _sqlSugarClient.Queryable<DrawingBalance>()
                .Where(x => x.UserId == userId)
                .Where(x => x.IsEnabled == true)
                .Where(x => x.EffectiveTime <= now && x.ExpirationTime >= now)
                .ToListAsync();
        }
    }
}

