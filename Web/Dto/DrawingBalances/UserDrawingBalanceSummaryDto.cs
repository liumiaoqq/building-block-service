namespace Web
{
    /// <summary>
    /// 用户画图余额汇总
    /// </summary>
    public class UserDrawingBalanceSummaryDto
    {
        [Description("ER图今日剩余次数")]
        public int ERDiagramRemainingToday { get; set; }

        [Description("ER图每日最大次数")]
        public int ERDiagramMaxDaily { get; set; }

        [Description("流程图今日剩余次数")]
        public int FlowchartRemainingToday { get; set; }

        [Description("流程图每日最大次数")]
        public int FlowchartMaxDaily { get; set; }

        [Description("时序图今日剩余次数")]
        public int SequenceDiagramRemainingToday { get; set; }

        [Description("时序图每日最大次数")]
        public int SequenceDiagramMaxDaily { get; set; }

        [Description("用例图今日剩余次数")]
        public int UseCaseDiagramRemainingToday { get; set; }

        [Description("用例图每日最大次数")]
        public int UseCaseDiagramMaxDaily { get; set; }

        [Description("三线表今日剩余次数")]
        public int ThreeLineTableRemainingToday { get; set; }

        [Description("三线表每日最大次数")]
        public int ThreeLineTableMaxDaily { get; set; }

        [Description("项目结构图今日剩余次数")]
        public int ProjectStructureDiagramRemainingToday { get; set; }

        [Description("项目结构图每日最大次数")]
        public int ProjectStructureDiagramMaxDaily { get; set; }
    }
}

