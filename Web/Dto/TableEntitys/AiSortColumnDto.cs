
namespace Web.Dto.TableEntitys
{
    public class AiSortColumnDto
    {

        [Description("列ID")]
        public Guid ColumnPropId { get; set; }


        [Description("列名称")]
        public string Name { get; set; }

        [Description("列编码")]
        public string Code { get; set; }


        [Description("当前排序")]
        public int OldSort { get; set; }

        [Description("新排序")]
        public int? NewSort { get; set; }

        [Description("AI建议理由")]
        public string Reason { get; set; }


    }
}

