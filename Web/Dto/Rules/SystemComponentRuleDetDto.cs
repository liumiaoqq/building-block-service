namespace Web.Dto.Rules
{
    public class SystemComponentRuleDetDto:FullBaseDto
    {
        [Description("键")]
        public string Key { get; set; }



        [Description("描述")]
        public string Desc { get; set; }

        [Description("系统组件规则Id")]
        public Guid SystemComponentRuleId { get; set; }


        [Description("顺序")]
        public int Sort { get; set; }
    }
}
