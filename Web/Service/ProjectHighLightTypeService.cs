using Web.Manager;

namespace Web.Service
{
    public class ProjectHighLightTypeService
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        private readonly ICurrentUser _currentUser;


        private readonly ProjectHighLightTypeManager _projectHighLightTypeManager;

        public ProjectHighLightTypeService(ISqlSugarClient sqlSugarClient, ICurrentUser currentUser, ProjectHighLightTypeManager projectHighLightTypeManager)
        {
            _sqlSugarClient = sqlSugarClient;
            _currentUser = currentUser;
            _projectHighLightTypeManager = projectHighLightTypeManager;
        }

        public async Task<PagedReuslt<ProjectHighlightTypeDto>> ListAsync(ProjectHighlightTypePagedInput input)
        {
            return await _projectHighLightTypeManager.ListAsync(input);
        }
        /// <summary>
        /// 用户端获取项目亮点类型列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedReuslt<UserProjectHighlightTypeDto>> UserListAsync(ProjectHighlightTypePagedInput input)
        {
            return await _projectHighLightTypeManager.UserListAsync(input);
        }


    }
}
