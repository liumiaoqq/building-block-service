using System.Text;

namespace Web.Manager
{
    public class CourseSubmitRecordManager
    {
        protected ISqlSugarClient _sqlSugarClient;

        public CourseSubmitRecordManager(ISqlSugarClient sqlSugarClient)
        {
            _sqlSugarClient = sqlSugarClient;
        }


        public async Task<CourseSubmitRecord> CreateAsync(CourseSubmitRecord CourseSubmitRecord)
        {
            var result = await _sqlSugarClient.Insertable(CourseSubmitRecord).ExecuteReturnEntityAsync();
            return result;
        }


        public async Task<PagedReuslt<CourseSubmitRecordDto>> ListAsync(CourseSubmitRecordPagedInput input)
        {
            var result = await _sqlSugarClient.Queryable<CourseSubmitRecord>()
            .WhereIF(input.CourseId != null, x => x.CourseId == input.CourseId)
            .WhereIF(input.UserId != null, x => x.UserId == input.UserId)
            .WhereIF(input.AuditStatus != null, x => x.AuditStatus == input.AuditStatus)
             .OrderByDescending(x => x.CreationTime)
            .Select<CourseSubmitRecordDto>()
            .ToPageListAsync(input.Page, input.Size);


            var courseIds = result.Select(x => x.CourseId).ToList();
            var userIds = result.Select(x => x.UserId).ToList();
            var courseList = await _sqlSugarClient.Queryable<Course>()
            .Where(x => courseIds.Contains(x.Id))
            .ToListAsync();
            var userList = await _sqlSugarClient.Queryable<AppUser>()
            .Where(x => userIds.Contains(x.Id))
            .ToListAsync();

            foreach (var item in result)
            {
                item.CourseName = courseList.FirstOrDefault(x => x.Id == item.CourseId)?.Title;
                item.CourseCover = courseList.FirstOrDefault(x => x.Id == item.CourseId)?.Cover;
                item.UserName = userList.FirstOrDefault(x => x.Id == item.UserId)?.Name;
                item.UserEmail = userList.FirstOrDefault(x => x.Id == item.UserId)?.Email;
            }




            return new PagedReuslt<CourseSubmitRecordDto>(result, result.Count);
        }
        public async Task<PagedReuslt<UserCourseSubmitRecordDto>> UserListAsync(CourseSubmitRecordPagedInput input)
        {
            var result = await _sqlSugarClient.Queryable<CourseSubmitRecord>()
            .WhereIF(input.CourseId != null, x => x.CourseId == input.CourseId)
            .WhereIF(input.UserId != null, x => x.UserId == input.UserId)
            .WhereIF(input.AuditStatus != null, x => x.AuditStatus == input.AuditStatus)

            .OrderByDescending(x => x.CreationTime)
             .Select<UserCourseSubmitRecordDto>()
            .ToPageListAsync(input.Page, input.Size);

            var courseIds = result.Select(x => x.CourseId).ToList();
            var userIds = result.Select(x => x.UserId).ToList();
            var courseList = await _sqlSugarClient.Queryable<Course>()
            .Where(x => courseIds.Contains(x.Id))
            .ToListAsync();

            foreach (var item in result)
            {
                item.CourseName = courseList.FirstOrDefault(x => x.Id == item.CourseId)?.Title;
                item.CourseCover = courseList.FirstOrDefault(x => x.Id == item.CourseId)?.Cover;

            }

            return new PagedReuslt<UserCourseSubmitRecordDto>(result, result.Count);
        }

        /// <summary>
        /// 用户上传
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task UserUploadAsync(CourseSubmitRecordDto input)
        {
            //查询对应的课程
            var course = await _sqlSugarClient.Queryable<Course>()
            .Where(x => x.Id == input.CourseId)
            .FirstAsync();
            if (course == null)
            {
                throw new YouJuException("课程不存在");
            }

            //查询用户是否存在待审核的记录
            var userCourseSubmitRecord = await _sqlSugarClient.Queryable<CourseSubmitRecord>()
            .Where(x => x.UserId == input.UserId)
            .Where(x => x.CourseId == input.CourseId)
            .Where(x => x.AuditStatus == CourseAuditStatus.审核中 || x.AuditStatus == CourseAuditStatus.审核通过 || x.AuditStatus == CourseAuditStatus.审核拒绝)
            .ToListAsync();
            if (userCourseSubmitRecord.Count > 0 && userCourseSubmitRecord.Any(x => x.AuditStatus == CourseAuditStatus.审核通过))
            {
                throw new YouJuException("已经审核通过了请勿重复上传");
            }
            if (userCourseSubmitRecord.Count > 0 && userCourseSubmitRecord.Any(x => x.AuditStatus == CourseAuditStatus.审核中))
            {
                throw new YouJuException("已经存在待审核的记录请勿重复上传");
            }
            //如果存在审核拒绝的说明是再次提交  进行作废处理
            if (userCourseSubmitRecord.Count > 0 && userCourseSubmitRecord.Any(x => x.AuditStatus == CourseAuditStatus.审核拒绝))
            {
                var rejectIds = userCourseSubmitRecord.Where(x => x.AuditStatus == CourseAuditStatus.审核拒绝).Select(x => x.Id).ToList();
                await _sqlSugarClient.Updateable<CourseSubmitRecord>()
                .SetColumns(x => x.AuditStatus == CourseAuditStatus.作废)
                .Where(x => rejectIds.Contains(x.Id))
                .ExecuteCommandAsync();
            }
            //进行创建
            var courseSubmitRecord = new CourseSubmitRecord()
            {
                CourseId = input.CourseId,
                UserId = input.UserId,
                AuditStatus = CourseAuditStatus.审核中,
                Points = course.Points,
                Remark = "",
                UploadImages = input.UploadImages,
            };
            await _sqlSugarClient.Insertable(courseSubmitRecord).ExecuteReturnEntityAsync();



        }


    }
}
