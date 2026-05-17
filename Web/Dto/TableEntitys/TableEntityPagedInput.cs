namespace Web.Dto.TableEntitys
{
    public class TableEntityPagedInput : PagedBaseInput
    {

        public Guid? PlanId { get; set; }


        public List<Guid> PlanIds { get; set; }

        public Guid? TableEntityId { get; set; }

        public Guid? NewPlanId { get; set; }

        public bool IsReset { get; set; }

        public Guid? UserId { get; set; }


        public List<Guid> TableEntityIds { get; set; } = new List<Guid>();

        /// <summary>
        /// 主显示列字典，key为表Code，value为主显示列Code
        /// </summary>
        public Dictionary<string, string> PrimaryDisplayColumns { get; set; }

    }
}
