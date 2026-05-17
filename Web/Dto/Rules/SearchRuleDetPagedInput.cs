namespace Web.Dto.Rules
{
    public class SearchRuleDetPagedInput : PagedBaseInput
    {
        public Guid? Id { get; set; }

        public Guid? SearchRuleId { get; set; }
    }
}
