using System;

namespace Web
{
    #region ER图会话相关DTO

    /// <summary>
    /// ER图会话Dto
    /// </summary>
    public class ErDiagramSessionDto : FullBaseDto
    {
        public string Name { get; set; }
        public Guid? CodeManagementId { get; set; }
        public bool IsActive { get; set; }
        public string CompleteSql { get; set; }
        public string TableRelationJson { get; set; }
        public string UserPrompt { get; set; }
        public string AiInputContent { get; set; }
        public string AiOutputResult { get; set; }
        public string Remark { get; set; }
    }

    /// <summary>
    /// 创建ER图会话输入Dto
    /// </summary>
    public class CreateErDiagramSessionInput
    {
        public string Name { get; set; }
        public Guid? CodeManagementId { get; set; }
        public bool IsActive { get; set; }
        public string CompleteSql { get; set; }
        public string TableRelationJson { get; set; }
        public string UserPrompt { get; set; }
        public string AiInputContent { get; set; }
        public string AiOutputResult { get; set; }
        public string Remark { get; set; }
    }

    /// <summary>
    /// 更新ER图会话输入Dto
    /// </summary>
    public class UpdateErDiagramSessionInput
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? CodeManagementId { get; set; }
        public bool IsActive { get; set; }
        public string CompleteSql { get; set; }
        public string TableRelationJson { get; set; }
        public string UserPrompt { get; set; }
        public string AiInputContent { get; set; }
        public string AiOutputResult { get; set; }
        public string Remark { get; set; }
    }

    /// <summary>
    /// ER图会话分页查询输入Dto
    /// </summary>
    public class GetErDiagramSessionPagedInput : CommPagedInput
    {
        public string Name { get; set; }
        public Guid? CodeManagementId { get; set; }
        public bool? IsActive { get; set; }
    }

    /// <summary>
    /// 获取或创建激活的ER图会话输入Dto
    /// </summary>
    public class GetOrCreateActiveErDiagramSessionInput
    {
        public Guid? CodeManagementId { get; set; }
    }

    /// <summary>
    /// 设置激活ER图会话输入Dto
    /// </summary>
    public class SetActiveErDiagramSessionInput
    {
        public Guid Id { get; set; }
    }

    /// <summary>
    /// AI生成ER图输入Dto
    /// </summary>
    public class GenerateErDiagramByAIInput
    {
        /// <summary>
        /// ER图会话ID（必填）
        /// </summary>
        public Guid ErDiagramSessionId { get; set; }

        /// <summary>
        /// 功能提示词（用户需求描述）
        /// </summary>
        public string UserPrompt { get; set; }

        /// <summary>
        /// 表关系JSON（SQL解析后的表结构数据）
        /// </summary>
        public string TableRelationJson { get; set; }
    }

    #endregion
}
