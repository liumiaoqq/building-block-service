using Web.Dto.Components;
using Web.Dto.Warehouses;

namespace Web.Dto.Plans
{
    public class SystemModuleDto : FullBaseDto
    {
        [Description("模块名称")]
        public string Name { get; set; }

        [Description("模块标签")]
        public string Label { get; set; }


        public Guid PlanId { get; set; }


        public Guid? WarehouseId { get; set; }

        public WarehouseDto WarehouseDto { get; set; }

        public List<ComponentDto> Components { get; set; }


        [Description("系统模块类型")]

        public SystemModuleType SystemModuleType { get; set; }

        public List<string> Labels => Label.IsNullOrWhiteSpace() ? new List<string>() : Label.JoinAsList(",").OrderBy(x=>x).ToList();


        public string SortLabel => Labels.JoinAsString(",");

        public string SystemModuleTypeFormat => SystemModuleType.ToDescription();

        public SystemModuleDto()
        {
            Components = new List<ComponentDto>();
        }
    }
}
