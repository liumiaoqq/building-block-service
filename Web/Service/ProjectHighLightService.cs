using Web.Manager;

namespace Web.Service
{
    public class ProjectHighLightService
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        private readonly ICurrentUser _currentUser;


        private readonly ProjectHighLightManager _projectHighLightManager;

        public ProjectHighLightService(ISqlSugarClient sqlSugarClient, ICurrentUser currentUser, ProjectHighLightManager projectHighLightManager)
        {
            _sqlSugarClient = sqlSugarClient;
            _currentUser = currentUser;
            _projectHighLightManager = projectHighLightManager;
        }

        public async Task<PagedReuslt<ProjectHighlightDto>> ListAsync(ProjectHighlightPagedInput input)
        {
            return await _projectHighLightManager.ListAsync(input);
        }
        /// <summary>
        /// 用户端获取项目亮点列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedReuslt<UserProjectHighLightDto>> UserListAsync(ProjectHighlightPagedInput input)
        {
            return await _projectHighLightManager.UserListAsync(input);
        }


    }
}
