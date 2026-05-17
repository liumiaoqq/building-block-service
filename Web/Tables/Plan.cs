using System.Numerics;
using Web.DB;

namespace Web.Tables
{

    /// <summary>
    /// 方案
    /// </summary>
    [YoungTable("Plan")]
    public class Plan : CreationAuditedAggregateRoot, IWarehouseId
    {
        [Description("方案名称")]
        public string PlanName { get; set; }
        [Description("数据库连接")]
        public string DatabaseConnection { get; set; }

        [Description("数据库脚本类型")]
        public Guid? SqlTempleteId { get; set; }

        [Description("文件名称")]
        public string FileName { get; set; }


        [Description("方案类型")]
        public PlanType? PlanType { get; set; }


        [Description("是否模板")]
        public bool IsTemplete { get; set; }
        public Guid? WarehouseId { get; set; }

        [Description("是否小程序")]
        [SugarColumn(IsNullable = true)]
        public bool IsMiniProgram { get; set; }

        [Description("后端端口")]
        [SugarColumn(IsNullable = true)]
        public int? BackPort { get; set; }




    }
}
