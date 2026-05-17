

using Web.Dto.Warehouses;

namespace Web.Dto.Plans
{
    public class PlanDto : FullBaseDto
    {
        [Description("方案名称")]
        public string PlanName { get; set; }
        [Description("数据库连接")]
        public string DatabaseConnection { get; set; }

        [Description("后端文件名称")]
        public string BackFileName { get; set; }

        [Description("后端对应语言")]
        public LanguageWay BackLanguageWay { get; set; }

        public SqlTempleteDto SqlTemplete { get; set; }

        public Guid? SqlTempleteId { get; set; }


        [Description("文件名称")]
        public string FileName { get; set; }


        [Description("方案类型")]
        public PlanType? PlanType { get; set; }


        [Description("是否模板")]
        public bool IsTemplete { get; set; }

        public string PlanTypeFormat => PlanType.HasValue ? PlanType.Value.ToDescription() : "无";


        public List<SystemModuleDto> SystemModules { get; set; }

        public List<TableEntityDto> TableEntitys { get; set; }

        public List<PlanEnumDto> PlanEnums { get; set; }


        public Guid? WarehouseId { get; set; }

        public WarehouseDto WarehouseDto { get; set; }

        [Description("是否小程序")]
        public bool IsMiniProgram { get; set; }

        [Description("后端端口")]
        public int? BackPort { get; set; }



        public PlanDto()
        {
            SystemModules = new List<SystemModuleDto>();
            TableEntitys = new List<TableEntityDto>();
            PlanEnums = new List<PlanEnumDto>();
        }

    }
}
