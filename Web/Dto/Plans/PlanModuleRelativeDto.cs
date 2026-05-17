namespace Web.Dto.Plans
{
    public class PlanModuleRelativeDto : FullBaseDto
    {

        public Guid? PlanId { get; set; }

        public Guid? ModuleId { get; set; }
        public PlanDto PlanDto { get; set; }

        public SystemModuleDto SystemModuleDto { get; set; }
    }
}
