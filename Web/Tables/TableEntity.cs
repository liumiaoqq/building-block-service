namespace Web.Tables
{
    [Description("表实体")]
    [YoungTable("TableEntity")]
    public class TableEntity : CreationAuditedAggregateRoot
    {
        [Description("实体名称")]
        public string Name { get; set; }

        [Description("实体编码")]
        public string Code { get; set; }

        [Description("对应计划")]
        public Guid PlanId { get; set; }
        [Description("是否额外的")]
        public bool IsExtra { get; set; }


        [Description("是否公开")]
        public bool IsOpen { get; set; }



        [Description("关联次数")]
        public int RelativeCount { get; set; }


        [Description("是否内置")]
        public bool IsBuiltin { get; set; }




    }
}
