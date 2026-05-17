using Web.Manager;

namespace Web.Service
{
    public class DrawingBalanceService
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        private readonly ICurrentUser _currentUser;

        private readonly DrawingBalanceManager _drawingBalanceManager;

        private readonly DrawingCallRecordManager _drawingCallRecordManager;

        public DrawingBalanceService(ISqlSugarClient sqlSugarClient, ICurrentUser currentUser, DrawingBalanceManager drawingBalanceManager, DrawingCallRecordManager drawingCallRecordManager)
        {
            _sqlSugarClient = sqlSugarClient;
            _currentUser = currentUser;
            _drawingBalanceManager = drawingBalanceManager;
            _drawingCallRecordManager = drawingCallRecordManager;
        }

        public async Task<PagedReuslt<DrawingBalanceDto>> ListAsync(DrawingBalancePagedInput input)
        {
            return await _drawingBalanceManager.ListAsync(input);
        }

        /// <summary>
        /// 获取用户自己的图形余额列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedReuslt<DrawingBalanceDto>> UserListAsync(DrawingBalancePagedInput input)
        {
            var userId = _currentUser.UserId;

            if (userId == null)
            {
                throw new Exception("用户未登录");
            }

            // 设置查询条件为当前用户
            input.UserId = userId.Value;

            return await _drawingBalanceManager.ListAsync(input);
        }

        /// <summary>
        /// 获取用户画图余额汇总
        /// </summary>
        /// <returns></returns>
        public async Task<UserDrawingBalanceSummaryDto> GetUserBalanceSummaryAsync()
        {
            var userId = _currentUser.UserId;

            if (userId == null)
            {
                throw new Exception("用户未登录");
            }

            // 获取用户所有有效的画图余额配置
            var balances = await _drawingBalanceManager.GetAllEffectiveBalancesAsync(userId.Value);

            // 获取今日所有类型的调用成功次数
            var todayCounts = await _drawingCallRecordManager.GetTodayAllSuccessCountAsync(userId.Value);

            var summary = new UserDrawingBalanceSummaryDto();

            // ER图
            var erBalance = balances.FirstOrDefault(x => x.DrawingType == DrawingType.ER图);
            summary.ERDiagramMaxDaily = erBalance?.MaxDailyGenerations ?? 0;
            var erTodayCount = todayCounts.ContainsKey(DrawingType.ER图) ? todayCounts[DrawingType.ER图] : 0;
            summary.ERDiagramRemainingToday = Math.Max(0, summary.ERDiagramMaxDaily - erTodayCount);

            // 流程图
            var flowchartBalance = balances.FirstOrDefault(x => x.DrawingType == DrawingType.流程图);
            summary.FlowchartMaxDaily = flowchartBalance?.MaxDailyGenerations ?? 0;
            var flowchartTodayCount = todayCounts.ContainsKey(DrawingType.流程图) ? todayCounts[DrawingType.流程图] : 0;
            summary.FlowchartRemainingToday = Math.Max(0, summary.FlowchartMaxDaily - flowchartTodayCount);

            // 时序图
            var sequenceBalance = balances.FirstOrDefault(x => x.DrawingType == DrawingType.时序图);
            summary.SequenceDiagramMaxDaily = sequenceBalance?.MaxDailyGenerations ?? 0;
            var sequenceTodayCount = todayCounts.ContainsKey(DrawingType.时序图) ? todayCounts[DrawingType.时序图] : 0;
            summary.SequenceDiagramRemainingToday = Math.Max(0, summary.SequenceDiagramMaxDaily - sequenceTodayCount);

            // 用例图
            var useCaseBalance = balances.FirstOrDefault(x => x.DrawingType == DrawingType.用例图);
            summary.UseCaseDiagramMaxDaily = useCaseBalance?.MaxDailyGenerations ?? 0;
            var useCaseTodayCount = todayCounts.ContainsKey(DrawingType.用例图) ? todayCounts[DrawingType.用例图] : 0;
            summary.UseCaseDiagramRemainingToday = Math.Max(0, summary.UseCaseDiagramMaxDaily - useCaseTodayCount);

            // 三线表
            var threeLineTableBalance = balances.FirstOrDefault(x => x.DrawingType == DrawingType.三线表);
            summary.ThreeLineTableMaxDaily = threeLineTableBalance?.MaxDailyGenerations ?? 0;
            var threeLineTableTodayCount = todayCounts.ContainsKey(DrawingType.三线表) ? todayCounts[DrawingType.三线表] : 0;
            summary.ThreeLineTableRemainingToday = Math.Max(0, summary.ThreeLineTableMaxDaily - threeLineTableTodayCount);

            // 项目结构图
            var projectStructureBalance = balances.FirstOrDefault(x => x.DrawingType == DrawingType.项目结构图);
            summary.ProjectStructureDiagramMaxDaily = projectStructureBalance?.MaxDailyGenerations ?? 0;
            var projectStructureTodayCount = todayCounts.ContainsKey(DrawingType.项目结构图) ? todayCounts[DrawingType.项目结构图] : 0;
            summary.ProjectStructureDiagramRemainingToday = Math.Max(0, summary.ProjectStructureDiagramMaxDaily - projectStructureTodayCount);

            return summary;
        }

