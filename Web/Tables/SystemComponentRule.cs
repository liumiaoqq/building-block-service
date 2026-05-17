using Web.DB;

namespace Web.Tables
{
    [Description("系统组件规则")]
    [YoungTable("SystemComponentRule")]
    public class SystemComponentRule : CreationAuditedAggregateRoot, IWarehouseId
    {

        [Description("名称")]
        public string Name { get; set; }



        [Description("对应语言")]
        public LanguageWay LanguageWay { get; set; }

        public Guid? WarehouseId { get; set; }
    }
}
