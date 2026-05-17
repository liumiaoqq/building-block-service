namespace Web.Tables
{
    [Description("文章")]
    [YoungTable("Article")]
    public class Article : CreationAuditedAggregateRoot
    {
        [Description("文章标题")]
        public string Title { get; set; }

        [Description("文章内容")]
        [SugarColumn(ColumnDataType = "text")]
        public string Content { get; set; }

        [Description("文章摘要")]
        public string Summary { get; set; }

        [Description("封面图片")]
        public string CoverImage { get; set; }

        [Description("分类Id")]
        public Guid CategoryId { get; set; }

        [Description("排序")]
        public int Sort { get; set; }

        [Description("是否显示")]
        public bool IsVisible { get; set; }

        [Description("浏览次数")]
        public int ViewCount { get; set; }
    }
}
