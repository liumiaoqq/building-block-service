using Web.Manager;

namespace Web.Service
{
    public class ProjectServiceBusinessService
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        private readonly ICurrentUser _currentUser;


        private readonly ProjectServiceBusinessManager _projectServiceBusinessManager;

        public ProjectServiceBusinessService(ISqlSugarClient sqlSugarClient, ICurrentUser currentUser, ProjectServiceBusinessManager projectServiceBusinessManager)
        {
            _sqlSugarClient = sqlSugarClient;
            _currentUser = currentUser;
            _projectServiceBusinessManager = projectServiceBusinessManager;   
        }

        public async Task<PagedReuslt<ProjectServiceBusinessDto>> ListAsync(ProjectServiceBusinessPagedInput input)
        {
            return await _projectServiceBusinessManager.ListAsync(input);  
        }
        /// <summary>
        /// 用户端获取项目服务业务列表  
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedReuslt<UserProjectServiceBusinessDto>> UserListAsync(ProjectServiceBusinessPagedInput input)
        {
            return await _projectServiceBusinessManager.UserListAsync(input);
        }
        /// <summary>
        /// 用户端获取项目服务业务详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<UserProjectServiceBusinessDetailDto> UserDetailAsync(Guid id)
        {
            return await _projectServiceBusinessManager.UserDetailAsync(id);
        }



    }
}
