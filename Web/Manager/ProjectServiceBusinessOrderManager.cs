using System.Text;

namespace Web.Manager
{
    public class ProjectServiceBusinessOrderManager
    {
        protected ISqlSugarClient _sqlSugarClient;
        protected ICurrentUser CurrentUser;

    
        public ProjectServiceBusinessOrderManager(ISqlSugarClient sqlSugarClient, ICurrentUser currentUser)
        {
            _sqlSugarClient = sqlSugarClient;
            CurrentUser = currentUser;
        }


        public async Task<PagedReuslt<ProjectServiceBusinessOrderDto>> ListAsync(ProjectServiceBusinessOrderPagedInput  input)
        {
            var result = await _sqlSugarClient.Queryable<ProjectServiceBusinessOrder>()
            .WhereIF(input.No.IsNotNullOrNotWhiteSpace(), x => x.No.Contains(input.No))
            .WhereIF(input.PayNo.IsNotNullOrNotWhiteSpace(), x => x.PayNo.Contains(input.PayNo))
            .WhereIF(input.ProjectServiceBusinessId.HasValue, x => x.ProjectServiceBusinessId == input.ProjectServiceBusinessId)
            .WhereIF(input.OrderStatus.HasValue, x => x.OrderStatus == input.OrderStatus)
            .OrderByDescending(x=>x.CreationTime)
            .Select<ProjectServiceBusinessOrderDto>()
            .ToPageListAsync(input.Page, input.Size);
            return new PagedReuslt<ProjectServiceBusinessOrderDto>(result, result.Count);
        }
        public async Task<PagedReuslt<UserProjectServiceBusinessOrderDto>> UserListAsync(ProjectServiceBusinessOrderPagedInput input)
        {

            Guid userId=CurrentUser.UserId.Value;

             var result = await _sqlSugarClient.Queryable<ProjectServiceBusinessOrder>()
            .Where(x=>x.UserId == userId)
            .WhereIF(input.No.IsNotNullOrNotWhiteSpace(), x => x.No.Contains(input.No))
            .WhereIF(input.PayNo.IsNotNullOrNotWhiteSpace(), x => x.PayNo.Contains(input.PayNo))
            .WhereIF(input.ProjectServiceBusinessId.HasValue, x => x.ProjectServiceBusinessId == input.ProjectServiceBusinessId)
            .WhereIF(input.OrderStatus.HasValue, x => x.OrderStatus == input.OrderStatus)
            .OrderByDescending(x=>x.CreationTime)
            .Select<UserProjectServiceBusinessOrderDto>()
            .ToPageListAsync(input.Page, input.Size);
            return new PagedReuslt<UserProjectServiceBusinessOrderDto>(result, result.Count);
        }



    }
}
