using System.Text;

namespace Web.Manager
{
    public class ProjectHighLightTypeManager
    {
        protected ISqlSugarClient _sqlSugarClient;

        public ProjectHighLightTypeManager(ISqlSugarClient sqlSugarClient)
        {
            _sqlSugarClient = sqlSugarClient;
        }


        public async Task<ProjectHighlightType> CreateAsync(ProjectHighlightType projectHighlightType)
        {
            var result = await _sqlSugarClient.Insertable(projectHighlightType).ExecuteReturnEntityAsync();
            return result;
        }


        public async Task<PagedReuslt<ProjectHighlightTypeDto>> ListAsync(ProjectHighlightTypePagedInput input)
        {
            var result = await _sqlSugarClient.Queryable<ProjectHighlightType>()
            .WhereIF(input.Name.IsNotNullOrNotWhiteSpace(), x => x.Name.Contains(input.Name))
            .WhereIF(input.IsPutaway.HasValue, x => x.IsPutaway == input.IsPutaway)
            .WhereIF(input.Sort.HasValue, x => x.Sort == input.Sort)
             .OrderByDescending(x => x.Sort)
            .Select<ProjectHighlightTypeDto>()
  
            .ToPageListAsync(input.Page, input.Size);
            return new PagedReuslt<ProjectHighlightTypeDto>(result, result.Count);
        }
        public async Task<PagedReuslt<UserProjectHighlightTypeDto>> UserListAsync(ProjectHighlightTypePagedInput input)
        {
            var result = await _sqlSugarClient.Queryable<ProjectHighlightType>()
            .WhereIF(input.Name.IsNotNullOrNotWhiteSpace(), x => x.Name.Contains(input.Name))
            .Where(x => x.IsPutaway == true)
            .OrderByDescending(x => x.Sort)
             .Select<UserProjectHighlightTypeDto>() 
            .ToPageListAsync(input.Page, input.Size);
            return new PagedReuslt<UserProjectHighlightTypeDto>(result, result.Count);
        }



    }
}
