
namespace Web.Dto.TableEntitys
{
    /// <summary>
    /// AI绑定枚举 - 保存提交DTO
    /// </summary>
    public class AiBindEnumSaveDto
    {
        [Description("方案ID")]
        public Guid PlanId { get; set; }

        [Description("表列表")]
        public List<AiBindEnumTableDto> Tables { get; set; }
    }
}

