namespace Web.Dto.Plans
{
    public class PlanEnumPagedInput : PagedBaseInput
    {
        public Guid? Id { get; set; }
        public Guid? PlanId { get; set; }

        public Guid? UserId { get; set; }
    }
}
