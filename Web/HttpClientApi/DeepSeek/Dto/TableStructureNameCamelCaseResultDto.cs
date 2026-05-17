namespace Web.Dto.TableEntitys
{
    /// <summary>
    /// 功能清单
    /// </summary>
    public class TableStructureNameCamelCaseResultDto
    {
        [Description("表名")]
        public string TableName { get; set; }

        [Description("新表名")]
        public string NewTableName { get; set; }


    }

    public class TableStructureNameCamelCaseResultDtoList
    {
        public List<TableStructureNameCamelCaseResultDto> Tables { get; set; }
    }
}
