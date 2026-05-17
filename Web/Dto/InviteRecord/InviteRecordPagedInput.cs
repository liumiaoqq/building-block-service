namespace Web
{
    public class InviteRecordPagedInput : PagedBaseInput
    {
        [Description("邀请码")]
        public string VisitCode { get; set; }



        [Description("邀请用户名称")]
        public string InviteUserName { get; set; }
        [Description("被邀请用户名称")]
        public string UserName { get; set; }
    }
}
