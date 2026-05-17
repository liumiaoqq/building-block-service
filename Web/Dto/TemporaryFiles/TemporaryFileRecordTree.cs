namespace Web.Dto.TemporaryFiles
{
    public class TemporaryFileRecordTree
    {
        [Description("分组标识")]
        public Guid GroupId { get; set; }


        [Description("存储文件名称")]
        public string NewFileName { get; set; }


        [Description("存储文件后缀")]
        public string NewFileSuffix { get; set; }

        [Description("存储路径")]
        public string NewPath { get; set; }


        [Description("父级文件Id")]
        public Guid ParentId { get; set; }

        [Description("SHA256")]
        public string SHA256 { get; set; }
        [Description("IsSpecialFile")]
        public bool IsSpecialFile { get; set; }



        public TemporaryFileRecordDto ParentTemporaryFileRecordDto { get; set; }


        public List<TemporaryFileRecordTree> Children { get; set; } = new List<TemporaryFileRecordTree>();

        [Description("内容")]
        public string Content { get; set; }
    }
}
