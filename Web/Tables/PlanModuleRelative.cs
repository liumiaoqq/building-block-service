namespace Web.Tables
{
    [Description("计划模块")]
    [YoungTable("PlanModule")]
    public class PlanModuleRelative : CreationAuditedAggregateRoot
    {
        public Guid PlanId { get; set; }

        public Guid ModuleId { get; set; }
    }
}
