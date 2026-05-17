using System;
using System.Text;
using System.Threading.Tasks;

namespace Web.Manager
{
    public class IntegralRecordManager
    {
        protected ISqlSugarClient _sqlSugarClient;
        protected ICurrentUser CurrentUser;


        public IntegralRecordManager(ISqlSugarClient sqlSugarClient, ICurrentUser currentUser)
        {
            _sqlSugarClient = sqlSugarClient;
            CurrentUser = currentUser;
        }


        public async Task<PagedReuslt<IntegralRecordDto>> ListAsync(IntegralRecordPagedInput input)
        {
            var result = await _sqlSugarClient.Queryable<IntegralRecord>()
            .WhereIF(input.UserId.HasValue, x => x.UserId == input.UserId)
            .WhereIF(input.SourceOrderNo.IsNotNullOrNotWhiteSpace(), x => x.SourceOrderNo.Contains(input.SourceOrderNo))
            .WhereIF(input.Type.HasValue, x => x.Type == input.Type)
            .OrderByDescending(x => x.CreationTime)
            .Select<IntegralRecordDto>()
            .ToPageListAsync(input.Page, input.Size);
            return new PagedReuslt<IntegralRecordDto>(result, result.Count);
        }
        public async Task<PagedReuslt<UserIntegralRecordDto>> UserListAsync(IntegralRecordPagedInput input)
        {

            Guid userId = CurrentUser.UserId.Value;

            var result = await _sqlSugarClient.Queryable<IntegralRecord>()
           .Where(x => x.UserId == userId)
           .WhereIF(input.SourceOrderNo.IsNotNullOrNotWhiteSpace(), x => x.SourceOrderNo.Contains(input.SourceOrderNo))
           .WhereIF(input.Type.HasValue, x => x.Type == input.Type)

           .OrderByDescending(x => x.CreationTime)
           .Select<UserIntegralRecordDto>()
           .ToPageListAsync(input.Page, input.Size);
            return new PagedReuslt<UserIntegralRecordDto>(result, result.Count);
        }

        /// <summary>
        /// 获取用户积分
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<int> GetUserIntegralAsync(Guid userId)
        {

            var result = await _sqlSugarClient.Queryable<IntegralRecord>()
            .Where(x => x.UserId == userId)
            .SumAsync(x => x.Points);
            return result;
        }

        /// <summary>
        /// 创建积分记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreateAsync(IntegralRecordDto input)
        {
            var userId = CurrentUser.GetUserId();
            var integralRecord = new IntegralRecord()
            {
                Title = input.Title,
                UserId = userId,
                Points = input.Points,
                Type = input.Type,
                Remark = input.Remark,
                SourceOrderNo = input.SourceOrderNo,
                RemainingPoints = input.RemainingPoints,
            };
            await _sqlSugarClient.Insertable(integralRecord).ExecuteCommandAsync();
        }

        /// <summary>
        /// 判断今日是否签到
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsDailySignInAsync(Guid userId)
        {
            var now = DateTime.Now.Date;
            var lastSignInDate = await _sqlSugarClient.Queryable<IntegralRecord>()
               .Where(x => x.UserId == userId)
               .Where(x => x.Type == IntegralRecordType.每日签到)
               .Where(x => x.CreationTime >= now)
               .OrderByDescending(x => x.CreationTime)
               .CountAsync();
            return lastSignInDate > 0;
        }
    }
}
