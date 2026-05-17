namespace Web.Tables
{
    [Description("课程提交记录")]
    [YoungTable("CourseSubmitRecord")]
    public class CourseSubmitRecord : CreationAuditedAggregateRoot
    {

        [Description("课程")]
        public Guid CourseId { get; set; }


        [Description("可得积分")]
        public int Points { get; set; }

        [Description("用户")]
        public Guid UserId { get; set; }

        [Description("审核状态")]
        public CourseAuditStatus AuditStatus { get; set; }


        [Description("审核备注")]
        public string Remark { get; set; }

        [Description("图片")]
        public string UploadImages { get; set; }


        [Description("审核时间")]
        public DateTime? AuditTime { get; set; }


    }
}
