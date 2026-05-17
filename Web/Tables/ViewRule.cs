namespace Web.Tables
{

    [Description("视图策略")]
    [YoungTable("ViewRule")]
    public class ViewRule : CreationAuditedAggregateRoot
    {
        /// <summary>
        /// 策略名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }
}
