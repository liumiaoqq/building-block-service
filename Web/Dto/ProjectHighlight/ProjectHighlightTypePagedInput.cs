namespace Web
{
    public class ProjectHighlightTypePagedInput : PagedBaseInput
    {
        [Description("类型名称")]
        public string? Name { get; set; }

        [Description("是否上架")]
        public bool? IsPutaway { get; set; }

        [Description("排序")]
        public int? Sort { get; set; }
    }
}
