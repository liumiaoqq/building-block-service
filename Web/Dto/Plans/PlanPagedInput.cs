namespace Web.Dto.Plans
{
    public class PlanPagedInput : PagedBaseInput
    {
        public Guid? PlanId { get; set; }


        public Guid? UserId { get; set; }

        /// <summary>
        /// 多个模块Ids
        /// </summary>
        public Guid[] ModuleIds { get; set; } = new Guid[0];


        public LookType LookType { get; set; }

        public string PlanName { get; set; }

        public string FileName { get; set; }


        [Description("方案类型")]
        public PlanType? PlanType { get; set; }


        public List<PlanType> PlanTypeList { get; set; }


        public bool? IsTemplete { get; set; }


        public Guid? WarehouseId { get; set; }



        public string UserKeyWord { get; set; }



        /// <summary>
        /// 后置过滤关键词
        /// </summary>
        public List<string> PostFilterKeyWords { get; set; }



    }
}
