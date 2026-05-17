using Microsoft.CodeAnalysis;
using Web.Dto.Plans;
using Web.Dto.Warehouses;

namespace Web
{
    public class UserCourseDto : FullBaseDto
    {

        [Description("标题")]
        public string Title { get; set; }
        [Description("封面")]

        public string Cover { get; set; }
        [Description("Bilibili链接")]

        public string BILIBILILinks { get; set; }

        [Description("课程类型")]
        public Guid? CourseTypeId { get; set; }
    
        [Description("课程类型")]
        public string CourseTypeName { get; set; }

        [Description("可得积分")]
        public int Points { get; set; }


        [Description("作者")]
        public string Author { get; set; }

        [Description("审核状态")]
        public CourseAuditStatus AuditStatus { get; set; }

        [Description("审核状态")]
        public string AuditStatusFormat => AuditStatus.ToDescription();

    }

}
