namespace Web.Dto.TableEntitys
{
    /// <summary>
    /// 表结构长度调整结果
    /// </summary>
    public class TableStructureLengthAdjustmentResultDto

    {

        [Description("表编码")]
        public string TableCode { get; set; }

        [Description("表名")]
        public string TableName { get; set; }


        [Description("列编码")]
        public string ColumnCode { get; set; }

        [Description("列名")]
        public string ColumnName { get; set; }

        [Description("新长度")]
        public string NewLength { get; set; }

        [Description("旧长度")]
        public string OldLength { get; set; }


    }

    public class TableStructureLengthAdjustmentResultDtoList
    {
        public List<TableStructureLengthAdjustmentResultDto> Columns { get; set; }
    }
}
