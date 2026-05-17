namespace Web.Dto.Rules
{
    public class ExportRuleDetDto : FullBaseDto
    {
        public Guid ExportRuleId { get; set; }

        public ExportRuleDto ExportRuleDto { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// 执行顺序(从大到小)
        /// </summary>
        public int Sort { get; set; }


        /// <summary>
        /// 匹配正则表达式
        /// </summary>
        public string MatchRegular { get; set; }


        /// <summary>
        /// 是否匹配为外键
        /// </summary>
        public bool IsMatchForeignKey { get; set; }


        /// <summary>
        /// 匹配属性规则
        /// </summary>
        public ExportRuleMatchType ExportRuleMatchType { get; set; }

        /// <summary>
        /// 匹配后处理
        /// </summary>
        public ExportRuleDispatchType ExportRuleDispatchType { get; set; }


        public string ExportRuleMatchTypeFormat => ExportRuleMatchType.ToDescription(); 

        public string ExportRuleDispatchTypeFormat => ExportRuleDispatchType.ToDescription();


    }
}
