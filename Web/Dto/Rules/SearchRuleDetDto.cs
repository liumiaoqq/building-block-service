namespace Web.Dto.Rules
{
    public class SearchRuleDetDto : FullBaseDto
    {
        public Guid SearchRuleId { get; set; }

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
        /// 视图匹配属性规则
        /// </summary>
        public SearchRuleMatchType SearchRuleMatchType { get; set; }

        public string SearchRuleMatchTypeFormat => SearchRuleMatchType.ToDescription();

        /// <summary>
        /// 匹配后处理
        /// </summary>
        public SearchType SearchType { get; set; }

        public string SearchTypeFormat => SearchType.ToDescription();
    }
}
