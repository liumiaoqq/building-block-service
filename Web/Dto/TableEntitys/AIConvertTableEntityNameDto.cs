
namespace Web.Dto.TableEntitys
{
    public class AIConvertTableEntityNameDto
    {
        [Description("计划ID")]
        public Guid PlanId { get; set; }


        [Description("实体ID")]
        public Guid TableEntityId { get; set; }

        [Description("实体名称")]
        public string Name { get; set; }

        [Description("实体编码")]
        public string Code { get; set; }


        [Description("新编码")]
        public string NewCode { get; set; }


    }
}
