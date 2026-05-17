
namespace Web.Dto.TableEntitys
{
    public class AiSortColumnTableDto
    {

        [Description("表名")]
        public string TableName { get; set; }

        [Description("表ID")]
        public Guid TableId { get; set; }

        [Description("列属性列表")]
        public List<AiSortColumnDto> Columns { get; set; }


    }
}

