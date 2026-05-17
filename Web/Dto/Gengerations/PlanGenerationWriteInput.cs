using Newtonsoft.Json.Linq;
using Web.Dto.Components;

namespace Web.Dto.Gengerations
{
    public class PlanGenerationWriteInput
    {
        [Description("方案名称")]
        public string PlanName { get; set; }


        [Description("数据库连接")]
        public string DatabaseConnection { get; set; }

        [Description("后端文件名称")]
        public string BackFileName { get; set; }

        [Description("后端对应语言")]
        public LanguageWay BackLanguageWay { get; set; }


        [Description("数据库类型")]
        public DataBaseType DataBaseType { get; set; }


        [Description("文件名称")]
        public string FileName { get; set; }

        [Description("是否小程序")]
        public bool IsMiniProgram { get; set; }

        [Description("后端端口")]

        public int? BackPort { get; set; }

        [Description("组件配置")]
        public JObject SystemComponentSetting { get; set; }


        public List<PlanEnumWriteInput> PlanEnums { get; set; } = new List<PlanEnumWriteInput>();


        public List<TableEntityWriteInput> TableEntitys { get; set; } = new List<TableEntityWriteInput>();




        public TableEntityWriteInput TableEntity { get; set; } = new TableEntityWriteInput();


        public PlanEnumWriteInput PlanEnum { get; set; } = new PlanEnumWriteInput();




    }
    public class PlanEnumWriteInput
    {
        public string Name { get; set; }

        public string Code { get; set; }


        public string EnumProps { get; set; }


        public List<EnumInfo> EnumPropsList => EnumProps.ToList<EnumInfo>();


    }
    public class TableEntityWriteInput
    {
        [Description("表实体Id")]
        public Guid TableEntityId { get; set; }
        [Description("实体名称")]
        public string Name { get; set; } = string.Empty;

        [Description("实体编码")]
        public string Code { get; set; } = string.Empty;

        [Description("对应计划")]
        public Guid PlanId { get; set; }

        [Description("是否额外的")]
        public bool IsExtra { get; set; }


        public List<ColumnPropWriteInput> ColumnProps { get; set; } = new List<ColumnPropWriteInput>();

        public List<TableNavigateRelativeWirteInput> TableNavigateRelatives { get; set; } = new List<TableNavigateRelativeWirteInput>();

        public TableSettingInput TableSetting { get; set; } = new TableSettingInput();

    }


    public class TableNavigateRelativeWirteInput
    {
        public TableNavigateType TableNavigateType { get; set; }

        public string TableNavigateTypeFormat => TableNavigateType.ToDescription();

        public Guid RelativeTableId { get; set; }
        /// <summary>
        /// 表A
        /// </summary>
        public Guid? AssociationATableId { get; set; }

        public TableEntityWriteInput AssociationATableEntity { get; set; }

        /// <summary>
        /// 表A列Id
        /// </summary>
        public Guid? AssociationAColumnId { get; set; }

        public ColumnPropWriteInput AssociationAColumnProp { get; set; }
        /// <summary>
        ///  表B
        /// </summary>
        public Guid? AssociationBTableId { get; set; }
        public TableEntityWriteInput AssociationBTableEntity { get; set; }
        /// <summary>
        /// 表B列Id
        /// </summary>
        public Guid? AssociationBColumnId { get; set; }

        public ColumnPropWriteInput AssociationBColumnProp { get; set; }
    }


    public class ColumnPropWriteInput
    {
        public Guid ColumnPropId { get; set; }

        [Description("表实体Id")]
        public Guid TableEntityId { get; set; }

        [Description("列名称")]
        public string Name { get; set; } = string.Empty;

        [Description("列编码")]
        public string Code { get; set; } = string.Empty;
        [Description("长度")]
        public int? Length { get; set; } = 0;
        [Description("类型")]
        public ColumnPropType ColumnPropType { get; set; }
        [Description("类型描述")]
        public string ColumnPropTypeFormat => ColumnPropType.ToDescription();



        [Description("枚举值")]
        public string EnumCode { get; set; }

        [Description("是否可空")]
        public bool IsNull { get; set; }

        [Description("解释")]
        public string Display { get; set; } = string.Empty;
    }


    public class TableSettingInput
    {
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


        /// <summary>
        /// 列表配置
        /// </summary>
        public string ViewColumnSettingJson { get; set; }


        public List<object> ViewColumnSetting => ViewColumnSettingJson.ToList<object>();

        /// <summary>
        /// 编辑表单
        /// </summary>
        public string EditFormSettingJson { get; set; }


        public List<object> EditFormSetting => EditFormSettingJson.ToList<object>();

        /// <summary>
        /// 搜索表单
        /// </summary>
        public string SearchFormSettingJson { get; set; }


        public List<object> SearchFormSetting => SearchFormSettingJson.ToList<object>();



        /// <summary>
        /// 导入
        /// </summary>
        public string ImportSettingJson { get; set; }

        public List<object> ImportSetting => ImportSettingJson.ToList<object>();
        /// <summary>
        /// 导出
        /// </summary>
        public string ExportSettingJson { get; set; }
        public List<object> ExportSetting => ExportSettingJson.ToList<object>();
    }

}
