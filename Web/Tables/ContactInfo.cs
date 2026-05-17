namespace Web.Tables
{
    [Description("联系方式")]
    [YoungTable("ContactInfo")]
    public class ContactInfo : CreationAuditedAggregateRoot
    {
        [Description("标题")]
        public string Title { get; set; }

        [Description("联系类型")]
        public string ContactType { get; set; }

        [Description("图片")]
        public string Image { get; set; }

        [Description("联系号码")]
        public string ContactNumber { get; set; }

        [Description("有效开始时间")]
        public string ValidStartTime { get; set; }

        [Description("有效结束时间")]
        public string ValidEndTime { get; set; }

        [Description("有效开始日期")]
        public DateTime? ValidStartDate { get; set; }

        [Description("有效结束日期")]
        public DateTime? ValidEndDate { get; set; }

        [Description("排序")]
        public int Sort { get; set; }

        [Description("是否启用")]
        public bool IsEnabled { get; set; }
    }
}
