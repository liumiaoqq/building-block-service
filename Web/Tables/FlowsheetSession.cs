namespace Web.Tables
{
    /// <summary>
    /// 流程图会话表
    /// </summary>
    [Description("流程图会话")]
    [YoungTable("FlowsheetSession")]
    public class FlowsheetSession : CreationAuditedAggregateRoot
    {
        /// <summary>
        /// 会话名称
        /// </summary>
        [Description("会话名称")]
        [SugarColumn(ColumnDataType = "varchar(200)", IsNullable = false)]
        public string Name { get; set; }

        /// <summary>
        /// 代码管理ID（关联的代码片段项目，可选）
        /// </summary>
        [Description("代码管理ID")]
        [SugarColumn(IsNullable = true)]
        public Guid? CodeManagementId { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        [Description("是否激活")]
        [SugarColumn(IsNullable = false)]
        public bool IsActive { get; set; }

        /// <summary>
        /// 流程图内容（JSON格式存储）
        /// </summary>
        [Description("流程图内容")]
        [SugarColumn(ColumnDataType = "text", IsNullable = true)]
        public string FlowsheetContent { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Description("备注")]
        [SugarColumn(ColumnDataType = "varchar(500)", IsNullable = true)]
        public string Remark { get; set; }
    }
}

