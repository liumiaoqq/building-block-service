
namespace Web.Dto.TableEntitys
{
    /// <summary>
    /// AI绑定枚举 - 枚举信息DTO
    /// </summary>
    public class AiBindEnumInfoDto
    {
        [Description("枚举ID")]
        public Guid EnumId { get; set; }

        [Description("枚举名称")]
        public string Name { get; set; }

        [Description("枚举编码")]
        public string Code { get; set; }
    }
}

