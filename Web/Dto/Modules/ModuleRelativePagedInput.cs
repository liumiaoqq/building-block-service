namespace Web.Dto.Modules
{
    public class ModuleRelativePagedInput : PagedBaseInput
    {
        public Guid PlanId { get; set; }

        public Guid? WarehouseId { get; set; }
    }
}
