

namespace Web.Dto.TableEntitys
{
    [Description("AI自动解析枚举保存")]
    public class AiFixEnumSaveDto
    {
        [Description("计划")]
        public Guid PlanId { get; set; }

        [Description("枚举列表")]
        public List<AiFixEnumResultDto> AiFixEnumResultDtos { get; set; }
    }



}
