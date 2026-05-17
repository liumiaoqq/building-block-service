using Microsoft.CodeAnalysis;
using Web.Dto.Plans;
using Web.Dto.Warehouses;

namespace Web
{
    public class UserProjectHighLightDto 
    {
        [Description("名称")]
        public string Name { get; set; }

        [Description("摘要")]
        public string Summary { get; set; }

        [Description("封面")]
        public string CoverImage { get; set; }

        [Description("功能介绍")]
        public string Content { get; set; }

        [Description("类型名称")]
        public string TypeName { get; set; }

        [Description("类型id")]
        public Guid? TypeId { get; set; }
    }

}
