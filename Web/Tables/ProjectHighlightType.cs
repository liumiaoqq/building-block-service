namespace Web.Tables
{
    [Description("项目亮点类型")]
    [YoungTable("ProjectHighlightType")]
    public class ProjectHighlightType : CreationAuditedAggregateRoot
    {


        [Description("类型名称")]
        public string Name { get; set; }

        [Description("是否上架")]
        public bool IsPutaway { get; set; }

        [Description("排序")]
        public int Sort { get; set; }
    }
}
