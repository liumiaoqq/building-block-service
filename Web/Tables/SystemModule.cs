using Web.DB;

namespace Web.Tables
{
    [Description("系统模块")]
    [YoungTable("SystemModule")]
    public class SystemModule : CreationAuditedAggregateRoot, IWarehouseId
    {

        [Description("模块名称")]
        public string Name { get; set; }

        [Description("模块标签")]
        public string Label { get; set; }

        [Description("系统模块类型")]

        public SystemModuleType SystemModuleType { get; set; }

        public Guid? WarehouseId { get; set; }


    }
}
