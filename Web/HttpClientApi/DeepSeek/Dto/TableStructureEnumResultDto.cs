namespace Web.Dto.TableEntitys
{
    /// <summary>
    /// 表结构枚举结果
    /// </summary>
    public class TableStructureEnumResultDto

    {
        [Description("列名称")]
        public string Name { get; set; }

        [Description("列编码")]
        public string Code { get; set; }

        public List<TableStructureEnumDtoList> EnumPropsList { get; set; }

    }

    public class TableStructureEnumDtoList
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }

    public class TableStructureEnumResultDtoList
    {
        public List<TableStructureEnumResultDto> Enums { get; set; }
    }
}
