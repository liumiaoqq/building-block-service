using System;

namespace Web
{
    #region 时序图会话相关DTO

    /// <summary>
    /// 时序图会话Dto
    /// </summary>
    public class SequenceDiagramSessionDto : FullBaseDto
    {
        public string Name { get; set; }
        public Guid? CodeManagementId { get; set; }
        public bool IsActive { get; set; }
        public string SequenceDiagramContent { get; set; }
        public string Remark { get; set; }
    }

    /// <summary>
    /// 创建时序图会话输入Dto
    /// </summary>
    public class CreateSequenceDiagramSessionInput
    {
        public string Name { get; set; }
        public Guid? CodeManagementId { get; set; }
        public bool IsActive { get; set; }
        public string SequenceDiagramContent { get; set; }
        public string Remark { get; set; }
    }

    /// <summary>
    /// 更新时序图会话输入Dto
    /// </summary>
    public class UpdateSequenceDiagramSessionInput
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? CodeManagementId { get; set; }
        public bool IsActive { get; set; }
        public string SequenceDiagramContent { get; set; }
        public string Remark { get; set; }
    }

    /// <summary>
    /// 时序图会话分页查询输入Dto
    /// </summary>
    public class GetSequenceDiagramSessionPagedInput : CommPagedInput
    {
        public string Name { get; set; }
        public Guid? CodeManagementId { get; set; }
        public bool? IsActive { get; set; }
    }

    /// <summary>
    /// 获取或创建激活的时序图会话输入Dto
    /// </summary>
    public class GetOrCreateActiveSequenceDiagramSessionInput
    {
        public Guid? CodeManagementId { get; set; }
    }

    /// <summary>
    /// 设置激活时序图会话输入Dto
    /// </summary>
    public class SetActiveSequenceDiagramSessionInput
    {
        public Guid Id { get; set; }
    }

    /// <summary>
    /// AI生成PlantUML时序图输入Dto
    /// </summary>
    public class GenerateSequenceDiagramByAIInput
    {
        public Guid CodeManagementId { get; set; }
        public string Question { get; set; }
    }

    #endregion
}

