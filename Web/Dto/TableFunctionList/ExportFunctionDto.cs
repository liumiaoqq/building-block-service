namespace Web.Dto.TableFunctionList
{
    
    /// <summary>
    /// 导出功能
    /// </summary>
    public class ExportFunctionDto
    {


        public List<TableFunctionEntityOption> Options { get; set; }
        /// <summary>
        /// 显示的菜单
        /// </summary>
        public List<ExportFunctionTableEntityDto> ExportFunctionTableEntityList{ get; set; }

      
    }
}
