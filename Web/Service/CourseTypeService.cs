using Web.Manager;

namespace Web.Service
{
    public class CourseTypeService
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        private readonly ICurrentUser _currentUser;

        private readonly CourseTypeManager _courseTypeManager;

        public CourseTypeService(ISqlSugarClient sqlSugarClient, ICurrentUser currentUser, CourseTypeManager courseTypeManager)
        {
            _sqlSugarClient = sqlSugarClient;
            _currentUser = currentUser;
            _courseTypeManager = courseTypeManager;
        }

        public async Task<PagedReuslt<CourseTypeDto>> ListAsync(CourseTypePagedInput input)
        {
            return await _courseTypeManager.ListAsync(input);
        }
        /// <summary>
        /// 用户端获取课程类型列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedReuslt<UserCourseTypeDto>> UserListAsync(CourseTypePagedInput input)
        {
            return await _courseTypeManager.UserListAsync(input);
        }


    }
}
