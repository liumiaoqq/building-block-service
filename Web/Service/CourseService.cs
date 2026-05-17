using Web.Manager;

namespace Web.Service
{
    public class CourseService
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        private readonly ICurrentUser _currentUser;

        private readonly CourseManager _courseManager;

        private readonly IntegralRecordManager _integralRecordManager;


        public CourseService(ISqlSugarClient sqlSugarClient, ICurrentUser currentUser, CourseManager CourseManager, IntegralRecordManager integralRecordManager)
        {
            _sqlSugarClient = sqlSugarClient;
            _currentUser = currentUser;
            _courseManager = CourseManager;
            _integralRecordManager = integralRecordManager;
        }

        public async Task<PagedReuslt<CourseDto>> ListAsync(CoursePagedInput input)
        {
            return await _courseManager.ListAsync(input);
        }
        /// <summary>
        /// 用户端获取课程列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedReuslt<UserCourseDto>> UserListAsync(CoursePagedInput input)
        {
            var userId = _currentUser.UserId;

            //查询用户所有的提交记录除了作废外的
            var userCourseSubmitRecords = await _sqlSugarClient.Queryable<CourseSubmitRecord>()
            .Where(x => x.UserId == userId)
            .Where(x => x.AuditStatus != CourseAuditStatus.作废)
            .ToListAsync();
            var courseIds = new List<Guid>();
            if (input.AuditStatus != null)
            {
                if (input.AuditStatus != CourseAuditStatus.待提交)
                {
                    courseIds = userCourseSubmitRecords.Where(x => x.AuditStatus == input.AuditStatus).Select(x => x.CourseId).ToList();
                }
                else
                {
                    //得到所有课程的ids
                    var allCourseIds = await _sqlSugarClient.Queryable<Course>()
                    .Select(x => x.Id)
                    .ToListAsync();
                    //待提交则是没有审核通过和没有提交过的
                    var submitCourseIds = userCourseSubmitRecords.Where(x => x.AuditStatus == CourseAuditStatus.审核通过 || x.AuditStatus == CourseAuditStatus.审核中).Select(x => x.CourseId).ToList();
                    courseIds = allCourseIds.Where(x => !submitCourseIds.Contains(x)).ToList();

                }
            }



            var result = await _sqlSugarClient.Queryable<Course>()
            .WhereIF(input.Title.IsNotNullOrNotWhiteSpace(), x => x.Title.Contains(input.Title))
            .WhereIF(input.CourseTypeId != null, x => x.CourseTypeId == input.CourseTypeId)
            .WhereIF(input.AuditStatus != null, x => courseIds.Contains(x.Id))
            .OrderByDescending(x => x.Sort)
             .Select<UserCourseDto>()
            .ToPageListAsync(input.Page, input.Size);

            //得到课程里面课程类型的ids
            var courseTypeIds = result.Select(x => x.CourseTypeId).ToList();

            var courseTypeNames = await _sqlSugarClient.Queryable<CourseType>()
            .Where(x => courseTypeIds.Contains(x.Id))
            .ToListAsync();

            foreach (var item in result)
            {
                item.CourseTypeName = courseTypeNames.FirstOrDefault(x => x.Id == item.CourseTypeId)?.Name;
                item.AuditStatus = userCourseSubmitRecords.FirstOrDefault(x => x.CourseId == item.Id)?.AuditStatus ?? CourseAuditStatus.待提交;
            }



            return new PagedReuslt<UserCourseDto>(result, result.Count);
        }


        /// <summary>
        /// 获取用户课程总结
        /// </summary>
        /// <returns></returns>
        public async Task<UserCourseSummaryDto> UserCourseSummaryAsync()
        {

            var userId = _currentUser.UserId;



            var totalCourseCount = await _sqlSugarClient.Queryable<Course>()
            .CountAsync();

            //根据这些课程总共可以获取的积分数量
            var totalPoints = await _sqlSugarClient.Queryable<Course>()
            .SumAsync(x => x.Points);


            //查询用户所有的课程提交数据
            var userCourseSubmitRecords = await _sqlSugarClient.Queryable<CourseSubmitRecord>()
            .Where(x => x.UserId == userId)
            .ToListAsync();


            //审核通过课程去重数量
            var passCourseCount = userCourseSubmitRecords.Where(x => x.AuditStatus == CourseAuditStatus.审核通过).GroupBy(x => x.CourseId).Count();

            //审核不通过课程去重数量
            var failCourseCount = userCourseSubmitRecords.Where(x => x.AuditStatus == CourseAuditStatus.审核拒绝).GroupBy(x => x.CourseId).Count();

            //审核中数量
            var pendingCourseCount = userCourseSubmitRecords.Where(x => x.AuditStatus == CourseAuditStatus.审核中).GroupBy(x => x.CourseId).Count();


            //剩余上传数量等于总课程数-审核通过数-审核中的数量 
            var remainingUploadCount = totalCourseCount - passCourseCount - pendingCourseCount;




            //得到课程获取的累计积分总数
            var totalGetPoints = await _sqlSugarClient.Queryable<IntegralRecord>()
            .Where(x => x.UserId == userId)
            .Where(x => x.Type == IntegralRecordType.视频打卡)
            .SumAsync(x => x.Points);

            //今天是否完成签到
            var isDailySignIn = await _integralRecordManager.IsDailySignInAsync(userId.Value);


            return new UserCourseSummaryDto
            {
                TotalPoints = totalPoints,
                TotalGetPoints = totalGetPoints,
                TotalPassCount = passCourseCount,
                TotalFailCount = failCourseCount,
                TotalPendingCount = pendingCourseCount,
                RemainingUploadCount = remainingUploadCount,
                IsDailySignIn = isDailySignIn,
            };
        }




    }
}
