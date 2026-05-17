namespace Web.Dto.TableEntitys
{
    /// <summary>
    /// 表列排序结果
    /// </summary>
    public class TableColumnSortResultDto
    {
        [Description("列编码")]
        public string ColumnCode { get; set; }

        [Description("列名")]
        public string ColumnName { get; set; }

        [Description("新排序")]
        public int NewSort { get; set; }

        [Description("建议理由")]
        public string Reason { get; set; }
    }

    public class TableColumnSortTableResultDto
    {
        [Description("表名")]
        public string TableName { get; set; }

        [Description("列列表")]
        public List<TableColumnSortResultDto> Columns { get; set; }
    }

    public class TableColumnSortResultDtoList
    {
        public List<TableColumnSortTableResultDto> Tables { get; set; }
    }
}
