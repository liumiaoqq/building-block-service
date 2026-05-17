namespace Web.Dto.TableEntitys
{
    public class TableNavigateRelativePagedInput:PagedBaseInput
    {
        public Guid? RelativeTableId { get; set; }

        public Guid? PlanId { get; set; }

        public Guid? UserId { get; set; }
    }
}
