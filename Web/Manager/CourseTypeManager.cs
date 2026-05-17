using System.Text;

namespace Web.Manager
{
    public class CourseTypeManager
    {
        protected ISqlSugarClient _sqlSugarClient;

        public CourseTypeManager(ISqlSugarClient sqlSugarClient)
        {
            _sqlSugarClient = sqlSugarClient;
        }


        public async Task<CourseType> CreateAsync(CourseType CourseType)
        {
            var result = await _sqlSugarClient.Insertable(CourseType).ExecuteReturnEntityAsync();
            return result;
        }


        public async Task<PagedReuslt<CourseTypeDto>> ListAsync(CourseTypePagedInput input)
        {
            var result = await _sqlSugarClient.Queryable<CourseType>()
            .WhereIF(input.Name.IsNotNullOrNotWhiteSpace(), x => x.Name.Contains(input.Name))

            .WhereIF(input.Sort.HasValue, x => x.Sort == input.Sort)
             .OrderByDescending(x => x.Sort)
            .Select<CourseTypeDto>()

            .ToPageListAsync(input.Page, input.Size);
            return new PagedReuslt<CourseTypeDto>(result, result.Count);
        }
        public async Task<PagedReuslt<UserCourseTypeDto>> UserListAsync(CourseTypePagedInput input)
        {
            var result = await _sqlSugarClient.Queryable<CourseType>()
            .WhereIF(input.Name.IsNotNullOrNotWhiteSpace(), x => x.Name.Contains(input.Name))

            .OrderByDescending(x => x.Sort)
             .Select<UserCourseTypeDto>()
            .ToPageListAsync(input.Page, input.Size);

            return new PagedReuslt<UserCourseTypeDto>(result, result.Count);
        }



    }
}
