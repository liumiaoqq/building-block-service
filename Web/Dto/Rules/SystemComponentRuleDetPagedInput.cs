namespace Web.Dto.Rules
{
    public class SystemComponentRuleDetPagedInput : PagedBaseInput
    {
        public Guid? Id { get; set; }


        [Description("系统组件规则Id")]
        public Guid? SystemComponentRuleId { get; set; }
    }
}
