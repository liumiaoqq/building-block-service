using Web.DB;

namespace Web.Tables
{
    public class SqlTemplete : CreationAuditedAggregateRoot, IWarehouseId
    {
        public string Name { get; set; }

        [Description("数据库类型")]
        public DataBaseType DataBaseType { get; set; }

        /// <summary>
        /// 整个数据库创建模板
        /// </summary>
        public string CreateDbContent { get; set; }


        /// <summary>
        /// 修改列模板
        /// </summary>
        public string AlterColumnConent { get; set; }

        /// <summary>
        /// 新列模板
        /// </summary>
        public string AddColumnContent { get; set; }

        /// <summary>
        /// 删除列模板
        /// </summary>
        public string DeleteColumnContent { get; set; }


        public Guid? WarehouseId { get; set; }

        /// <summary>
        /// 链接字符串
        /// </summary>
        public string ConectionString { get; set; }

     
    }
}
