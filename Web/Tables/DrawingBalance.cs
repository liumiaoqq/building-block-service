namespace Web.Tables
{
    [Description("用户画图余额")]
    [YoungTable("DrawingBalance")]
    public class DrawingBalance : CreationAuditedAggregateRoot
    {
        [Description("用户")]
        public Guid UserId { get; set; }

        [Description("画图类型")]
        public DrawingType DrawingType { get; set; }

        [Description("生效时间")]
        public DateTime EffectiveTime { get; set; }

        [Description("截至时间")]
        public DateTime ExpirationTime { get; set; }

        [Description("天最大生成次数")]
        public int MaxDailyGenerations { get; set; }

        [Description("是否启用")]
        public bool IsEnabled { get; set; }
    }
}

