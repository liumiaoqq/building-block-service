namespace Web.Tables
{

    [Description("导出策略")]
    [YoungTable("ExportRule")]
    public class ExportRule : CreationAuditedAggregateRoot
    {
        /// <summary>
        /// 策略名称
        /// </summary>
        public string  Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }


    }
}
