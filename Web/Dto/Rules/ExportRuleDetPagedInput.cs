namespace Web.Dto.Rules
{
    public class ExportRuleDetPagedInput : PagedBaseInput
    {
        public Guid? Id { get; set; }

        public Guid? ExportRuleId { get; set; }
    }
}
