namespace Web.DB
{
    public interface IWarehouseId
    {

        [SugarColumn(ColumnDataType = "char(36)")]
        public Guid? WarehouseId { get; set; }
    }
}
