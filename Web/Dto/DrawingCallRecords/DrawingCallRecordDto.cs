namespace Web
{
    public class DrawingCallRecordDto : FullBaseDto
    {
        [Description("用户")]
        public Guid UserId { get; set; }

        [Description("用户账号")]
        public string UserName { get; set; }

        [Description("用户邮箱")]
        public string Email { get; set; }

        [Description("画图类型")]
        public DrawingType DrawingType { get; set; }

        [Description("调用时间")]
        public DateTime CallTime { get; set; }

        [Description("是否成功")]
        public bool IsSuccess { get; set; }

        [Description("失败原因")]
        public string FailReason { get; set; }

        public string DrawingTypeFormat => DrawingType.ToDescription();
    }
}

