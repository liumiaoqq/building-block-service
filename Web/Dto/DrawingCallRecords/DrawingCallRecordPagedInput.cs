namespace Web
{
    public class DrawingCallRecordPagedInput : PagedBaseInput
    {
        [Description("用户")]
        public Guid? UserId { get; set; }

        [Description("用户账号")]
        public string UserName { get; set; }

        [Description("用户邮箱")]
        public string Email { get; set; }

        [Description("画图类型")]
        public DrawingType? DrawingType { get; set; }

        [Description("是否成功")]
        public bool? IsSuccess { get; set; }

        [Description("开始时间")]
        public DateTime? StartTime { get; set; }

        [Description("结束时间")]
        public DateTime? EndTime { get; set; }
    }
}

