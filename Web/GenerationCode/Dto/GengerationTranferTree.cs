namespace Web.GenerationCode.Dto
{
    public class GengerationTranferTree
    {

       
        [Description("文件名称")]
        public string FileName { get; set; }


        [Description("是否是文件夹")]
        public bool IsFolder { get; set; }


        [Description("文件类型")]
        public string FileType { get; set; }

        public bool IsTemplete { get; set; }


        [Description("模板内容")]
        public string Content { get; set; }


      
        [Description("资源地址")]
        public string NetworkAddress { get; set; }
    

    

      

        public List<GengerationTranferTree> Children { get; set; }


        public GengerationTranferTree()
        {
            Children = new List<GengerationTranferTree>();


        }
    }
}
