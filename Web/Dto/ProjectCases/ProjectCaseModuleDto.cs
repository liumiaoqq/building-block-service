using Microsoft.CodeAnalysis;
using Web.Dto.Warehouses;

namespace Web
{
    public class ProjectCaseModuleDto : FullBaseDto
    {
        [Description("模块名称")]
        public string ModuleName { get; set; }
        [Description("系统模块")]
        public Guid SystemModuleId { get; set; }

        [Description("项目案例")]
        public Guid ProjectCaseId { get; set; }

        [Description("排序")]
        public int Sort { get; set; }

        public Guid? WarehouseId { get; set; }



    }



}
