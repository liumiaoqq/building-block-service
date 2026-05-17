using System;

namespace Web
{
    #region 用例图会话相关DTO

    /// <summary>
    /// 用例图会话Dto
    /// </summary>
    public class UseCaseDiagramSessionDto : FullBaseDto
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string ActorsJson { get; set; }
        public string UseCasesJson { get; set; }
        public string RelationshipsJson { get; set; }
        public string UserPrompt { get; set; }
        public string AiInputContent { get; set; }
        public string AiOutputResult { get; set; }
    }

    /// <summary>
    /// 创建用例图会话输入Dto
    /// </summary>
    public class CreateUseCaseDiagramSessionInput
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string ActorsJson { get; set; }
        public string UseCasesJson { get; set; }
        public string RelationshipsJson { get; set; }
        public string UserPrompt { get; set; }
        public string AiInputContent { get; set; }
        public string AiOutputResult { get; set; }
    }

    /// <summary>
    /// 更新用例图会话输入Dto
    /// </summary>
    public class UpdateUseCaseDiagramSessionInput
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string ActorsJson { get; set; }
        public string UseCasesJson { get; set; }
        public string RelationshipsJson { get; set; }
        public string UserPrompt { get; set; }
        public string AiInputContent { get; set; }
        public string AiOutputResult { get; set; }
    }

    /// <summary>
    /// 用例图会话分页查询输入Dto
    /// </summary>
    public class GetUseCaseDiagramSessionPagedInput : CommPagedInput
    {
        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }

    /// <summary>
    /// 获取或创建激活的用例图会话输入Dto
    /// </summary>
    public class GetOrCreateActiveUseCaseDiagramSessionInput
    {
    }

    /// <summary>
    /// 设置激活用例图会话输入Dto
    /// </summary>
    public class SetActiveUseCaseDiagramSessionInput
    {
        public Guid Id { get; set; }
    }

    /// <summary>
    /// AI生成用例图输入Dto
    /// </summary>
    public class GenerateUseCaseDiagramByAIInput
    {
        /// <summary>
        /// 用例图会话ID（必填）
        /// </summary>
        public Guid UseCaseDiagramSessionId { get; set; }

        /// <summary>
        /// 功能提示词（用户需求描述）
        /// </summary>
        public string UserPrompt { get; set; }
    }

    #endregion
}
