namespace Web
{
    public class IntegralRecordPagedInput : PagedBaseInput
    {
        [Description("用户")]
        public Guid? UserId { get; set; }

        [Description("类型")]
        public IntegralRecordType? Type { get; set; }
        [Description("来源单号")]
        public string SourceOrderNo { get; set; }
    }
}
