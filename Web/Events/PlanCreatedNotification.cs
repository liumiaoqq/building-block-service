using MediatR;
using Web.Tables;

namespace Web.Events
{
    /// <summary>
    /// 计划创建通知
    /// </summary>
    public class PlanCreatedNotification : INotification
    {
        /// <summary>
        /// 新创建的计划ID
        /// </summary>
        public Guid PlanId { get; set; }


        /// <summary>
        /// 项目案例类型
        /// </summary>
        public ProjectCaseType CaseType { get; set; }
    }
}