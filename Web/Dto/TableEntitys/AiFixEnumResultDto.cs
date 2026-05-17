

namespace Web.Dto.TableEntitys
{
    [Description("AI自动解析枚举结果")]
    public class AiFixEnumResultDto
    {

        [Description("计划")]
        public Guid PlanId { get; set; }

        [Description("列名称")]
        public string Name { get; set; }

        [Description("列编码")]
        public string Code { get; set; }

        [Description("枚举值列表")]
        public List<AiFixEnumDtoList> EnumPropsList { get; set; }

    }

    public class AiFixEnumDtoList
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }
}
