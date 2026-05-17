using System.Collections.Specialized;
using System.Text;

namespace Web.Manager
{
    public class CourseManager
    {
        protected ISqlSugarClient _sqlSugarClient;

        public CourseManager(ISqlSugarClient sqlSugarClient)
        {
            _sqlSugarClient = sqlSugarClient;
        }


        public async Task<Course> CreateAsync(Course Course)
        {
            var result = await _sqlSugarClient.Insertable(Course).ExecuteReturnEntityAsync();
            return result;
        }


        public async Task<PagedReuslt<CourseDto>> ListAsync(CoursePagedInput input)
        {
            var result = await _sqlSugarClient.Queryable<Course>()
            .WhereIF(input.Title.IsNotNullOrNotWhiteSpace(), x => x.Title.Contains(input.Title))
            .WhereIF(input.CourseTypeId != null, x => x.CourseTypeId == input.CourseTypeId)
            .OrderByDescending(x => x.Sort)
            .Select<CourseDto>()

            .ToPageListAsync(input.Page, input.Size);
            //得到对应的课程类型ids
            var courseTypeIds = result.Select(x => x.CourseTypeId).Distinct().ToList();
            //根据课程类型ids得到课程类型名称
            var courseTypeNames = await _sqlSugarClient.Queryable<CourseType>()
            .Where(x => courseTypeIds.Contains(x.Id))
            .ToListAsync();
            //给结果集赋值
            foreach (var item in result)
            {
                item.CourseTypeName = courseTypeNames.FirstOrDefault(x => x.Id == item.CourseTypeId).Name;




            }



            return new PagedReuslt<CourseDto>(result, result.Count);
        }
        public async Task<PagedReuslt<UserCourseDto>> UserListAsync(CoursePagedInput input)
        {



            var result = await _sqlSugarClient.Queryable<Course>()
            .WhereIF(input.Title.IsNotNullOrNotWhiteSpace(), x => x.Title.Contains(input.Title))
            .WhereIF(input.CourseTypeId != null, x => x.CourseTypeId == input.CourseTypeId)

            .OrderByDescending(x => x.Sort)
             .Select<UserCourseDto>()
            .ToPageListAsync(input.Page, input.Size);

            return new PagedReuslt<UserCourseDto>(result, result.Count);
        }





    }
}
