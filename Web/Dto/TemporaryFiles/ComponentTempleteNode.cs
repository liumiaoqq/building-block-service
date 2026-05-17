using Web.Dto.Components;

namespace Web.Dto.TemporaryFiles
{
    public class ComponentTempleteNode: CombineTemporaryFileNode
    {

        public Guid Id { get; set; }
        [Description("文件名称")]
        public string Label { get; set; }


        [Description("父级模板")]
        public Guid? ParentId { get; set; }

        [Description("模板内容")]
        public string Content { get; set; }

        public string HashCode { get; set; }
      

        [Description("是否横向扩展")]
        public bool Horizontal { get; set; }

        [Description("是否枚举横向扩展")]
        public bool EnumHorizontal { get; set; }

        [Description("是否不使用模板语法")]
        public bool IsTempleteGrammar { get; set; }
        [Description("资源地址")]
        public string NetworkAddress { get; set; }
        [Description("是否可解压")]
        public bool Decompressed { get; set; }

        [Description("组件Id")]
        public Guid? ComponentId { get; set; }

        [Description("是否是文件夹")]
        public bool IsFolder { get; set; }


        [Description("文件类型")]
        public string FileType { get; set; }

        public bool IsTemplete { get; set; }

 
        public List<ComponentTempleteNode> Children { get; set; } = new List<ComponentTempleteNode>();
    }
}
