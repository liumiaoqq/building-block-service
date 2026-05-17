using Web.Manager;
using System.Collections.Concurrent;
using System.Threading;
using Web.Extensions;

namespace Web.Service
{
    public class CourseSubmitRecordService
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        private readonly ICurrentUser _currentUser;

        private readonly CourseSubmitRecordManager _courseSubmitRecordManager;

        private readonly IntegralRecordManager _integralRecordManager;

        private readonly IRedisLockService _redisLockService;

        public CourseSubmitRecordService(ISqlSugarClient sqlSugarClient, ICurrentUser currentUser, CourseSubmitRecordManager CourseSubmitRecordManager, IntegralRecordManager integralRecordManager, IRedisLockService redisLockService)
        {
            _sqlSugarClient = sqlSugarClient;
            _currentUser = currentUser;
            _courseSubmitRecordManager = CourseSubmitRecordManager;
            _integralRecordManager = integralRecordManager;
            _redisLockService = redisLockService;
        }

        public async Task<PagedReuslt<CourseSubmitRecordDto>> ListAsync(CourseSubmitRecordPagedInput input)
        {
            return await _courseSubmitRecordManager.ListAsync(input);
        }
        /// <summary>
        /// 用户端获取课程提交记录列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedReuslt<UserCourseSubmitRecordDto>> UserListAsync(CourseSubmitRecordPagedInput input)
        {
            input.UserId = _currentUser.UserId.Value;
            return await _courseSubmitRecordManager.UserListAsync(input);
        }

        /// <summary>
        /// 用户上传
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task UserUploadAsync(CourseSubmitRecordDto input)
        {
            input.UserId = _currentUser.UserId.Value;
            if (input.CourseId == Guid.Empty)
            {
                throw new YouJuException("课程不能为空");
            }
            if (input.UploadImages == null)
            {
                throw new YouJuException("图片不能为空");
            }

            // 尝试获取锁执行上传操作
            var lockSuccess = await _redisLockService.ExecuteWithLockAsync($"courseSubmitRecord_{input.UserId}", async () =>
            {
                await _courseSubmitRecordManager.UserUploadAsync(input);
            });

            // 如果没有获取到锁，提示用户业务正在进行中
            if (!lockSuccess)
            {
                throw new YouJuException("业务正在进行中，请勿重复操作");
            }
        }
        /**
        管理员进行审核
        */
        public async Task AuditAsync(CourseSubmitRecordDto input)
        {
            //修改数据到数据库
            var courseSubmitRecord = await _sqlSugarClient.Queryable<CourseSubmitRecord>()
            .Where(x => x.Id == input.Id)
            .FirstAsync();



            courseSubmitRecord.AuditStatus = input.AuditStatus;
            courseSubmitRecord.Remark = input.Remark;
            courseSubmitRecord.AuditTime = DateTime.Now;
            await _sqlSugarClient.Updateable(courseSubmitRecord).ExecuteCommandAsync();

            if (input.AuditStatus == CourseAuditStatus.审核通过)
            {
                //查询对应课程
                var course = await _sqlSugarClient.Queryable<Course>()
                .Where(x => x.Id == courseSubmitRecord.CourseId)
                .FirstAsync();
                var integralRecord = new IntegralRecord()
                {
                    UserId = courseSubmitRecord.UserId,
                    Points = courseSubmitRecord.Points,
                    Type = IntegralRecordType.视频打卡,
                    Title = "你完成了视频[" + course.Title + "]三连击+关注",
                    SourceOrderNo = courseSubmitRecord.Id.ToString(),
                    RemainingPoints = await _integralRecordManager.GetUserIntegralAsync(courseSubmitRecord.UserId),
                };
                await _sqlSugarClient.Insertable(integralRecord).ExecuteCommandAsync();
            }

        }

    }
}
