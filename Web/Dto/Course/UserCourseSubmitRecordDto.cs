using Microsoft.CodeAnalysis;
using Web.Dto.Plans;
using Web.Dto.Warehouses;

namespace Web
{
    public class UserCourseSubmitRecordDto
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


        [Description("课程名称")]
        public string CourseName { get; set; }

        [Description("课程封面")]
        public string CourseCover { get; set; }

        [Description("创建时间")]
        public DateTime CreationTime { get; set; }



        [Description("审核状态")]
        public string AuditStatusFormat => AuditStatus.ToDescription();



    }

}
