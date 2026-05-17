namespace Web.Dto.TableEntitys
{
    /// <summary>
    /// 主显示列选择请求参数
    /// </summary>
    public class PrimaryDisplayColumnRequestDto
    {
        [Description("表编码")]
        public string TableCode { get; set; }

        [Description("表名称")]
        public string TableName { get; set; }

        [Description("字符串列编码，逗号分隔")]
        public string Columns { get; set; }
    }

    /// <summary>
    /// 主显示列选择结果
    /// </summary>
    public class PrimaryDisplayColumnResultDto
    {
        [Description("表编码")]
        public string TableCode { get; set; }

        [Description("主显示列编码")]
        public string PrimaryColumnCode { get; set; }
    }

    public class PrimaryDisplayColumnResultDtoList
    {
        public List<PrimaryDisplayColumnResultDto> Tables { get; set; }
    }
}
