namespace Web.Tables
{
    [Description("画图调用记录")]
    [YoungTable("DrawingCallRecord")]
    public class DrawingCallRecord : CreationAuditedAggregateRoot
    {
        [Description("用户")]
        public Guid UserId { get; set; }

        [Description("画图类型")]
        public DrawingType DrawingType { get; set; }

        [Description("调用时间")]
        public DateTime CallTime { get; set; }

        [Description("是否成功")]
        public bool IsSuccess { get; set; }

        [Description("失败原因")]
        public string FailReason { get; set; }
    }
}

