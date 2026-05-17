namespace Web.Dto.Gengerations
{
    public class GengerationComponentTreeData : FullBaseDto
    {
        [Description("文件名称")]
        public string Label { get; set; }


        [Description("父级模板")]
        public Guid? ParentId { get; set; }

        [Description("模板内容")]
        public string Content { get; set; }


        [Description("是否横向扩展")]
        public bool Horizontal { get; set; }


        [Description("枚举横向扩展")]
        public bool EnumHorizontal { get; set; }

        [Description("资源地址")]
        public string NetworkAddress { get; set; }
        [Description("是否可解压")]
        public bool Decompressed { get; set; }

        [Description("是否不使用模板语法")]
        public bool IsTempleteGrammar { get; set; }

        [Description("组件Id")]
        public Guid? ComponentId { get; set; }

        [Description("是否是文件夹")]
        public bool IsFolder { get; set; }


        [Description("文件类型")]
        public string FileType { get; set; }

        public bool IsTemplete { get; set; }

        [Description("对应语言")]
        public LanguageWay LanguageWay { get; set; }


        [Description("第一层组件配置内容")]
        public string ComponentSettingContent { get; set; }


        public List<GengerationComponentTreeData> Children { get; set; }


        public GengerationComponentTreeData()
        {
            Children = new List<GengerationComponentTreeData>();


        }
    }
}
