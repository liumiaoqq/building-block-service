namespace Web
{
    public class ProjectServiceBusinessPagedInput : PagedBaseInput
    {
         [Description("标题")]
        public string Title { get; set; }
    [Description("是否上架")]
        public bool? IsPutaway { get; set; }
    }
}
