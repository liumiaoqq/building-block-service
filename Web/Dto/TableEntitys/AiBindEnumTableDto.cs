
namespace Web.Dto.TableEntitys
{
    /// <summary>
    /// AI绑定枚举 - 表DTO
    /// </summary>
    public class AiBindEnumTableDto
    {
        [Description("表名")]
        public string TableName { get; set; }

        [Description("表编码")]
        public string TableCode { get; set; }

        [Description("表ID")]
        public Guid TableId { get; set; }

        [Description("列属性列表")]
        public List<AiBindEnumColumnDto> Columns { get; set; }
    }
}

