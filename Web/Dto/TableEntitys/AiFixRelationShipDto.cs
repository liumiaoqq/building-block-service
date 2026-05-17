
namespace Web.Dto.TableEntitys
{
    public class AiFixRelationShipDto
    {

        [Description("表ID")]
        public Guid TableId { get; set; }

        [Description("表编码")]
        public string TableCode { get; set; }

        [Description("表名称")]
        public string TableName { get; set; }



        [Description("引用表编码")]
        public string RefTableCode { get; set; }

        [Description("引用表名称")]
        public string RefTableName { get; set; }

        [Description("引用表ID")]
        public Guid RefTableId { get; set; }

        [Description("引用列编码")]
        public string RefColumnCode { get; set; }

        [Description("引用列ID")]
        public Guid RefColumnId { get; set; }

        [Description("引用列名称")]
        public string RefColumnName { get; set; }


    }
}
