using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Web.Dto.TableFunctionList;
using Web.Tables;

namespace Web.Manager
{
    public class TableSettingManager
    {
        protected ISqlSugarClient _sqlSugarClient;

        public TableSettingManager(ISqlSugarClient sqlSugarClient)
        {
            _sqlSugarClient = sqlSugarClient;
        }

        public async Task<List<TableSettingDto>> FindTableSettingDtoByPlanIdAsync(Guid planId)
        {
            var tableSettings = await _sqlSugarClient.Queryable<TableSetting>().Where(x => x.PlanId == planId).Select<TableSettingDto>().ToListAsync();
            return tableSettings;
        }


        public async Task<TableSettingDto> FindTableSettingDtoAsync(Guid tableEntityId)
        {
            var tableSettings = await _sqlSugarClient.Queryable<TableSetting>().Where(x => x.TableEntityId == tableEntityId).Select<TableSettingDto>().FirstAsync();

            tableSettings = tableSettings ?? new TableSettingDto();
            LoadSetting(tableSettings);
            return tableSettings;

        }
        private void LoadSetting(TableSettingDto tableSettings)
        {
            try
            {
                tableSettings.ViewColumnSetting = tableSettings.ViewColumnSettingJson.DeserializeObject<List<ViewFunctionTableEntityDto>>();
                tableSettings.EditFormSetting = tableSettings.EditFormSettingJson.DeserializeObject<List<EditFunctionTableEntityDto>>();
                foreach (var item in tableSettings.EditFormSetting)
                {

                    if (item.EditFormType == EditFormType.Input)
                    {
                        item.Setting = item.Setting.ToJson().DeserializeObject<FunctionEditInput>();
                    }
                    else if (item.EditFormType == EditFormType.SigleSelect)
                    {
                        item.Setting = item.Setting.ToJson().DeserializeObject<FunctionEditSelect>();
                    }
                    else if (item.EditFormType == EditFormType.MulitSelect)
                    {
                        item.Setting = item.Setting.ToJson().DeserializeObject<FunctionEditMulitSelect>();
                    }
                    else if (item.EditFormType == EditFormType.Image)
                    {
                        item.Setting = item.Setting.ToJson().DeserializeObject<FunctionEditImage>();
                    }
                    else if (item.EditFormType == EditFormType.DateTimePicker)
                    {
                        item.Setting = item.Setting.ToJson().DeserializeObject<FunctiontEditDateTimePicker>();
                    }
                    else if (item.EditFormType == EditFormType.RichText)
                    {
                        item.Setting = item.Setting.ToJson().DeserializeObject<FunctionEditRichText>();
                    }
                    else if (item.EditFormType == EditFormType.DatePicker)
                    {
                        item.Setting = item.Setting.ToJson().DeserializeObject<FunctiontEditDatePicker>();
                    }
                    else if (item.EditFormType == EditFormType.FileUpload)
                    {
                        item.Setting = item.Setting.ToJson().DeserializeObject<FunctionEditFileUpload>();
                    }
                    else if (item.EditFormType == EditFormType.Switch)
                    {
                        item.Setting = item.Setting.ToJson().DeserializeObject<FunctionEditSwitch>();
                    }
                    else if (item.EditFormType == EditFormType.LongText)
                    {
                        item.Setting = item.Setting.ToJson().DeserializeObject<FunctionEditLongInput>();
                    }
                    else if (item.EditFormType == EditFormType.QtyRange)
                    {
                        item.Setting = item.Setting.ToJson().DeserializeObject<FunctionEditQtyRange>();
                    }
                }
                tableSettings.SearchFormSetting = tableSettings.SearchFormSettingJson.DeserializeObject<List<SearchFunctionTableEntityDto>>();

                tableSettings.ImportSetting = tableSettings.ImportSettingJson.DeserializeObject<List<ImportSetting>>();
                tableSettings.ExportSetting = tableSettings.ExportSettingJson.DeserializeObject<List<ExportFunctionTableEntityDto>>();
            }
            catch (Exception ex)
            {
                //兼容 老模板 如果是老的重新写
                tableSettings.ViewColumnSetting = new List<ViewFunctionTableEntityDto>();
                tableSettings.EditFormSetting = new List<EditFunctionTableEntityDto>();
                tableSettings.SearchFormSetting = new List<SearchFunctionTableEntityDto>();
                tableSettings.ImportSetting = new List<ImportSetting>();
                tableSettings.ExportSetting = new List<ExportFunctionTableEntityDto>();



            }
        }


        public async Task<TableSettingDto> CreateTableSettingAsync(TableSettingDto input)
        {
            await _sqlSugarClient.Deleteable<TableSetting>().Where(x => x.TableEntityId == input.TableEntityId).ExecuteCommandAsync();
            input.ViewColumnSettingJson = input.ViewColumnSetting.ToJson();
            input.SearchFormSettingJson = input.SearchFormSetting.ToJson();
            input.EditFormSettingJson = input.EditFormSetting.ToJson();
            input.ImportSettingJson = input.ImportSetting.ToJson();
            input.ExportSettingJson = input.ExportSetting.ToJson();
            var setting = new TableSetting()
            {
                Add = input.Add,
                BatchDelete = input.BatchDelete,
                Export = input.Export,
                Edit = input.Edit,
                Delete = input.Delete,
                View = input.View,
                Import = input.Import,
                Search = input.Search,
                PlanId = input.PlanId,
                TableEntityId = input.TableEntityId,
                ViewColumnSettingJson = input.ViewColumnSettingJson,
                SearchFormSettingJson = input.SearchFormSettingJson,
                EditFormSettingJson = input.EditFormSettingJson,
                ImportSettingJson = input.ImportSettingJson,
                ExportSettingJson = input.ExportSettingJson,
            };
            await _sqlSugarClient.Insertable(setting).ExecuteCommandAsync();
            return input;
        }

        public async Task BatchCreateTableSettingAsync(List<TableSettingDto> input)
        {
            var tableEntityIds = input.Select(x => x.TableEntityId).ToList();
            await _sqlSugarClient.Deleteable<TableSetting>().Where(x => tableEntityIds.Contains(x.TableEntityId)).ExecuteCommandAsync();

            var tableSettings = new List<TableSetting>();

            foreach (var item in input)
            {
                item.ViewColumnSettingJson = item.ViewColumnSetting.ToJson();
                item.SearchFormSettingJson = item.SearchFormSetting.ToJson();
                item.EditFormSettingJson = item.EditFormSetting.ToJson();
                item.ImportSettingJson = item.ImportSetting.ToJson();
                item.ExportSettingJson = item.ExportSetting.ToJson();
                var setting = new TableSetting()
                {
                    Add = item.Add,
                    BatchDelete = item.BatchDelete,
                    Export = item.Export,
                    Edit = item.Edit,
                    Delete = item.Delete,
                    View = item.View,
                    Import = item.Import,
                    Search = item.Search,
                    PlanId = item.PlanId,
                    TableEntityId = item.TableEntityId,
                    ViewColumnSettingJson = item.ViewColumnSettingJson,
                    SearchFormSettingJson = item.SearchFormSettingJson,
                    EditFormSettingJson = item.EditFormSettingJson,
                    ImportSettingJson = item.ImportSettingJson,
                    ExportSettingJson = item.ExportSettingJson,
                };
                tableSettings.Add(setting);
            }
            await _sqlSugarClient.Insertable(tableSettings).ExecuteCommandAsync();

        }






    }
}
