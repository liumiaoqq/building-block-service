using System;

namespace Web
{
    #region 流程图会话相关DTO

    /// <summary>
    /// 流程图会话Dto
    /// </summary>
    public class FlowsheetSessionDto : FullBaseDto
    {
        public string Name { get; set; }
        public Guid? CodeManagementId { get; set; }
        public bool IsActive { get; set; }
        public string FlowsheetContent { get; set; }
        public string Remark { get; set; }
    }

    /// <summary>
    /// 创建流程图会话输入Dto
    /// </summary>
    public class CreateFlowsheetSessionInput
    {
        public string Name { get; set; }
        public Guid? CodeManagementId { get; set; }
        public bool IsActive { get; set; }
        public string FlowsheetContent { get; set; }
        public string Remark { get; set; }
    }

    /// <summary>
    /// 更新流程图会话输入Dto
    /// </summary>
    public class UpdateFlowsheetSessionInput
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? CodeManagementId { get; set; }
        public bool IsActive { get; set; }
        public string FlowsheetContent { get; set; }
        public string Remark { get; set; }
    }

    /// <summary>
    /// 流程图会话分页查询输入Dto
    /// </summary>
    public class GetFlowsheetSessionPagedInput : CommPagedInput
    {
        public string Name { get; set; }
        public Guid? CodeManagementId { get; set; }
        public bool? IsActive { get; set; }
    }

    /// <summary>
    /// 获取或创建激活的流程图会话输入Dto
    /// </summary>
    public class GetOrCreateActiveFlowsheetSessionInput
    {
        public Guid? CodeManagementId { get; set; }
    }

    /// <summary>
    /// 设置激活流程图会话输入Dto
    /// </summary>
    public class SetActiveFlowsheetSessionInput
    {
        public Guid Id { get; set; }
    }

    /// <summary>
    /// AI生成PlantUML输入Dto
    /// </summary>
    public class GeneratePlantUMLByAIInput
    {
        public Guid CodeManagementId { get; set; }
        public string Question { get; set; }
    }

    #endregion
}

