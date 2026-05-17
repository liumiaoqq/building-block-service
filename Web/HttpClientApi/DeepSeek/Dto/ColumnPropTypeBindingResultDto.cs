namespace Web.Dto.TableEntitys
{
    /// <summary>
    /// DeepSeek 列类型绑定结果
    /// </summary>
    public class ColumnPropTypeBindingResultDto
    {
        [Description("表编码")]
        public string TableCode { get; set; }

        [Description("列编码")]
        public string ColumnCode { get; set; }

        [Description("建议的类型")]
        public string SuggestedType { get; set; }
    }

    /// <summary>
    /// DeepSeek 列类型绑定结果列表
    /// </summary>
    public class ColumnPropTypeBindingResultDtoList
    {
        [Description("匹配结果列表")]
        public List<ColumnPropTypeBindingResultDto> Bindings { get; set; }
    }
}

