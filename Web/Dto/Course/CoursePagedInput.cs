namespace Web
{
    public class CoursePagedInput : PagedBaseInput
    {

        [Description("标题")]
        public string Title { get; set; }



        [Description("课程类型")]
        public Guid? CourseTypeId { get; set; }



        [Description("作者")]
        public string Author { get; set; }

        [Description("审核状态")]
        public CourseAuditStatus? AuditStatus { get; set; }

    }
}
