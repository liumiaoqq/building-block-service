
namespace Web.Dto.TableEntitys
{
    public class AiFixColumnLengthDto
    {


        [Description("列ID")]
        public Guid ColumnPropId { get; set; }


        [Description("列名称")]
        public string Name { get; set; }

        [Description("列编码")]
        public string Code { get; set; }


        [Description("旧长度")]
        public int? OldLength { get; set; }

        [Description("新长度")]
        public int? NewLength { get; set; }


    }
}
