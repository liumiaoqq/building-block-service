using Microsoft.CodeAnalysis;
using Web.Dto.Plans;
using Web.Dto.Warehouses;

namespace Web
{
    public class UserInviteRecordDto
    {

        [Description("邀请码")]
        public string VisitCode { get; set; }



        [Description("被邀请用户")]
        public string UserName { get; set; }

        [Description("被邀请用户邮箱")]
        public string Email { get; set; }

        [Description("邀请时间")]
        public DateTime VisitTime { get; set; }

        [Description("奖励积分")]
        public int Points { get; set; }
    }
}
