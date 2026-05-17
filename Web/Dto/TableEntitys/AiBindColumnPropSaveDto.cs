
namespace Web.Dto.TableEntitys
{
    /// <summary>
    /// AI绑定列类型 - 保存提交DTO
    /// </summary>
    public class AiBindColumnPropSaveDto
    {
        [Description("方案ID")]
        public Guid PlanId { get; set; }

        [Description("表列表")]
        public List<AiBindColumnPropTableDto> Tables { get; set; }
    }
}

