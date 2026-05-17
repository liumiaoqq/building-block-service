
namespace Web.Dto.TableEntitys
{
    public class AiFixRelationShipTableDto
    {

        [Description("表名")]
        public string TableName { get; set; }

        [Description("表编码")]
        public string TableCode { get; set; }

        [Description("表ID")]
        public Guid TableId { get; set; }

        [Description("列属性列表")]
        public List<AiFixRelationShipColumnDto> Columns { get; set; }


        [Description("表关系列表")]
        public List<AiFixRelationShipDto> Relations { get; set; }

    }
}
