namespace Web.Dto.Rules
{
    public class SearchRuleDto : FullBaseDto
    {
        /// <summary>
        /// 规则名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        public List<SearchRuleDetDto> RuleDets { get; set; }
    }
}
