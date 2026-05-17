namespace Web.Tables
{
    [Description("邀请记录")]
    [YoungTable("InviteRecord")]
    public class InviteRecord : CreationAuditedAggregateRoot
    {


        [Description("邀请码")]
        public string VisitCode { get; set; }

        [Description("被邀请用户")]//一般是自己
        public Guid UserId { get; set; }

        [Description("邀请用户")]
        public Guid InviteUserId { get; set; }

        [Description("邀请时间")]
        public DateTime VisitTime { get; set; }

        [Description("奖励")]
        public int Reward { get; set; }







    }
}
