namespace Web.Tables
{
    [Description("文章分类")]
    [YoungTable("ArticleCategory")]
    public class ArticleCategory : CreationAuditedAggregateRoot
    {
        [Description("分类名称")]
        public string Name { get; set; }

        [Description("分类描述")]
        public string Description { get; set; }

        [Description("排序")]
        public int Sort { get; set; }

        [Description("是否显示")]
        public bool IsVisible { get; set; }
    }
}
