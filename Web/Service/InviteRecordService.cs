using System;
using Web.Manager;
using Web.Tables;

namespace Web.Service
{
    public class InviteRecordService
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        private readonly ICurrentUser _currentUser;

        private readonly InviteRecordManager _InviteRecordManager;



        private readonly IntegralRecordManager _integralRecordManager;

        public InviteRecordService(ISqlSugarClient sqlSugarClient, ICurrentUser currentUser, InviteRecordManager InviteRecordManager, IntegralRecordManager integralRecordManager)
        {
            _sqlSugarClient = sqlSugarClient;
            _currentUser = currentUser;
            _InviteRecordManager = InviteRecordManager;

            _integralRecordManager = integralRecordManager;
        }

        public async Task<PagedReuslt<InviteRecordDto>> ListAsync(InviteRecordPagedInput input)
        {

            return await _InviteRecordManager.ListAsync(input);
        }
        /// <summary>
        /// 用户端获取邀请记录列表  
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedReuslt<UserInviteRecordDto>> UserListAsync(InviteRecordPagedInput input)
        {

            return await _InviteRecordManager.UserListAsync(input);
        }

        /// <summary>
        /// 注册邀请
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task RegisterInviteAsync(string visitCode, Guid userId, Guid inviteUserId)
        {
            //得到邀请人的邀请次数
            var inviteCount = await _sqlSugarClient.Queryable<InviteRecord>()
            .Where(x => x.InviteUserId == inviteUserId)
            .CountAsync();

            int reward = 20;
            //如果邀请次数大于等于5次，就不能再邀请了
            if (inviteCount >= 10)
            {
                reward = 20 + 10 * 2;
            }
            else if (inviteCount >= 5)
            {
                reward = 20 + 10 * 1;
            }


            var inviteRecord = new InviteRecord()
            {
                VisitCode = visitCode,
                UserId = userId,
                InviteUserId = inviteUserId,
                VisitTime = DateTime.Now,
                Reward = reward,

            };
            await _InviteRecordManager.CreateAsync(inviteRecord);


            var publishUser = await _sqlSugarClient.Queryable<AppUser>().Where(x => x.Id == userId).FirstAsync();




            //邀请用户获得积分
            var integralRecordDto = new IntegralRecordDto()
            {
                Title = $"邀请账号{publishUser.UserName}进行注册获得积分",


                UserId = inviteUserId,
                Points = reward,
                RemainingPoints = await _integralRecordManager.GetUserIntegralAsync(inviteUserId),
                SourceOrderNo = publishUser.UserName,
                Type = IntegralRecordType.邀请注册,

            };

            //自己获得积分
            var selfIntegralRecordDto = new IntegralRecordDto()
            {
                Title = $"参与邀请获得积分",
                UserId = userId,
                Points = 10,
                RemainingPoints = await _integralRecordManager.GetUserIntegralAsync(userId),
                SourceOrderNo = publishUser.UserName,
                Type = IntegralRecordType.邀请注册,
            };


            await _integralRecordManager.CreateAsync(integralRecordDto);

        }



        /// <summary>
        /// 获取用户邀请信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Object> GetUserInviteInfoAsync()
        {
            var userId = _currentUser.UserId.Value;
            //求我的邀请成功次数
            var inviteCount = await _sqlSugarClient.Queryable<InviteRecord>()
            .Where(x => x.InviteUserId == userId)
            .CountAsync();
            //求我邀请的积分总和
            var invitePoints = await _sqlSugarClient.Queryable<IntegralRecord>()
            .Where(x => x.Type == IntegralRecordType.邀请注册)
            .Where(x => x.UserId == userId)
            .SumAsync(x => x.Points);

            //得到最近10条邀请记录
            var inviteRecordList = await _sqlSugarClient.Queryable<InviteRecord>()
            .Where(x => x.InviteUserId == userId)
            .OrderByDescending(x => x.CreationTime)
            .Take(10)
            .ToListAsync();
            //得到被我邀请的用户列表
            var inviteUserIds = inviteRecordList.Select(x => x.UserId).ToList();
            var inviteUserList = await _sqlSugarClient.Queryable<AppUser>()
            .Where(x => inviteUserIds.Contains(x.Id))
            .ToListAsync();

            var inviteUserDtoList = inviteRecordList.Select(x => new UserInviteRecordDto()
            {

                UserName = inviteUserList.FirstOrDefault(y => y.Id == x.UserId)?.UserName,
                VisitCode = x.VisitCode,
                Email = inviteUserList.FirstOrDefault(y => y.Id == x.UserId)?.Email,

                VisitTime = x.VisitTime,
            }).ToList();
            return new
            {
                TotalInviteCount = inviteCount,
                TotalInvitePoints = invitePoints,
                InviteUserDtoList = inviteUserDtoList,

            };
        }

    }
}
