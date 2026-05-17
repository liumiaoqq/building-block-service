namespace Web.Dto.TableEntitys
{
    public class TableEntityShareItemDto
    {


        public Guid Id { get; set; }


        [Description("关联次数")]
        public int RelativeCount { get; set; }

        [Description("实体名称")]
        public string Name { get; set; }

        [Description("实体编码")]
        public string Code { get; set; }

        [Description("对应计划")]
        public Guid PlanId { get; set; }

        [Description("是否禁用")]
        public bool IsForbidden { get; set; }
    }
}
