namespace Web.Tables
{
    /// <summary>
    /// ER图会话表
    /// </summary>
    [Description("ER图会话")]
    [YoungTable("ErDiagramSession")]
    public class ErDiagramSession : CreationAuditedAggregateRoot
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
        /// 完整表SQL（用于生成ER图的SQL语句）
        /// </summary>
        [Description("完整表SQL")]
        [SugarColumn(ColumnDataType = "text", IsNullable = true)]
        public string CompleteSql { get; set; }

        /// <summary>
        /// 表关系JSON字符串（存储表之间的关系数据）
        /// </summary>
        [Description("表关系JSON")]
        [SugarColumn(ColumnDataType = "text", IsNullable = true)]
        public string TableRelationJson { get; set; }

        /// <summary>
        /// 用户提示词（用户输入的需求描述）
        /// </summary>
        [Description("用户提示词")]
        [SugarColumn(ColumnDataType = "text", IsNullable = true)]
        public string UserPrompt { get; set; }

        /// <summary>
        /// AI输入完整内容（发送给AI的完整提示词，包含上下文）
        /// </summary>
        [Description("AI输入完整内容")]
        [SugarColumn(ColumnDataType = "text", IsNullable = true)]
        public string AiInputContent { get; set; }

        /// <summary>
        /// AI输出的结果（AI返回的原始结果）
        /// </summary>
        [Description("AI输出结果")]
        [SugarColumn(ColumnDataType = "text", IsNullable = true)]
        public string AiOutputResult { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Description("备注")]
        [SugarColumn(ColumnDataType = "varchar(500)", IsNullable = true)]
        public string Remark { get; set; }
    }
}
