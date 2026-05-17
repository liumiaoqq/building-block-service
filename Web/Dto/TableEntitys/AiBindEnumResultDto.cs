
namespace Web.Dto.TableEntitys
{
    /// <summary>
    /// AI绑定枚举 - 完整结果DTO
    /// </summary>
    public class AiBindEnumResultDto
    {
        [Description("表列表")]
        public List<AiBindEnumTableDto> Tables { get; set; }

        [Description("枚举列表")]
        public List<AiBindEnumInfoDto> Enums { get; set; }
    }
}

