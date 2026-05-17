namespace Web.Dto.Plans
{
    public class ProjectCasePagedInput : PagedBaseInput
    {
        [Description("案例名称")]
        public string? CaseName { get; set; }

        public Guid? Id { get; set; }

        [Description("案例类型")]
        public ProjectCaseType? CaseType { get; set; }

        public Guid? PlanId { get; set; }


        [Description("关键字")]
        public string KeyWord { get; set; }
    }
}
