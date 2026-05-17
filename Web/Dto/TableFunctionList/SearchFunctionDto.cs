namespace Web.Dto.TableFunctionList
{
    public class SearchFunctionDto
    {
        public List<TableFunctionEntityOption> Options { get; set; }

        /// <summary>
        /// 默认所有的配置
        /// </summary>
        public List<object> SettingList { get; set; }

        public List<SelectResult> SearchTypeSelectList { get; set; } = new List<SelectResult>();
        /// <summary>
        /// 显示的菜单
        /// </summary>
        public List<SearchFunctionTableEntityDto> SearchFunctionTableEntityList { get; set; }

        public SearchFunctionDto()
        {
            SettingList = new List<object>();

            SettingList.Add(FunctionSearchSelect.NormalBuilder());

            SettingList.Add(FunctionMulitSearchSelect.NormalBuilder());

            
        }
    }
}
