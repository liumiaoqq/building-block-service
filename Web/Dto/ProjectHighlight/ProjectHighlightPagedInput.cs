namespace Web
{
    public class ProjectHighlightPagedInput : PagedBaseInput
    {
        [Description("名称")]
        public string? Name { get; set; }

        [Description("类型")]
        public Guid? TypeId { get; set; }

        [Description("排序")]
        public int? Sort { get; set; }
    }
}
