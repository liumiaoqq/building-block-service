namespace Web.Tables
{
    /// <summary>
    /// 代码管理表
    /// </summary>
    [Description("代码管理")]
    [YoungTable("CodeManagement")]
    public class CodeManagement : CreationAuditedAggregateRoot
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        [Description("项目名称")]
        [SugarColumn(ColumnDataType = "varchar(200)", IsNullable = false)]
        public string ProjectName { get; set; }

        /// <summary>
        /// 项目描述
        /// </summary>
        [Description("项目描述")]
        [SugarColumn(ColumnDataType = "varchar(500)", IsNullable = true)]
        public string Description { get; set; }

        /// <summary>
        /// 文件总数
        /// </summary>
        [Description("文件总数")]
        [SugarColumn(IsNullable = false)]
        public int TotalFileCount { get; set; }

        /// <summary>
        /// 总大小（字节）
        /// </summary>
        [Description("总大小")]
        [SugarColumn(IsNullable = false)]
        public long TotalSize { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Description("备注")]
        [SugarColumn(ColumnDataType = "varchar(500)", IsNullable = true)]
        public string Remark { get; set; }
    }
}

