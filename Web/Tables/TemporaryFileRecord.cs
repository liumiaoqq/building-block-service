namespace Web.Tables
{

    [Description("临时文件记录")]
    [YoungTable("TemporaryFileRecord")]
    public class TemporaryFileRecord : CreationAuditedAggregateRoot
    {

        [Description("分组标识")]
        public Guid GroupId { get; set; }
       

        [Description("存储文件名称")]
        public string NewFileName { get; set; }

        [Description("存储文件后缀")]
        public string NewFileSuffix { get; set; }

        [Description("存储路径")]
        public string NewPath { get; set; }

        [Description("HashCode")]
        public string HashCode { get; set; }
        [Description("IsSpecialFile")]
        public bool IsSpecialFile { get; set; }
        

        [Description("父级文件Id")]
        public Guid? ParentId { get; set; }

   

        [Description("内容")]
        public string Content { get; set; }

    }
}
