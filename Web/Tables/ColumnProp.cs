namespace Web.Tables
{
    [Description("列实体")]
    [YoungTable("ColumnProp")]
    public class ColumnProp : CreationAuditedAggregateRoot
    {
        #region 表的基本属性
        [Description("表实体Id")]
        public Guid TableEntityId { get; set; }

        [Description("列名称")]
        public string Name { get; set; }

        [Description("列编码")]
        public string Code { get; set; }
        [Description("长度")]
        public int? Length { get; set; }
        [Description("类型")]
        public ColumnPropType ColumnPropType { get; set; }

        [Description("关联枚举类型")]
        public Guid? PlanEnumId { get; set; }

        [Description("是否可空")]
        public bool IsNull { get; set; }

        [Description("解释")]
        public string Display { get; set; }



        [Description("是否主要的显示列")]
        [SugarColumn(IsNullable = true)]
        public bool? IspPimaryDisplayColumn { get; set; }

        #endregion
    }
}
