using Web.Dto.Rules;
using Web.Dto.Warehouses;

namespace Web.Dto.Components
{
    public class SystemComponentRuleDto : FullBaseDto
    {

        [Description("名称")]
        public string Name { get; set; }



        [Description("对应语言")]
        public LanguageWay LanguageWay { get; set; }


        public string LanguageWayFormat => LanguageWay.ToDescription();

        public Guid? WarehouseId { get; set; }

        public WarehouseDto WarehouseDto { get; set; }


        public List<SystemComponentRuleDetDto> SystemComponentRuleDetDtos { get; set; }

    }
}
