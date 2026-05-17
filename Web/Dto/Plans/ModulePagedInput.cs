using Web.DB;

namespace Web.Dto.Plans
{
    public class ModulePagedInput : PagedBaseInput, IWarehouseId
    {
        public List<Guid> ModuleIds { get; set; }

        public List<Guid> PlanIds { get; set; }

        public Guid? PlanId { get; set; }

        public SystemModuleType? SystemModuleType { get; set; }

        public List<SystemModuleType> SystemModuleTypeList { get; set; }


        public string Name { get; set; }

        public string Label { get; set; }
        public Guid? WarehouseId { get; set; }
    }
}
