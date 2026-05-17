namespace Web.Dto.Rules
{
    public class ExportRuleDto : FullBaseDto
    {
        /// <summary>
        /// 策略名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        public List<ExportRuleDetDto> RuleDets { get; set; }
    }

}
