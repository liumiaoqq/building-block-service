namespace Web.Tables
{
    [Description("视图规则")]
    [YoungTable("ViewRuleDet")]
    public class ViewRuleDet : CreationAuditedAggregateRoot
    {
        public Guid ViewRuleId { get; set; }

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
        public ViewRuleMatchType ViewRuleMatchType { get; set; }

        /// <summary>
        /// 匹配后处理
        /// </summary>
        public ViewColumnType ViewColumnType { get; set; }

    }
}
