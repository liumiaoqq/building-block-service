namespace Web.Dto.Rules
{
    public class ViewRuleDto : FullBaseDto
    {
        /// <summary>
        /// 策略名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        public List<ViewRuleDetDto> RuleDets { get; set; }
    }
}
