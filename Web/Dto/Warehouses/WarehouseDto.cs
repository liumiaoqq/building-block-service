namespace Web.Dto.Warehouses
{
    public class WarehouseDto : FullBaseDto
    {
        [Description("仓库名称")]
        public string Name { get; set; }


        [Description("仓库作用")]
        public string Content { get; set; }

    }
}
