using System.Numerics;
using Web.DB;

namespace Web.Tables
{

    /// <summary>
    /// 项目案例模块
    /// </summary>
    [YoungTable("ProjectCaseModule")]
    public class ProjectCaseModule : CreationAuditedAggregateRoot, IWarehouseId
    {
        [Description("模块名称")]
        public string ModuleName { get; set; }
        [Description("系统模块")]
        public Guid SystemModuleId { get; set; }

        [Description("项目案例")]
        public Guid ProjectCaseId { get; set; }

        [Description("排序")]
        public int Sort { get; set; }

        public Guid? WarehouseId { get; set; }
    }
}
