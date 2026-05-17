using System.Numerics;
using Web.DB;

namespace Web.Tables
{

    /// <summary>
    /// 项目案例
    /// </summary>
    [YoungTable("ProjectCase")]
    public class ProjectCase : CreationAuditedAggregateRoot, IWarehouseId
    {
        [Description("案例名称")]
        public string CaseName { get; set; }

        [Description("案例描述")]
        public string CaseDescription { get; set; }


        [Description("封面")]
        public string Cover { get; set; }


        [Description("内容")]
        public string Content { get; set; }


        [Description("是否公开")]
        public bool IsPublic { get; set; }
        public Guid? WarehouseId { get; set; }

        [Description("案例类型")]
        public ProjectCaseType? CaseType { get; set; }

        public Guid? PlanId { get; set; }

        [Description("排序")]
        public int? Sort { get; set; }

        [Description("哔哩哔哩视频地址")]
        public string BilibiliUrl { get; set; }

        [Description("哔哩哔哩视频iframe地址")]
        public string BilibiliVideoIframeUrl { get; set; }

        [Description("浏览次数")]
        public int ViewCount { get; set; }
    }
}
