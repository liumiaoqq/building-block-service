namespace Web.Dto.Components
{
    public class SystemComponentRulePagedInput : PagedBaseInput
    {
        public Guid? WarehouseId { get; set; }

        [Description("对应语言")]
        public LanguageWay? LanguageWay { get; set; }

        public Guid? Id { get; set; }


    }
}
