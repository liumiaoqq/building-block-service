namespace Web
{
    public class ArticleCategoryDto : FullBaseDto
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

    public class ArticleCategoryPagedInput : PagedBaseInput
    {
        [Description("分类名称")]
        public string Name { get; set; }

        [Description("是否显示")]
        public bool? IsVisible { get; set; }
    }
}
