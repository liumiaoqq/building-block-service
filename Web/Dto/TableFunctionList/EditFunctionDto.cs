namespace Web.Dto.TableFunctionList
{
    public class EditFunctionDto
    {



        /// <summary>
        /// 默认下拉的配置
        /// </summary>
        public List<SelectResult> EditFormTypeSelectList { get; set; }=new List<SelectResult>();
        /// <summary>
        /// 默认所有的配置
        /// </summary>
        public List<object> SettingList { get; set; }

        public List<TableFunctionEntityOption> Options { get; set; }
        /// <summary>
        /// 显示的菜单
        /// </summary>
        public List<EditFunctionTableEntityDto> EditFunctionTableEntityList { get; set; }

        public EditFunctionDto()
        {

            SettingList = new List<object>();
            SettingList.Add(FunctionEditFileUpload.NormalBuilder());
            SettingList.Add(FunctionEditImage.NormalBuilder());
            SettingList.Add(FunctionEditInput.NormalBuilder());
            SettingList.Add(FunctionEditMulitSelect.NormalBuilder());
            SettingList.Add(FunctionEditQtyRange.NormalBuilder());
            SettingList.Add(FunctionEditSwitch.NormalBuilder());
            SettingList.Add(FunctiontEditDateTimePicker.NormalBuilder());
            SettingList.Add(FunctiontEditDatePicker.NormalBuilder());
            SettingList.Add(FunctionEditSelect.NormalBuilder());
            SettingList.Add(FunctionEditRichText.NormalBuilder());
            SettingList.Add(FunctionEditMulitSelect.NormalBuilder());
        }
    }
}
