namespace Web.Tables
{
    /// <summary>
    /// 仓库
    /// </summary>
    [YoungTable("Warehouse")]
    public class Warehouse : CreationAuditedAggregateRoot
    {
        [Description("仓库名称")]
        public string Name { get; set; }


        [Description("仓库作用")]
        public string Content { get; set; }


    }
}
