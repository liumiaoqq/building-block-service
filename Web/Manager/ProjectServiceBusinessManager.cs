using System.Text;

namespace Web.Manager
{
    public class ProjectServiceBusinessManager

    {
        protected ISqlSugarClient _sqlSugarClient;

        public ProjectServiceBusinessManager(ISqlSugarClient sqlSugarClient)
        {
            _sqlSugarClient = sqlSugarClient;
        }


        public async Task<PagedReuslt<ProjectServiceBusinessDto>> ListAsync(ProjectServiceBusinessPagedInput input)
        {
            var result = await _sqlSugarClient.Queryable<ProjectServiceBusiness>()
            .WhereIF(input.Title.IsNotNullOrNotWhiteSpace(), x => x.Title.Contains(input.Title))
            .WhereIF(input.IsPutaway.HasValue, x => x.IsPutaway == input.IsPutaway)
             .OrderByDescending(x => x.Sort)
            .Select<ProjectServiceBusinessDto>()
            .ToPageListAsync(input.Page, input.Size);
            return new PagedReuslt<ProjectServiceBusinessDto>(result, result.Count);
        }
        public async Task<PagedReuslt<UserProjectServiceBusinessDto>> UserListAsync(ProjectServiceBusinessPagedInput input)
        {
            var result = await _sqlSugarClient.Queryable<ProjectServiceBusiness>()
            .WhereIF(input.Title.IsNotNullOrNotWhiteSpace(), x => x.Title.Contains(input.Title))
            .Where(x => x.IsPutaway == true)
            .OrderByDescending(x => x.Sort)
             .Select<UserProjectServiceBusinessDto>() 
            .ToPageListAsync(input.Page, input.Size);
            return new PagedReuslt<UserProjectServiceBusinessDto>(result, result.Count);
        }
        public async Task<UserProjectServiceBusinessDetailDto> UserDetailAsync(Guid id)
        {
            var result = await _sqlSugarClient.Queryable<ProjectServiceBusiness>()
            .Where(x => x.Id == id)
            .Where(x => x.IsPutaway == true)
            .Select<UserProjectServiceBusinessDetailDto>()
            .FirstAsync();
          
            return result;
        }


    }
}
