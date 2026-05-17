using Web.Extensions;
using Web.Service;
using Web.Tables;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CodeManagementController : YouJuController<CodeManagement, CodeManagementDto, GetCodeManagementPagedInput>
    {
        private readonly CodeManagementService _codeManagementService;

        public CodeManagementController(
            IServiceProvider serviceProvider,
            CodeManagementService codeManagementService) : base(serviceProvider)
        {
            _codeManagementService = codeManagementService;
        }

        /// <summary>
        /// 获取代码管理分页列表
        /// </summary>
        [HttpPost("ListAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async override Task<PagedReuslt<CodeManagementDto>> ListAsync(GetCodeManagementPagedInput input)
        {
            return await _codeManagementService.ListAsync(input);
        }

        /// <summary>
        /// 根据ID获取代码管理
        /// </summary>
        [HttpPost("GetByIdAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<CodeManagementDto> GetByIdAsync(IdInput<Guid> input)
        {
            return await _codeManagementService.GetByIdAsync(input.Id);
        }

        /// <summary>
        /// 创建代码管理
        /// </summary>
        [HttpPost("CreateAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<Guid> CreateAsync(CreateCodeManagementInput input)
        {
            return await _codeManagementService.CreateAsync(input);
        }

        /// <summary>
        /// 更新代码管理
        /// </summary>
        [HttpPost("UpdateAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task UpdateAsync(UpdateCodeManagementInput input)
        {
            await _codeManagementService.UpdateAsync(input);
        }

        /// <summary>
        /// 更新项目信息（仅更新项目名称、描述和备注）
        /// </summary>
        [HttpPost("UpdateProjectInfoAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task UpdateProjectInfoAsync(UpdateProjectInfoInput input)
        {
            await _codeManagementService.UpdateProjectInfoAsync(input);
        }

        /// <summary>
        /// 删除代码管理
        /// </summary>
        [HttpPost("DeleteAsync")]
        [CustomAuthorization(RoleType.用户)]
        public override async Task DeleteAsync(IdInput<Guid> input)
        {
            await _codeManagementService.DeleteAsync(input.Id);
        }

        /// <summary>
        /// 批量删除代码管理
        /// </summary>
        [HttpPost("BatchDeleteAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task BatchDeleteAsync(BatchDeleteInput input)
        {
            await _codeManagementService.BatchDeleteAsync(input.Ids);
        }

        /// <summary>
        /// 获取代码管理统计信息
        /// </summary>
        [HttpPost("GetStatisticsAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<CodeManagementStatisticsOutput> GetStatisticsAsync()
        {
            return await _codeManagementService.GetStatisticsAsync();
        }

        /// <summary>
        /// AI分析代码模块
        /// </summary>
        [HttpPost("AnalyzeCodeModulesAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<AnalyzeCodeModulesOutput> AnalyzeCodeModulesAsync(AnalyzeCodeModulesInput input)
        {
            return await _codeManagementService.AnalyzeCodeModulesAsync(input);
        }

        /// <summary>
        /// 批量拆分代码模块
        /// </summary>
        [HttpPost("BatchSplitModulesAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task BatchSplitModulesAsync(BatchSplitModulesInput input)
        {
            await _codeManagementService.BatchSplitModulesAsync(input);
        }
    }
}

