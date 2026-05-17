namespace Web.Dto.TableEntitys
{
    /// <summary>
    /// DeepSeek 枚举绑定结果
    /// </summary>
    public class EnumBindingResultDto
    {
        [Description("表编码")]
        public string TableCode { get; set; }

        [Description("列编码")]
        public string ColumnCode { get; set; }

        [Description("匹配的枚举编码")]
        public string MatchedEnumCode { get; set; }
    }

    /// <summary>
    /// DeepSeek 枚举绑定结果列表
    /// </summary>
    public class EnumBindingResultDtoList
    {
        [Description("匹配结果列表")]
        public List<EnumBindingResultDto> Bindings { get; set; }
    }
}

