using Microsoft.CodeAnalysis;
using Web.Dto.Plans;
using Web.Dto.Warehouses;

namespace Web
{
    public class UserCourseSummaryDto
    {

        [Description("课程总积分")]
        public int TotalPoints { get; set; }

        [Description("累计获取总积分")]
        public int TotalGetPoints { get; set; }

        [Description("审核通过数量")]
        public int TotalPassCount { get; set; }

        [Description("审核不通过数量")]
        public int TotalFailCount { get; set; }

        [Description("审核中数量")]
        public int TotalPendingCount { get; set; }

        


        [Description("剩余上传数量")]
        public int RemainingUploadCount { get; set; }

        [Description("今天是否完成签到")]
        public bool IsDailySignIn { get; set; }




    }

}
