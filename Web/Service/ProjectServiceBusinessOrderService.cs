using Web.Manager;

namespace Web.Service
{
    public class ProjectServiceBusinessOrderService
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        private readonly ICurrentUser _currentUser;


        private readonly ProjectServiceBusinessOrderManager _projectServiceBusinessOrderManager;

        public ProjectServiceBusinessOrderService(ISqlSugarClient sqlSugarClient, ICurrentUser currentUser, ProjectServiceBusinessOrderManager projectServiceBusinessOrderManager)
        {
            _sqlSugarClient = sqlSugarClient;
            _currentUser = currentUser;
            _projectServiceBusinessOrderManager = projectServiceBusinessOrderManager;       
        }

        public async Task<PagedReuslt<ProjectServiceBusinessOrderDto>> ListAsync(ProjectServiceBusinessOrderPagedInput input)       

        {
            return await _projectServiceBusinessOrderManager.ListAsync(input);  
        }
        /// <summary>
        /// 用户端获取项目服务业务列表  
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedReuslt<UserProjectServiceBusinessOrderDto>> UserListAsync(ProjectServiceBusinessOrderPagedInput input)   
        {
            return await _projectServiceBusinessOrderManager.UserListAsync(input);
        }


    }
}
