using MediatR;
using Web.Service;
using Microsoft.Extensions.Logging;

namespace Web.Events.Handlers
{
    /// <summary>
    /// 计划创建通知处理器
    /// </summary>
    public class PlanCreatedNotificationHandler : INotificationHandler<PlanCreatedNotification>
    {
        private readonly RuleCenterService _ruleCenterService;
        private readonly ILogger<PlanCreatedNotificationHandler> _logger;

        public PlanCreatedNotificationHandler(
            RuleCenterService ruleCenterService,
            ILogger<PlanCreatedNotificationHandler> logger)
        {
            _ruleCenterService = ruleCenterService;
            _logger = logger;
        }

        public async Task Handle(PlanCreatedNotification notification, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"处理新创建的计划: {notification.PlanId}, 案例类型: {notification.CaseType}");


                // 调用 RuleCenterService 的方法处理新创建的计划
                await _ruleCenterService.ProcessNewPlanAsync(notification.PlanId);

                _logger.LogInformation($"计划 {notification.PlanId} 处理完成");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"处理计划 {notification.PlanId} 时发生错误");
                // 根据需求决定是否重新抛出异常
            }
        }
    }
}