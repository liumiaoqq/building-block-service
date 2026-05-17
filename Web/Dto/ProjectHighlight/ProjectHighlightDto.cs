using Microsoft.CodeAnalysis;
using Web.Dto.Plans;
using Web.Dto.Warehouses;

namespace Web
{
    public class ProjectHighlightDto : FullBaseDto
    {
        [Description("名称")]
        public string Name { get; set; }
        [Description("摘要")]
        public string Summary { get; set; }
        [Description("类型")]
        public Guid? TypeId { get; set; }

        [Description("封面")]
        public string CoverImage { get; set; }

        [Description("功能介绍")]
        public string Content { get; set; }

        [Description("排序")]
        public int Sort { get; set; }

        [Description("类型名称")]
        public ProjectHighlightTypeDto TypeDto { get; set; }
    }

}
