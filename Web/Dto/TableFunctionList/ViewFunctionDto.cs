namespace Web.Dto.TableFunctionList
{
    public class ViewFunctionDto
    {
        public List<TableFunctionEntityOption> Options { get; set; }


        public List<SelectResult> ViewColumnTypeSelectList { get; set; } = new List<SelectResult>();
        /// <summary>
        /// 显示的菜单
        /// </summary>
        public List<ViewFunctionTableEntityDto> ViewFunctionTableEntityList { get; set; }
    }
}
