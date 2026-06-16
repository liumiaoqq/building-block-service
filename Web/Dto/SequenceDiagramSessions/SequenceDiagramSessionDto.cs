using System;

namespace Web
{
    #region 时序图会话相关DTO

    public class SequenceDiagramSessionDto : FullBaseDto
    {
        public string Name { get; set; }
        public Guid? CodeManagementId { get; set; }
        public bool IsActive { get; set; }
        public string SequenceDiagramContent { get; set; }
        public string Remark { get; set; }
    }

    public class CreateSequenceDiagramSessionInput
    {
        public string Name { get; set; }
        public Guid? CodeManagementId { get; set; }
        public bool IsActive { get; set; }
        public string SequenceDiagramContent { get; set; }
        public string Remark { get; set; }
    }

    public class UpdateSequenceDiagramSessionInput
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? CodeManagementId { get; set; }
        public bool IsActive { get; set; }
        public string SequenceDiagramContent { get; set; }
        public string Remark { get; set; }
    }

    public class GetSequenceDiagramSessionPagedInput : CommPagedInput
    {
        public string Name { get; set; }
        public Guid? CodeManagementId { get; set; }
        public bool? IsActive { get; set; }
    }

    public class GetOrCreateActiveSequenceDiagramSessionInput
    {
        public Guid? CodeManagementId { get; set; }
    }

    public class SetActiveSequenceDiagramSessionInput
    {
        public Guid Id { get; set; }
    }

    public class GenerateSequenceDiagramByAIInput
    {
        public string Question { get; set; }
    }

    #endregion
}
