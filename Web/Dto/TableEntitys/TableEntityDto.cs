using Web.Dto.Plans;
using Web.Tables;

namespace Web.Dto.TableEntitys
{
    public class TableEntityDto : FullBaseDto
    {
        [Description("实体名称")]
        public string Name { get; set; }

        [Description("实体编码")]
        public string Code { get; set; }

        [Description("对应计划")]
        public Guid PlanId { get; set; }


        [Description("是否额外的")]
        public bool IsExtra { get; set; }

        [Description("是否公开")]
        public bool IsOpen { get; set; }


        [Description("关联次数")]
        public int RelativeCount { get; set; }

        public PlanDto PlanDto { get; set; }

        public TableSettingDto TableSettingDto { get; set; }

        public List<TableNavigateRelativeDto> TableNavigateRelatives { get; set; }
        public List<ColumnPropDto> Columns { get; set; }

        [Description("是否内置")]
        public bool IsBuiltin { get; set; }

        public TableEntityDto()
        {
            TableSettingDto = new TableSettingDto();
            Columns = new List<ColumnPropDto>();
            TableNavigateRelatives = new List<TableNavigateRelativeDto>();
        }
    }
}
