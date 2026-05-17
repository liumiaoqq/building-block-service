namespace Web.Dto.TableEntitys
{
    public class ColumnPropDto : FullBaseDto
    {

        [Description("表实体Id")]
        public Guid? TableEntityId { get; set; }

        public TableEntityDto TableEntityDto { get; set; }

        [Description("列名称")]
        public string Name { get; set; }

        [Description("列编码")]
        public string Code { get; set; }
        [Description("长度")]
        public int? Length { get; set; }
        [Description("类型")]
        public ColumnPropType ColumnPropType { get; set; }

        public string ColumnPropTypeFormat => ColumnPropType.ToDescription() ?? "";
        public Guid? PlanEnumId { get; set; }

        public Guid? PlanId { get; set; }

        public string ColumnPropTypeValue { get; set; }

        [Description("是否可空")]
        public bool IsNull { get; set; }

        [Description("解释")]
        public string Display { get; set; }


        [Description("是否外键")]
        public bool IsForeignKey { get; set; }
        [Description("是否主要的显示列")]
        public bool? IspPimaryDisplayColumn { get; set; }
    }
}
