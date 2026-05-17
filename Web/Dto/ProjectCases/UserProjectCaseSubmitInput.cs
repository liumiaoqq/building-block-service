namespace Web
{
    public class UserProjectCaseSubmitInput
    {
        [Description("方案名称")]
        public string PlanName { get; set; }

        [Description("文件名称")]
        public string FileName { get; set; }

        [Description("选择的案例")]
        public Guid SelectProjectCaseId { get; set; }
    }
}
