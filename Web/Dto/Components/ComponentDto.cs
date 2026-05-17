using Web.Dto.Components;
using Web.Dto.Plans;

namespace Web.Dto
{
    public class ComponentDto : FullBaseDto
    {
        [Description("组件类型")]
        [Obsolete]
        public ComponentType Type { get; set; }

        [Description("组件名")]
        public string Name { get; set; }

        [Description("对应语言")]
        public LanguageWay? LanguageWay { get; set; }


        [Description("对应模块")]
        public Guid ModuleId { get; set; } 


        [Description("配置规则")]
        public List<SystemComponentSettingDetDto> ComponentSettingDetDtos = new List<SystemComponentSettingDetDto>();


        public SystemModuleDto SystemModule { get; set; }

        public List<ComponentTempleteDto> ComponentTempletes { get; set; }

        public ComponentDto()
        {
            ComponentTempletes = new List<ComponentTempleteDto>();
        }
    }
}
