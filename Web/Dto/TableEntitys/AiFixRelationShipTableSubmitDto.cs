
namespace Web.Dto.TableEntitys
{
    public class AiFixRelationShipTableSubmitDto
    {

        [Description("计划ID")]
        public Guid PlanId { get; set; }

        [Description("表关系列表")]
        public List<AiFixRelationShipTableDto> RelationShipTables { get; set; }

    }
}
