
namespace Web.Dto.TableEntitys
{
    public class AiFixRelationShipColumnDto
    {


        [Description("列ID")]
        public Guid ColumnPropId { get; set; }


        [Description("列名称")]
        public string Name { get; set; }

        [Description("列编码")]
        public string Code { get; set; }


        [Description("列类型")]
        public String ColumnType { get; set; }

    }
}
