using Web.Manager;

namespace Web.Service
{
    public class IntegralRecordService
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        private readonly ICurrentUser _currentUser;


        private readonly IntegralRecordManager _IntegralRecordManager;

        private readonly IRedisLockService _redisLockService;

        public IntegralRecordService(ISqlSugarClient sqlSugarClient, ICurrentUser currentUser, IntegralRecordManager IntegralRecordManager, IRedisLockService redisLockService)
        {
            _sqlSugarClient = sqlSugarClient;
            _currentUser = currentUser;
            _IntegralRecordManager = IntegralRecordManager;
            _redisLockService = redisLockService;
        }

        public async Task<PagedReuslt<IntegralRecordDto>> ListAsync(IntegralRecordPagedInput input)
        {
            return await _IntegralRecordManager.ListAsync(input);
        }
        /// <summary>
        /// 用户端获取积分记录列表  
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedReuslt<UserIntegralRecordDto>> UserListAsync(IntegralRecordPagedInput input)
        {
            input.UserId = _currentUser.UserId.Value;
            return await _IntegralRecordManager.UserListAsync(input);
        }
        /// <summary>
        /// 得到我当前的积分
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetUserIntegralAsync()
        {
            return await _IntegralRecordManager.GetUserIntegralAsync(_currentUser.UserId.Value);
        }

        ///每日签到
        /// </summary>
        /// <returns></returns>
        public async Task DailySignInAsync()
        {
            var lockSuccess = await _redisLockService.ExecuteWithLockAsync($"dailySignIn_{_currentUser.UserId.Value}", async () =>
            {

                var lastSignInDate = await _IntegralRecordManager.IsDailySignInAsync(_currentUser.UserId.Value);
                if (!lastSignInDate)
                {
                    var integralRecord = new IntegralRecordDto()
                    {
                        UserId = _currentUser.UserId.Value,
                        Type = IntegralRecordType.每日签到,
                        RemainingPoints = await _IntegralRecordManager.GetUserIntegralAsync(_currentUser.UserId.Value),
                        Points = 10,
                        Title = "每日签到",
                    };
                    await _IntegralRecordManager.CreateAsync(integralRecord);
                }
                else
                {
                    throw new YouJuException("今日已签到,感谢您的积极参与");
                }
            });

            if (!lockSuccess)
            {
                throw new YouJuException("业务正在进行中，请勿重复操作");
            }



        }



    }
}
