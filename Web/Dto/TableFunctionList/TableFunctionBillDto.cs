namespace Web.Dto.TableFunctionList
{

    /// <summary>
    /// 功能配置的根
    /// </summary>
    public class TableFunctionBillDto
    {

        public TableSettingDto TableSettingDto { get; set; }



        /// <summary>
        /// 视图功能
        /// </summary>
        public ViewFunctionDto ViewFunctionDto { get; set; }



        /// <summary>
        /// 搜索功能
        /// </summary>
        public SearchFunctionDto SearchFunctionDto { get; set; }

        /// <summary>
        /// 编辑功能
        /// </summary>
        public EditFunctionDto EditFunctionDto { get; set; }

        /// <summary>
        /// 导出功能
        /// </summary>
        public ExportFunctionDto ExportFunction { get; set; }




    }
}
