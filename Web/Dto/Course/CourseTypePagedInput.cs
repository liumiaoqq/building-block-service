namespace Web
{
    public class CourseTypePagedInput : PagedBaseInput
    {
        [Description("课程类型名称")]
        public string Name { get; set; }


        [Description("课程类型排序")]
        public int? Sort { get; set; }

    }
}
