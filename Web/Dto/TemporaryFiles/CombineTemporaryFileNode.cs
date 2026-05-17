namespace Web.Dto.TemporaryFiles
{
    /// <summary>
    /// 对比内容的key
    /// </summary>
    public  abstract class CombineTemporaryFileNode
    {
        public string FullUrl { get; set; }

        [Description("层级")]
        public int Depth { get; set; }

        public string ParentFullUrl { get; set; }

        public bool IsFolder { get; set; }
    }
}
