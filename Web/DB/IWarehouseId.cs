namespace Web.DB
{
    public interface IWarehouseId
    {

        [SugarColumn( ColumnDataType = "uniqueidentifier")]
        public Guid? WarehouseId { get; set; }
    }
}
