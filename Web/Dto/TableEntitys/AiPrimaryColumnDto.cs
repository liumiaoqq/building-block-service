
namespace Web.Dto.TableEntitys
{
    /// <summary>
    /// AI识别主要列 - 列DTO
    /// </summary>
    public class AiPrimaryColumnDto
    {
        [Description("列ID")]
        public Guid ColumnPropId { get; set; }

        [Description("列名称")]
        public string Name { get; set; }

        [Description("列编码")]
        public string Code { get; set; }

        [Description("是否为主要列")]
        public bool IsPrimary { get; set; }
    }
}
