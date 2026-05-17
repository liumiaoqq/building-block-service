namespace Web
{
    public class CourseSubmitRecordPagedInput : PagedBaseInput
    {

        [Description("课程")]
        public Guid? CourseId { get; set; }



        [Description("用户")]
        public Guid? UserId { get; set; }

        [Description("审核状态")]
        public CourseAuditStatus? AuditStatus { get; set; }


        [Description("审核备注")]
        public string Remark { get; set; }

    }
}
