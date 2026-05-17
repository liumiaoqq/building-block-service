namespace Web.Dto.Modules
{
    public class CopyModuleInput
    {
        public List<Guid> ModuleIds { get; set; }

        public Guid WarehouseId { get; set; }
    }
}
