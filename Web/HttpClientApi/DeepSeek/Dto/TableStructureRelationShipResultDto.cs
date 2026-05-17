namespace Web.Dto.TableEntitys
{
    /// <summary>
    /// 表结构关系结果
    /// </summary>
    public class TableStructureRelationShipResultDto

    {

        [Description("表编码")]
        public string TableCode { get; set; }

        [Description("表名")]
        public string TableName { get; set; }





        [Description("引用表编码")]
        public string RefTableCode { get; set; }

        [Description("引用表名")]
        public string RefTableName { get; set; }

        [Description("引用列编码")]
        public string RefColumnCode { get; set; }

        [Description("引用列名称")]
        public string RefColumnName { get; set; }

    }

    public class TableStructureRelationShipResultDtoList
    {
        public List<TableStructureRelationShipResultDto> Tables { get; set; }
    }

}
