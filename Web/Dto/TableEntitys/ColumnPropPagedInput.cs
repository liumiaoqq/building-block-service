namespace Web.Dto.TableEntitys
{
    public class ColumnPropPagedInput : PagedBaseInput
    {
        public Guid? TableEntityId { get; set; }

        public Guid? PlanId { get; set; }

        public Guid? UserId { get; set; }

        public List<Guid> TableEntityIds { get; set; }

    }
}
