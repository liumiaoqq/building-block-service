using Web.Manager;

namespace Web.Service
{
    public class CodeSnippetService
    {
        private readonly ISqlSugarClient _sqlSugarClient;
        private readonly ICurrentUser _currentUser;
        private readonly CodeSnippetManager _codeSnippetManager;

        public CodeSnippetService(
            ISqlSugarClient sqlSugarClient,
            ICurrentUser currentUser,
            CodeSnippetManager codeSnippetManager)
        {
            _sqlSugarClient = sqlSugarClient;
            _currentUser = currentUser;
            _codeSnippetManager = codeSnippetManager;
        }

        /// <summary>
        /// 获取代码片段树形结构
        /// </summary>
        public async Task<CodeSnippetTreeOutput> GetTreeAsync(Guid codeManagementId)
        {
            return await _codeSnippetManager.GetTreeAsync(codeManagementId);
        }

        /// <summary>
        /// 根据ID获取代码片段
        /// </summary>
        public async Task<CodeSnippetDto> GetByIdAsync(Guid id)
        {
            return await _codeSnippetManager.GetByIdAsync(id);
        }

        /// <summary>
        /// 根据ID获取代码片段内容
        /// </summary>
        public async Task<string> GetFileContentAsync(Guid id)
        {
            return await _codeSnippetManager.GetFileContentAsync(id);
        }

        /// <summary>
        /// 批量创建代码片段
        /// </summary>
        public async Task<int> BatchCreateAsync(BatchCreateCodeSnippetsInput input)
        {
            return await _codeSnippetManager.BatchCreateAsync(input);
        }

        /// <summary>
        /// 更新代码片段
        /// </summary>
        public async Task UpdateAsync(UpdateCodeSnippetInput input)
        {
            await _codeSnippetManager.UpdateAsync(input);
        }

        /// <summary>
        /// 删除代码片段
        /// </summary>
        public async Task DeleteAsync(Guid id)
        {
            await _codeSnippetManager.DeleteAsync(id);
        }

        /// <summary>
        /// 复制到新项目
        /// </summary>
        public async Task<Guid> CopyToNewProjectAsync(CopyToNewProjectInput input)
        {
            return await _codeSnippetManager.CopyToNewProjectAsync(input);
        }
    }
}
