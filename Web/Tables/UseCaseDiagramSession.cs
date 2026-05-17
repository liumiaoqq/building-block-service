namespace Web.Tables
{
    /// <summary>
    /// 用例图会话表
    /// </summary>
    [Description("用例图会话")]
    [YoungTable("UseCaseDiagramSession")]
    public class UseCaseDiagramSession : CreationAuditedAggregateRoot
    {
        /// <summary>
        /// 会话名称
        /// </summary>
        [Description("会话名称")]
        [SugarColumn(ColumnDataType = "varchar(200)", IsNullable = false)]
        public string Name { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        [Description("是否激活")]
        [SugarColumn(IsNullable = false)]
        public bool IsActive { get; set; }

        /// <summary>
        /// 参与者JSON数组（存储参与者列表）
        /// </summary>
        [Description("参与者JSON")]
        [SugarColumn(ColumnDataType = "text", IsNullable = true)]
        public string ActorsJson { get; set; }

        /// <summary>
        /// 用例JSON数组（存储用例及其关联的参与者）
        /// </summary>
        [Description("用例JSON")]
        [SugarColumn(ColumnDataType = "text", IsNullable = true)]
        public string UseCasesJson { get; set; }

        /// <summary>
        /// 用例关系JSON数组（存储用例之间的关系，如include、extend）
        /// </summary>
        [Description("用例关系JSON")]
        [SugarColumn(ColumnDataType = "text", IsNullable = true)]
        public string RelationshipsJson { get; set; }

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
    }
}
