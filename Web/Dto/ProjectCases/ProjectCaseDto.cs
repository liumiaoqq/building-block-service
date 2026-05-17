using Microsoft.CodeAnalysis;
using Web.Dto.Plans;
using Web.Dto.Warehouses;

namespace Web
{
    public class ProjectCaseDto : FullBaseDto
    {
        [Description("案例名称")]
        public string CaseName { get; set; }

        [Description("案例描述")]
        public string CaseDescription { get; set; }


        [Description("封面")]
        public string Cover { get; set; }

        [Description("案例类型")]
        public ProjectCaseType? CaseType { get; set; }

        public string CaseTypeDesc => CaseType.HasValue ? CaseType.Value.ToDescription() : "无";


        [Description("内容")]
        public string Content { get; set; }


        [Description("是否公开")]
        public bool IsPublic { get; set; }
        public Guid? WarehouseId { get; set; }

        public WarehouseDto WarehouseDto { get; set; }



        [Description("哔哩哔哩视频iframe地址")]
        public string BilibiliVideoIframeUrl { get; set; }

        public int? Sort { get; set; }


        public Guid? PlanId { get; set; }

        public PlanDto PlanDto { get; set; }

        public string BilibiliUrl { get; set; }

        [Description("浏览次数")]
        public int ViewCount { get; set; }

        


    }

}
