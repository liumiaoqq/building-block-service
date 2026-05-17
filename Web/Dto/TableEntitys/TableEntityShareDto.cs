namespace Web.Dto.TableEntitys
{
    public class TableEntityShareDto
    {
        /// <summary>
        /// 计划名称
        /// </summary>
        public string PlanName { get; set; }
    
        public Guid PlanId { get; set; }

        public PlanType PlanType { get; set; }

        public List<Guid> CheckIds { get; set; }

        public int ShareCount { get; set; }

        public string PlanTypeFormat => PlanType.ToDescription();

        public List<TableEntityShareItemDto> TableEntityList { get; set; }


    }
}