        /// <summary>
        /// 检查用户是否有权限调用画图
        /// </summary>
        /// <param name="drawingType"></param>
        /// <returns></returns>
        public async Task<bool> CheckDrawingPermissionAsync(DrawingType drawingType)
        {
            var userId = _currentUser.UserId;

            if (userId == null)
            {
                return false;
            }

            // 获取有效的余额配置
            var balance = await _drawingBalanceManager.GetEffectiveBalanceAsync(userId.Value, drawingType);

            if (balance == null)
            {
                return false;
            }

            // 获取今日已使用次数
            var todayCount = await _drawingCallRecordManager.GetTodaySuccessCountAsync(userId.Value, drawingType);

            // 判断是否还有剩余次数
            return todayCount < balance.MaxDailyGenerations;
        }

        /// <summary>
        /// 记录画图调用
        /// </summary>
        /// <param name="drawingType"></param>
        /// <param name="isSuccess"></param>
        /// <param name="failReason"></param>
        /// <returns></returns>
        public async Task RecordDrawingCallAsync(DrawingType drawingType, bool isSuccess, string failReason = "")
        {
            var userId = _currentUser.UserId;

            if (userId == null)
            {
                throw new Exception("用户未登录");
            }

            var record = new DrawingCallRecord
            {
                Id = Guid.NewGuid(),
                UserId = userId.Value,
                DrawingType = drawingType,
                CallTime = DateTime.Now,
                IsSuccess = isSuccess,
                FailReason = failReason,
                CreationTime = DateTime.Now
            };

            await _drawingCallRecordManager.CreateAsync(record);
        }

        /// <summary>
        /// 检查用户余额并记录画图调用（原子操作）
        /// </summary>
        /// <param name="drawingType">画图类型</param>
        /// <returns>检查结果对象，包含是否有权限、剩余次数等信息</returns>
        public async Task<CheckAndRecordDrawingResult> CheckAndRecordDrawingAsync(DrawingType drawingType)
        {
            var userId = _currentUser.UserId;

            if (userId == null)
            {
                throw new Exception("用户未登录");
            }

            var result = new CheckAndRecordDrawingResult
            {
                HasPermission = false,
                RemainingCount = 0,
                MaxDailyCount = 0,
                Message = ""
            };

            // 获取有效的余额配置
            var balance = await _drawingBalanceManager.GetEffectiveBalanceAsync(userId.Value, drawingType);

            if (balance == null)
            {
                result.Message = "您还没有开通画图权限，请联系管理员开通";
                return result;
            }

            result.MaxDailyCount = balance.MaxDailyGenerations;

            // 获取今日已使用次数
            var todayCount = await _drawingCallRecordManager.GetTodaySuccessCountAsync(userId.Value, drawingType);

            result.RemainingCount = Math.Max(0, balance.MaxDailyGenerations - todayCount);

            // 判断是否还有剩余次数
            if (todayCount >= balance.MaxDailyGenerations)
            {
                result.Message = $"今日{drawingType}生成次数已用完，每日最多可生成{balance.MaxDailyGenerations}次";
                return result;
            }

            // 有权限，记录本次调用
            await RecordDrawingCallAsync(drawingType, true);

            result.HasPermission = true;
            result.RemainingCount = Math.Max(0, balance.MaxDailyGenerations - todayCount - 1); // 减去本次使用
            result.Message = "检查通过";

            return result;
        }
    }
}

