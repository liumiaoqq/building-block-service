
namespace Web.Dto.TableEntitys
{
    public class AiFixColumnLengthTableDto
    {

        [Description("表名")]
        public string TableName { get; set; }

        [Description("表ID")]
        public Guid TableId { get; set; }

        [Description("列属性列表")]
        public List<AiFixColumnLengthDto> Columns { get; set; }


    }
}
