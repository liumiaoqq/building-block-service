using Org.BouncyCastle.Bcpg.OpenPgp;

namespace Web.Tables
{
    [Description("表配置")]
    [YoungTable("TableSetting")]
    public class TableSetting : CreationAuditedAggregateRoot
    {
        public Guid PlanId { get; set; }
        public Guid TableEntityId { get; set; }
        #region 额外属性
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
        #endregion

        #region 列表配置

        /// <summary>
        /// 列表配置
        /// </summary>
        public string ViewColumnSettingJson { get; set; }



        /// <summary>
        /// 编辑表单
        /// </summary>
        public string EditFormSettingJson { get; set; }


        /// <summary>
        /// 搜索表单
        /// </summary>
        public string SearchFormSettingJson { get; set; }


        /// <summary>
        /// 导入
        /// </summary>
        public string ImportSettingJson { get; set; }


        /// <summary>
        /// 导出
        /// </summary>
        public string ExportSettingJson { get; set; }


        #endregion

    }

    #region 视图
    public class ViewColumnSetting
    {

        /// <summary>
        /// 名称
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 显示编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string ColumnViewType { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        public string Width { get; set; }

        /// <summary>
        /// 模板
        /// </summary>
        public string Templete { get; set; }


    }

    #endregion

    #region 编辑
    public class EditFormSetting
    {
        public string Label { get; set; }

        public string Code { get; set; }

        public string EditFormType { get; set; }

        public bool IsRequired { get; set; }


        [SugarColumn(IsIgnore = true)]
        public object Setting { get; set; }

    }
    #endregion

    #region 搜索

    public class SearchFormSetting
    {
        public string Label { get; set; }

        public string Code { get; set; }

        public string SearchFormType { get; set; }





        [SugarColumn(IsIgnore = true)]
        public object Setting { get; set; }


    }


    #endregion

    #region 导入
    public class ImportSetting
    {
        public string Label { get; set; }

        public string Code { get; set; }
    }
    #endregion

    #region 导出

    #endregion
    public class ExportSetting
    {
        public string Label { get; set; }

        public string Code { get; set; }

        public string Type { get; set; }

        public object Prop { get; set; }
    }
}
