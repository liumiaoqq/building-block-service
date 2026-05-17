
namespace Web.Dto.TableEntitys
{
    /// <summary>
    /// AI绑定列类型 - 列DTO
    /// </summary>
    public class AiBindColumnPropColumnDto
    {
        [Description("列ID")]
        public Guid ColumnPropId { get; set; }

        [Description("列名称")]
        public string Name { get; set; }

        [Description("列编码")]
        public string Code { get; set; }

        [Description("当前列类型")]
        public string CurrentType { get; set; }

        [Description("建议的列类型")]
        public string SuggestedType { get; set; }

        [Description("建议的列类型枚举值")]
        public int? SuggestedTypeValue { get; set; }
    }
}

