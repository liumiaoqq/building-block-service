namespace Web.Dto.Plans
{
    public class UserPlanTempleteSubmitInput
    {
        [Description("方案名称")]
        public string PlanName { get; set; }

        [Description("文件名称")]
        public string FileName { get; set; }

        [Description("选中的方案")]
        public Guid SelectPlanId { get; set; }
    }
}
