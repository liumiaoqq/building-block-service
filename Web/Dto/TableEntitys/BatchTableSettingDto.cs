
using Newtonsoft.Json;
using Web.Dto.TableFunctionList;
using Web.Tables;

namespace Web.Dto.TableEntitys
{
    public class BatchTableSettingDto : FullBaseDto
    {
        public Guid PlanId { get; set; }
        public Guid TableEntityId { get; set; }

        public Guid? TableSettingId { get; set; }


        public TableEntityDto TableEntityDto { get; set; }

        [Description("新增功能")]
        public bool Add { get; set; }

        [Description("修改功能")]
        public bool Edit { get; set; }

        [Description("视图功能")]
        public bool View { get; set; }

        [Description("删除功能")]
        public bool Delete { get; set; }

        [Description("批量删除功能")]
        public bool BatchDelete { get; set; }


        [Description("导入功能")]
        public bool Import { get; set; }

        [Description("导出功能")]
        public bool Export { get; set; }

        [Description("搜索功能")]
        public bool Search { get; set; }




        public BatchTableSettingDto()
        {


        }
    }
}
