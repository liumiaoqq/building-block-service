namespace Web.Tables
{
    /// <summary>
    /// 代码片段表
    /// </summary>
    [Description("代码片段")]
    [YoungTable("CodeSnippet")]
    public class CodeSnippet : CreationAuditedAggregateRoot
    {
        /// <summary>
        /// 代码管理ID（外键）
        /// </summary>
        [Description("代码管理ID")]
        [SugarColumn(IsNullable = false)]
        public Guid CodeManagementId { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        [Description("文件名称")]
        [SugarColumn(ColumnDataType = "varchar(200)", IsNullable = false)]
        public string FileName { get; set; }

        /// <summary>
        /// 文件路径（相对路径）
        /// </summary>
        [Description("文件路径")]
        [SugarColumn(ColumnDataType = "varchar(500)", IsNullable = false)]
        public string FilePath { get; set; }

        /// <summary>
        /// 上级片段ID（父节点ID，根节点为空）
        /// </summary>
        [Description("上级片段ID")]
        [SugarColumn(IsNullable = true)]
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 是否文件夹
        /// </summary>
        [Description("是否文件夹")]
        [SugarColumn(IsNullable = false)]
        public bool IsFolder { get; set; }

        /// <summary>
        /// 文件内容（如果是文件夹则为空）
        /// </summary>
        [Description("文件内容")]
        [SugarColumn(ColumnDataType = "text", IsNullable = true)]
        public string FileContent { get; set; }

        /// <summary>
        /// 文件大小（字节）
        /// </summary>
        [Description("文件大小")]
        [SugarColumn(IsNullable = false)]
        public long FileSize { get; set; }

        /// <summary>
        /// 文件扩展名
        /// </summary>
        [Description("文件扩展名")]
        [SugarColumn(ColumnDataType = "varchar(50)", IsNullable = true)]
        public string FileExtension { get; set; }

        /// <summary>
        /// 文件类型（JavaScript、Vue、CSS等）
        /// </summary>
        [Description("文件类型")]
        [SugarColumn(ColumnDataType = "varchar(50)", IsNullable = true)]
        public string FileType { get; set; }

        /// <summary>
        /// 层级深度
        /// </summary>
        [Description("层级深度")]
        [SugarColumn(IsNullable = false)]
        public int Level { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        [Description("排序号")]
        [SugarColumn(IsNullable = false)]
        public int SortOrder { get; set; }
    }
}

