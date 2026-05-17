
using Newtonsoft.Json;
using Web.Dto.TableFunctionList;
using Web.Tables;

namespace Web.Dto.TableEntitys
{
    public class TableSettingDto : FullBaseDto
    {
        public Guid PlanId { get; set; }
        public Guid TableEntityId { get; set; }
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



        [Description("是否需要编辑滚动页面")]
        public bool IsEditScrollPage { get; set; }



        /// <summary>
        /// 列表配置
        /// </summary>
        [JsonIgnore]
        public string ViewColumnSettingJson { get; set; }


        public List<ViewFunctionTableEntityDto> ViewColumnSetting { get; set; }

        /// <summary>
        /// 编辑表单
        /// </summary>
        [JsonIgnore]
        public string EditFormSettingJson { get; set; }


        public List<EditFunctionTableEntityDto> EditFormSetting { get; set; }

        /// <summary>
        /// 搜索表单
        /// </summary>
        [JsonIgnore]
        public string SearchFormSettingJson { get; set; }



        public List<SearchFunctionTableEntityDto> SearchFormSetting { get; set; }



        /// <summary>
        /// 导入
        /// </summary>
        [JsonIgnore]
        public string ImportSettingJson { get; set; }

        public List<ImportSetting> ImportSetting { get; set; }
        /// <summary>
        /// 导出
        /// </summary>
        [JsonIgnore]
        public string ExportSettingJson { get; set; }
        public List<ExportFunctionTableEntityDto> ExportSetting { get; set; }


        //public List<ExportFunctionTableEntityDto> ExportSetting { get; set; }

        public TableSettingDto()
        {


        }
    }
}
