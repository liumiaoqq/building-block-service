using Web.Extensions;
using Web.Service;
using Web.Tables;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DrawingBalanceController : YouJuController<DrawingBalance, DrawingBalanceDto, DrawingBalancePagedInput>
    {
        private readonly DrawingBalanceService _drawingBalanceService;

        public DrawingBalanceController(IServiceProvider serviceProvider, DrawingBalanceService drawingBalanceService) : base(serviceProvider)
        {
            _drawingBalanceService = drawingBalanceService;
        }

        [HttpPost("ListAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async override Task<PagedReuslt<DrawingBalanceDto>> ListAsync(DrawingBalancePagedInput input)
        {
            return await _drawingBalanceService.ListAsync(input);
        }

        /// <summary>
        /// 获取用户自己的图形余额列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("UserListAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<PagedReuslt<DrawingBalanceDto>> UserListAsync(DrawingBalancePagedInput input)
        {
            return await _drawingBalanceService.UserListAsync(input);
        }

        [HttpPost("DeleteAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public override async Task DeleteAsync(IdInput<Guid> input)
        {
            await base.DeleteAsync(input);
        }

        /// <summary>
        /// 获取用户画图余额汇总
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetUserBalanceSummaryAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<UserDrawingBalanceSummaryDto> GetUserBalanceSummaryAsync()
        {
            return await _drawingBalanceService.GetUserBalanceSummaryAsync();
        }

        /// <summary>
        /// 检查用户是否有权限调用画图
        /// </summary>
        /// <param name="drawingType"></param>
        /// <returns></returns>
        [HttpPost("CheckDrawingPermissionAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<bool> CheckDrawingPermissionAsync(DrawingType drawingType)
        {
            return await _drawingBalanceService.CheckDrawingPermissionAsync(drawingType);
        }

        /// <summary>
        /// 记录画图调用
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("RecordDrawingCallAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task RecordDrawingCallAsync(RecordDrawingCallInput input)
        {
            await _drawingBalanceService.RecordDrawingCallAsync(input.DrawingType, input.IsSuccess, input.FailReason);
        }

        /// <summary>
        /// 检查用户余额并记录ER图调用（原子操作）
        /// </summary>
        /// <returns>检查结果，包含是否有权限、剩余次数等信息</returns>
        [HttpPost("CheckAndRecordERDiagramAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<CheckAndRecordDrawingResult> CheckAndRecordERDiagramAsync()
        {
            return await _drawingBalanceService.CheckAndRecordDrawingAsync(DrawingType.ER图);
        }

        /// <summary>
        /// 检查用户余额并记录流程图调用（原子操作）
        /// </summary>
        /// <returns>检查结果，包含是否有权限、剩余次数等信息</returns>
        [HttpPost("CheckAndRecordFlowchartAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<CheckAndRecordDrawingResult> CheckAndRecordFlowchartAsync()
        {
            return await _drawingBalanceService.CheckAndRecordDrawingAsync(DrawingType.流程图);
        }

        /// <summary>
        /// 检查用户余额并记录时序图调用（原子操作）
        /// </summary>
        /// <returns>检查结果，包含是否有权限、剩余次数等信息</returns>
        [HttpPost("CheckAndRecordSequenceDiagramAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<CheckAndRecordDrawingResult> CheckAndRecordSequenceDiagramAsync()
        {
            return await _drawingBalanceService.CheckAndRecordDrawingAsync(DrawingType.时序图);
        }

        /// <summary>
        /// 检查用户余额并记录用例图调用（原子操作）
        /// </summary>
        /// <returns>检查结果，包含是否有权限、剩余次数等信息</returns>
        [HttpPost("CheckAndRecordUseCaseDiagramAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<CheckAndRecordDrawingResult> CheckAndRecordUseCaseDiagramAsync()
        {
            return await _drawingBalanceService.CheckAndRecordDrawingAsync(DrawingType.用例图);
        }

        /// <summary>
        /// 检查用户余额并记录三线表调用（原子操作）
        /// </summary>
        /// <returns>检查结果，包含是否有权限、剩余次数等信息</returns>
        [HttpPost("CheckAndRecordThreeLineTableAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<CheckAndRecordDrawingResult> CheckAndRecordThreeLineTableAsync()
        {
            return await _drawingBalanceService.CheckAndRecordDrawingAsync(DrawingType.三线表);
        }

        /// <summary>
        /// 检查用户余额并记录项目结构图调用（原子操作）
        /// </summary>
        /// <returns>检查结果，包含是否有权限、剩余次数等信息</returns>
        [HttpPost("CheckAndRecordProjectStructureAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<CheckAndRecordDrawingResult> CheckAndRecordProjectStructureAsync()
        {
            return await _drawingBalanceService.CheckAndRecordDrawingAsync(DrawingType.项目结构图);
        }

        /// <summary>
        /// 获取画图类型枚举列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetDrawingTypeListAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public PagedReuslt<SelectResult> GetDrawingTypeListAsync()
        {
            var list = typeof(DrawingType).GetEnumList().Select(x => new SelectResult() { Name = x.Key, Value = x.Value }).ToList();
            return new PagedReuslt<SelectResult>(list, list.Count);
        }
    }

    public class RecordDrawingCallInput
    {
        [Description("画图类型")]
        public DrawingType DrawingType { get; set; }

        [Description("是否成功")]
        public bool IsSuccess { get; set; }

        [Description("失败原因")]
        public string FailReason { get; set; }
    }
}

