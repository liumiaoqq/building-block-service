namespace Web
{
    public class DrawingBalancePagedInput : PagedBaseInput
    {
        [Description("用户")]
        public Guid? UserId { get; set; }

        [Description("用户账号")]
        public string UserName { get; set; }

        [Description("用户邮箱")]
        public string Email { get; set; }

        [Description("画图类型")]
        public DrawingType? DrawingType { get; set; }

        [Description("是否启用")]
        public bool? IsEnabled { get; set; }
    }
}

