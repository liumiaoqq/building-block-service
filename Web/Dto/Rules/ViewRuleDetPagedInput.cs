namespace Web.Dto.Rules
{
    public class ViewRuleDetPagedInput : PagedBaseInput
    {
        public Guid? Id { get; set; }

        public Guid? ViewRuleId { get; set; }
    }
}
