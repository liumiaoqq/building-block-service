using Web.DB;

namespace Web.Tables
{
  
    [Description("系统组件配置明细")]
    [YoungTable("SystemComponentSettingDet")]
    public class SystemComponentSettingDet : CreationAuditedAggregateRoot
    {

        [Description("键")]
        public string Key { get; set; }

        [Description("值")]
        public string Value { get; set; }

        [Description("唯一码")]
        public string Code { get; set; }

        public Guid SystemComponentId { get; set; }


    }
}
