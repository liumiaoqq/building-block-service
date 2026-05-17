namespace Web.Tables
{
    [Description("字典值")]
    [YoungTable("DicCode")]
    public class DicCode : CreationAuditedAggregateRoot
    {
        public Guid DicTypeId { get; set; }

        public string Name { get; set; }
        public string Code { get; set; }

        public int Sort { get; set; }

        public string Remark { get; set; }

        public bool IsSystem { get; set; }
    }
}
