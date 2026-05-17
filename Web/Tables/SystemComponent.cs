namespace Web.Tables
{
    [Description("单一组件")]
    [YoungTable("SystemComponent")]
    public class SystemComponent : CreationAuditedAggregateRoot
    {

        //[Description("组件类型")]
        //public ComponentType Type { get; set; }

        [Description("组件名")]
        public string Name { get; set; }

        [Description("对应语言")]
        public LanguageWay LanguageWay { get; set; }


        [Description("对应模块")]
        public Guid ModuleId { get; set; }




    }
}
