
namespace Web.Dto.TableEntitys
{
    /// <summary>
    /// AI绑定枚举 - 列DTO
    /// </summary>
    public class AiBindEnumColumnDto
    {
        [Description("列ID")]
        public Guid ColumnPropId { get; set; }

        [Description("列名称")]
        public string Name { get; set; }

        [Description("列编码")]
        public string Code { get; set; }

        [Description("列类型")]
        public string ColumnType { get; set; }

        [Description("匹配的枚举ID")]
        public Guid? MatchedEnumId { get; set; }

        [Description("匹配的枚举编码")]
        public string MatchedEnumCode { get; set; }

        [Description("匹配的枚举名称")]
        public string MatchedEnumName { get; set; }
    }
}

