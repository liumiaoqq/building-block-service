using Microsoft.CodeAnalysis;
using Web.Dto.Plans;
using Web.Dto.Warehouses;

namespace Web
{
    public class InviteRecordDto : FullBaseDto
    {

        [Description("邀请码")]
        public string VisitCode { get; set; }

        [Description("邀请用户")]
        public Guid UserId { get; set; }

        [Description("邀请用户名称")]
        public string UserName { get; set; }

        [Description("邀请用户邮箱")]
        public string Email { get; set; }



        [Description("被邀请用户")]
        public Guid InviteUserId { get; set; }

        [Description("被邀请用户名称")]
        public string InviteUserName { get; set; }

        [Description("被邀请用户邮箱")]
        public string InviteEmail { get; set; }



        [Description("邀请时间")]
        public DateTime VisitTime { get; set; }
        [Description("奖励")]
        public int Reward { get; set; }
    }
}
