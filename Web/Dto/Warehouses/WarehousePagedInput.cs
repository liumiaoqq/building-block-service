namespace Web.Dto.Warehouses
{
    public class WarehousePagedInput : PagedBaseInput
    {
        public string Name { get; set; }

        public Guid? Id { get; set; }
    }
}
