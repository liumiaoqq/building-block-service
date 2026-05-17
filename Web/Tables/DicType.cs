namespace Web.Tables
{
    [Description("字典类型")]
    [YoungTable("DicType")]
    public class DicType : CreationAuditedAggregateRoot
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public int Sort { get; set; }

        public string Remark { get; set; }

        public bool IsSystem { get; set; }
    }
}
