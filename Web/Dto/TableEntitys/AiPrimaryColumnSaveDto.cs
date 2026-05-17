
namespace Web.Dto.TableEntitys
{
    /// <summary>
    /// AI识别主要列 - 保存提交DTO
    /// </summary>
    public class AiPrimaryColumnSaveDto
    {
        [Description("方案ID")]
        public Guid PlanId { get; set; }

        [Description("表列表")]
        public List<AiPrimaryColumnTableDto> Tables { get; set; }
    }
}
