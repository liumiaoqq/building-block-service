namespace Web.Dto.PlanModuleRelatives
{
    public class BatchCreateOrEditPlanModuleRelativesInput
    {

        public Guid PlanId { get; set; }

        public List<Guid> ModuleIds { get; set; }
    }
}
