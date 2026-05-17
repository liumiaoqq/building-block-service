

namespace Web.Dto.TableEntitys
{
    [Description("AI自动解析枚举")]
    public class AiFixEnumBySqlScriptSubmitDto
    {

        [Description("计划")]
        public Guid PlanId { get; set; }

        [Description("SQL脚本")]
        public string SqlScript { get; set; }

    }
}
