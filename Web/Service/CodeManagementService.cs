using Web.Manager;

namespace Web.Service
{
    public class CodeManagementService
    {
        private readonly ISqlSugarClient _sqlSugarClient;
        private readonly ICurrentUser _currentUser;
        private readonly CodeManagementManager _codeManagementManager;

        public CodeManagementService(
            ISqlSugarClient sqlSugarClient,
            ICurrentUser currentUser,
            CodeManagementManager codeManagementManager)
        {
            _sqlSugarClient = sqlSugarClient;
            _currentUser = currentUser;
            _codeManagementManager = codeManagementManager;
        }

        /// <summary>
        /// 获取代码管理分页列表
        /// </summary>
        public async Task<PagedReuslt<CodeManagementDto>> ListAsync(GetCodeManagementPagedInput input)
        {
            return await _codeManagementManager.ListAsync(input);
        }

        /// <summary>
        /// 根据ID获取代码管理
        /// </summary>
        public async Task<CodeManagementDto> GetByIdAsync(Guid id)
        {
            return await _codeManagementManager.GetByIdAsync(id);
        }

        /// <summary>
        /// 创建代码管理
        /// </summary>
        public async Task<Guid> CreateAsync(CreateCodeManagementInput input)
        {
            return await _codeManagementManager.CreateAsync(input);
        }

        /// <summary>
        /// 更新代码管理
        /// </summary>
        public async Task UpdateAsync(UpdateCodeManagementInput input)
        {
            await _codeManagementManager.UpdateAsync(input);
        }

        /// <summary>
        /// 更新项目信息（仅更新项目名称、描述和备注）
        /// </summary>
        public async Task UpdateProjectInfoAsync(UpdateProjectInfoInput input)
        {
            await _codeManagementManager.UpdateProjectInfoAsync(input);
        }

        /// <summary>
        /// 删除代码管理
        /// </summary>
        public async Task DeleteAsync(Guid id)
        {
            await _codeManagementManager.DeleteAsync(id);
        }

        /// <summary>
        /// 批量删除代码管理
        /// </summary>
        public async Task BatchDeleteAsync(List<Guid> ids)
        {
            await _codeManagementManager.BatchDeleteAsync(ids);
        }

        /// <summary>
        /// 获取代码管理统计信息
        /// </summary>
        public async Task<CodeManagementStatisticsOutput> GetStatisticsAsync()
        {
            return await _codeManagementManager.GetStatisticsAsync();
        }

        /// <summary>
        /// AI分析代码模块
        /// </summary>
        public async Task<AnalyzeCodeModulesOutput> AnalyzeCodeModulesAsync(AnalyzeCodeModulesInput input)
        {
            return await _codeManagementManager.AnalyzeCodeModulesAsync(input);
        }

        /// <summary>
        /// 批量拆分代码模块
        /// </summary>
        public async Task BatchSplitModulesAsync(BatchSplitModulesInput input)
        {
            await _codeManagementManager.BatchSplitModulesAsync(input);
        }
    }
}

