namespace Web.Tables
{
    /// <summary>
    /// 系统配置
    /// </summary>
    [YoungTable("SystemConfig")]
    public class SystemConfig : CreationAuditedAggregateRoot
    {
        /// <summary>
        /// 是否开启审计
        /// </summary>
        [Description("是否开启审计")]
        public bool IsAuditEnabled { get; set; }
    }
}
