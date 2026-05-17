namespace Web
{
    /// <summary>
    /// 检查并记录画图调用结果
    /// </summary>
    public class CheckAndRecordDrawingResult
    {
        [Description("是否有权限")]
        public bool HasPermission { get; set; }

        [Description("剩余次数")]
        public int RemainingCount { get; set; }

        [Description("每日最大次数")]
        public int MaxDailyCount { get; set; }

        [Description("消息")]
        public string Message { get; set; }
    }
}

