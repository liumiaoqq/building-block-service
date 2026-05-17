namespace Web.Dto.Rules
{
    public class EditRuleDetPagedInput : PagedBaseInput
    {
        public Guid? Id { get; set; }

        public Guid? EditRuleId { get; set; }
    }
}
