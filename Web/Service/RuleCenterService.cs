using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using Web.Dto.Rules;
using Web.Dto.TableEntitys;
using Web.Dto.TableFunctionList;
using Web.HttpClient;
using Web.HttpClientApi.DeepSeek.Service;
using Web.Manager;
using Web.Tables;

namespace Web.Service
{
    /// <summary>
    /// 规则中心策略服务
    /// </summary>
    public class RuleCenterService
    {
        protected ISqlSugarClient _sqlSugarClient;


        private readonly TableEntityManager _tableEntityManager;

        private readonly TableNavigateRelativeManager _tableNavigateRelativeManager;

        private readonly ExportRuleManager _exportRuleManager;

        private readonly ViewRuleManager _viewRuleManager;

        private readonly SearchRuleManager _searchRuleManager;

        private readonly EditRuleManager _editRuleManager;

        private readonly ColumnPropManager _columnPropManager;

        private readonly TableSettingManager _tableSettingManager;

        private readonly PlanEnumManager _planEnumManager;

        private readonly ICurrentUser _currentUser;

        private readonly IDeepSeekService _deepSeekService;



        public RuleCenterService(ISqlSugarClient sqlSugarClient, TableSettingManager tableSettingManager, TableEntityManager tableEntityManager, TableNavigateRelativeManager tableNavigateRelativeManager, ExportRuleManager exportRuleManager, ColumnPropManager columnPropManager, EditRuleManager editRuleManager, PlanEnumManager planEnumManager, ViewRuleManager viewRuleManager, SearchRuleManager searchRuleManager, ICurrentUser currentUser, IDeepSeekService deepSeekService)
        {
            _sqlSugarClient = sqlSugarClient;

            _tableEntityManager = tableEntityManager;
            _tableNavigateRelativeManager = tableNavigateRelativeManager;
            _exportRuleManager = exportRuleManager;
            _columnPropManager = columnPropManager;
            _tableSettingManager = tableSettingManager;
            _editRuleManager = editRuleManager;
            _planEnumManager = planEnumManager;
            _viewRuleManager = viewRuleManager;
            _searchRuleManager = searchRuleManager;
            _currentUser = currentUser;
            _deepSeekService = deepSeekService;
        }


        /// <summary>
        /// 获取功能清单列表
        /// </summary>
        public async Task<List<BatchTableSettingDto>> GetTableSettingList(BatchTableSettingQueryInput input)
        {
            var tableSettings = await _tableSettingManager.FindTableSettingDtoByPlanIdAsync(input.PlanId);
            var tableEntitys = await _tableEntityManager.GetListAsync(input.PlanId);
            var batchTableSettingDtos = new List<BatchTableSettingDto>();

            //循环表
            foreach (var tableEntity in tableEntitys.Where(x => x.IsExtra == false))
            {

                //查询对应的表配置
                var tableSetting = tableSettings.FirstOrDefault(x => x.TableEntityId == tableEntity.Id);

                var newTableSetting = new BatchTableSettingDto()
                {
                    PlanId = input.PlanId,
                    TableEntityDto = tableEntity,
                    Add = tableSetting == null ? false : tableSetting.Add,
                    Edit = tableSetting == null ? false : tableSetting.Edit,
                    View = tableSetting == null ? false : tableSetting.View,
                    Search = tableSetting == null ? false : tableSetting.Search,
                    Export = tableSetting == null ? false : tableSetting.Export,
                    BatchDelete = tableSetting == null ? false : tableSetting.BatchDelete,
                    Delete = tableSetting == null ? false : tableSetting.Delete,
                    Import = tableSetting == null ? false : tableSetting.Import,

                    TableSettingId = tableSetting?.Id,
                };
                batchTableSettingDtos.Add(newTableSetting);

            }

            return batchTableSettingDtos;
        }

        /// <summary>
        /// 批量保存表配置
        /// </summary>
        /// <param name="batchTableSettingDtos"></param>
        public async Task BatchSaveTableSetting(List<BatchTableSettingDto> batchTableSettingDtos)
        {
            var tableSettings = new List<TableSettingDto>();

            foreach (var batchTableSettingDto in batchTableSettingDtos)
            {
                var tableFunctionBillDto = await GetTableFunctionBillDto(batchTableSettingDto.TableEntityId, true);

                tableSettings.Add(new TableSettingDto()
                {
                    Add = batchTableSettingDto.Add,
                    Edit = batchTableSettingDto.Edit,
                    View = batchTableSettingDto.View,
                    Search = batchTableSettingDto.Search,
                    Export = batchTableSettingDto.Export,
                    BatchDelete = batchTableSettingDto.BatchDelete,
                    Delete = batchTableSettingDto.Delete,
                    Import = batchTableSettingDto.Import,
                    TableEntityId = batchTableSettingDto.TableEntityId,
                    EditFormSetting = tableFunctionBillDto.EditFunctionDto.EditFunctionTableEntityList,
                    ExportSetting = tableFunctionBillDto.ExportFunction.ExportFunctionTableEntityList,
                    SearchFormSetting = tableFunctionBillDto.SearchFunctionDto.SearchFunctionTableEntityList,
                    ViewColumnSetting = tableFunctionBillDto.ViewFunctionDto.ViewFunctionTableEntityList,
                    PlanId = batchTableSettingDto.PlanId,
                    Id = batchTableSettingDto.TableSettingId,
                });

            }
            await _tableSettingManager.BatchCreateTableSettingAsync(tableSettings);

        }


        /// <summary>
        /// 获取表功能配置
        /// </summary>
        /// <param name="tableEntityId">表实体ID</param>
        /// <param name="isReset">是否重置</param>
        /// <returns></returns>
        public async Task<TableFunctionBillDto> GetTableFunctionBillDto(Guid tableEntityId, bool isReset)
        {
            var dto = new TableFunctionBillDto()
            {
                ExportFunction = new ExportFunctionDto(),
                EditFunctionDto = new EditFunctionDto(),
                ViewFunctionDto = new ViewFunctionDto(),
                SearchFunctionDto = new SearchFunctionDto()
            };


            var tableSettings = await _tableSettingManager.FindTableSettingDtoAsync(tableEntityId);
            if (_currentUser.IsUser())
            {
                if (tableSettings.CreatorId != tableSettings.CreatorId)
                {
                    throw new YouJuException("该功能清单不属于你");
                }

            }

            dto.TableSettingDto = tableSettings;


            //得到这个表所有的列
            var columnProps = await _columnPropManager.GetColumnPropList(new ColumnPropPagedInput() { TableEntityId = tableEntityId });

            //得到表对应的所有关联
            var tableNavigateRelatives = await _tableNavigateRelativeManager.GetTableNavigateRelativeDtos(new TableNavigateRelativePagedInput() { RelativeTableId = tableEntityId });


            //计算出外键
            foreach (var columnProp in columnProps)
            {
                columnProp.IsForeignKey = tableNavigateRelatives.Exists(x => x.AssociationAColumnId == columnProp.Id && x.TableNavigateType == TableNavigateType.OneToOne);

            }

            #region 视图处理
            //处理导出表格下拉菜单
            dto.ViewFunctionDto.Options = GetViewFunctionOptions(columnProps, tableNavigateRelatives);

            dto.ViewFunctionDto.ViewFunctionTableEntityList = await DispatchViewSetting(tableSettings, columnProps, tableNavigateRelatives, isReset);
            dto.ViewFunctionDto.ViewColumnTypeSelectList = typeof(ViewColumnType).GetEnumSelect();

            #endregion

            #region 搜索处理
            dto.SearchFunctionDto.Options = GetSearchFunctionOptions(columnProps, tableNavigateRelatives);
            dto.SearchFunctionDto.SearchFunctionTableEntityList = await DispatchSearchSetting(tableSettings, columnProps, tableNavigateRelatives, isReset);
            dto.SearchFunctionDto.SearchTypeSelectList = typeof(SearchType).GetEnumSelect();

            #endregion

            #region 表单处理


            dto.EditFunctionDto.EditFormTypeSelectList = typeof(EditFormType).GetEnumSelect();


            dto.EditFunctionDto.Options = GetEditFunctionOptions(columnProps);

            //处理编辑表单逻辑
            dto.EditFunctionDto.EditFunctionTableEntityList = await DispatchEditSetting(tableSettings, columnProps, tableNavigateRelatives, isReset);





            #endregion

            #region 导出处理
            //处理导出表格下拉菜单
            dto.ExportFunction.Options = GetExportFunctionOptions(columnProps, tableNavigateRelatives);

            //处理导出逻辑
            dto.ExportFunction.ExportFunctionTableEntityList = await DispatchExportSetting(tableSettings, columnProps, tableNavigateRelatives, isReset);
            #endregion


            return dto;
        }

        /// <summary>
        /// 处理下拉选择
        /// </summary>
        private List<TableFunctionEntityOption> GetExportFunctionOptions(List<ColumnPropDto> columnProps, List<TableNavigateRelativeDto> tableNavigateRelatives)
        {

            //得到可下拉的列表
            var options = new List<TableFunctionEntityOption>();
            //先显示自己的所有列
            foreach (var columnProp in columnProps)
            {
                //结尾是Id的不执行
                if (columnProp.Code.EndsWith("Id") || columnProp.Code == nameof(CreationAuditedAggregateRoot.CreationTime))
                {
                    continue;
                }
                options.Add(new TableFunctionEntityOption()
                {
                    ColumnPropDto = columnProp,
                    ColumnPropId = columnProp.Id.Value,
                    ShowColumnCode = columnProp.Code,
                    ShowColumnName = columnProp.Name,

                    IsForeignKey = false,
                });
            }
            //处理有的外键
            foreach (var columnProp in columnProps.Where(x => x.IsForeignKey == true).ToList())
            {


                var tableNavigateRelative = tableNavigateRelatives.FirstOrDefault(x => x.AssociationAColumnId == columnProp.Id && x.TableNavigateType.IsIn(TableNavigateType.OneToOne));
                foreach (var foreignTableColumns in tableNavigateRelative.AssociationATableEntity.Columns)
                {
                    //结尾是Id的不执行
                    if (foreignTableColumns.Code.EndsWith("Id") || foreignTableColumns.Code == nameof(CreationAuditedAggregateRoot.CreationTime))
                    {
                        continue;
                    }

                    options.Add(new TableFunctionEntityOption()
                    {
                        ColumnPropDto = foreignTableColumns,
                        ColumnPropId = foreignTableColumns.Id.Value,
                        ShowColumnCode = $"{tableNavigateRelative.AssociationATableEntity.Code}.{foreignTableColumns.Code}",
                        ShowColumnName = $"{tableNavigateRelative.AssociationATableEntity.Name}{foreignTableColumns.Name}",
                        RelativeColumnPropDto = columnProp,
                        RelativeColumnPropId = columnProp.Id.Value,
                        IsForeignKey = true,

                    });
                }
            }
            return options;

        }

        /// <summary>
        /// 智能处理导出
        /// </summary>
        /// <returns></returns>
        private async Task<List<ExportFunctionTableEntityDto>> DispatchExportSetting(TableSettingDto tableSettings, List<ColumnPropDto> columnProps, List<TableNavigateRelativeDto> tableNavigateRelatives, bool isReset)
        {
            #region 处理智能导出
            //智能带出来
            if (tableSettings.ExportSetting.Count == 0 || isReset == true)
            {
                var exportList = new List<ExportFunctionTableEntityDto>();

                var rules = await _exportRuleManager.ListAsync(new ExportRulePagedInput());


                foreach (var columnPropItem in columnProps)
                {
                    foreach (var rule in rules)
                    {
                        var ruleDets = rule.RuleDets.OrderByDescending(x => x.Sort).ToList();
                        foreach (var dets in ruleDets)
                        {
                            //勾选了是否匹配外键
                            if (dets.IsMatchForeignKey && columnPropItem.IsForeignKey == false)
                            {
                                //跳过此规则
                                continue;
                            }

                            if (dets.ExportRuleMatchType == ExportRuleMatchType.匹配属性名称 && !Regex.IsMatch(columnPropItem.Name, dets.MatchRegular))
                            {
                                continue;
                            }
                            else if (dets.ExportRuleMatchType == ExportRuleMatchType.匹配属性值 && !Regex.IsMatch(columnPropItem.Code, dets.MatchRegular))
                            {
                                continue;
                            }
                            else if (dets.ExportRuleMatchType == ExportRuleMatchType.匹配属性类型 && !Regex.IsMatch(columnPropItem.ColumnPropTypeFormat, dets.MatchRegular))
                            {
                                continue;
                            }


                            if (dets.ExportRuleDispatchType == ExportRuleDispatchType.显示)
                            {
                                var exportFunctionTableEntityDto = new ExportFunctionTableEntityDto();
                                exportFunctionTableEntityDto.ColumnPropId = columnPropItem.Id.Value;
                                exportFunctionTableEntityDto.ColumnPropDto = columnPropItem;
                                exportFunctionTableEntityDto.ShowColumnName = columnPropItem.Name;
                                exportFunctionTableEntityDto.ShowColumnCode = columnPropItem.Code;
                                exportList.Add(exportFunctionTableEntityDto);

                            }
                            else if (dets.ExportRuleDispatchType == ExportRuleDispatchType.外键显示 && columnPropItem.IsForeignKey)
                            {



                                var tableNavigateRelative = tableNavigateRelatives.FirstOrDefault(x => x.AssociationAColumnId == columnPropItem.Id && x.TableNavigateType.IsIn(TableNavigateType.OneToOne));
                                if (tableNavigateRelative != null)
                                {

                                    //带出一个默认的列

                                    var stringCloumnProps = tableNavigateRelative.AssociationATableEntity.Columns.Where(x => x.ColumnPropType == ColumnPropType.字符串).ToList();
                                    var specString = new List<string>() { "No", "Title", "Name", "OrderNo" };
                                    ColumnPropDto firstColumnProp = null;
                                    if (stringCloumnProps.Exists(x => specString.Contains(x.Code)))
                                    {
                                        firstColumnProp = stringCloumnProps.FirstOrDefault(x => specString.Contains(x.Code));
                                    }
                                    else if (stringCloumnProps.Count > 0)
                                    {
                                        firstColumnProp = stringCloumnProps.FirstOrDefault();
                                    }


                                    if (firstColumnProp != null)
                                    {
                                        var exportFunctionTableEntityDto = new ExportFunctionTableEntityDto();
                                        exportFunctionTableEntityDto.ColumnPropId = firstColumnProp.Id.Value;
                                        exportFunctionTableEntityDto.ColumnPropDto = firstColumnProp;
                                        exportFunctionTableEntityDto.IsForeignKey = true;
                                        //if (firstColumnProp.Name.Contains(tableNavigateRelative.AssociationATableEntity.Name))
                                        //{
                                        exportFunctionTableEntityDto.ShowColumnName = firstColumnProp.Name;
                                        //}
                                        //else
                                        //{
                                        //    exportFunctionTableEntityDto.ShowColumnName = $"{tableNavigateRelative.AssociationATableEntity.Name}{firstColumnProp.Name}";
                                        //}
                                        exportFunctionTableEntityDto.ShowColumnCode = $"{tableNavigateRelative.AssociationATableEntity.Code}.{firstColumnProp.Code}";
                                        exportFunctionTableEntityDto.RelativeColumnPropId = columnPropItem.Id.Value;
                                        exportFunctionTableEntityDto.RelativeColumnPropDto = columnPropItem;
                                        exportList.Add(exportFunctionTableEntityDto);
                                    }

                                }
                            }
                            break;
                        }

                    }

                }
                var index = 0;
                foreach (var exportTableEntityDto in exportList)
                {
                    exportTableEntityDto.ShowSort = index;
                    index++;
                }


                return exportList;

            }
            else
            {
                return tableSettings.ExportSetting;
            }



            #endregion


        }
        /// <summary>
        /// 处理编辑下拉选择
        /// </summary>
        private List<TableFunctionEntityOption> GetEditFunctionOptions(List<ColumnPropDto> columnProps)
        {

            //得到可下拉的列表
            var options = new List<TableFunctionEntityOption>();
            //先显示自己的所有列
            foreach (var columnProp in columnProps)
            {

                options.Add(new TableFunctionEntityOption()
                {
                    ColumnPropDto = columnProp,
                    ColumnPropId = columnProp.Id.Value,
                    ShowColumnCode = columnProp.Code,
                    ShowColumnName = columnProp.Name,
                    IsForeignKey = columnProp.IsForeignKey,
                });
            }
            return options;


        }
        /// <summary>
        /// 智能编辑
        /// </summary>
        /// <param name="tableSettings">表设置</param>
        /// <param name="columnProps">列属性</param>
        /// <param name="tableNavigateRelatives">表导航关系</param>
        /// <param name="isReset">是否重置</param>
        /// <returns></returns>
        private async Task<List<EditFunctionTableEntityDto>> DispatchEditSetting(TableSettingDto tableSettings, List<ColumnPropDto> columnProps, List<TableNavigateRelativeDto> tableNavigateRelatives, bool isReset)
        {

            //特殊字符
            const int sortSpecCode = 5000;
            //外键下拉框
            const int sortForgienkeySelect = 4900;
            //枚举下拉框
            const int sortEnumSelect = 4800;
            //图片
            const int sortImage = 4700;

            //时间选择
            const int sortDateSelect = 4600;
            //长文本
            const int sortLong = 4500;
            //开关
            const int sortSwitch = 4400;

            //普通文本
            const int sortCommInput = 4300;
            //数量输入文本
            const int sortQtyInput = 4200;
            //文件上传
            const int sortFile = 4100;
            //地址选择
            const int sortAddressSelect = 1;
            //富文本
            const int sortRichText = 0;




            #region 处理智能编辑
            //智能带出来
            if (tableSettings.EditFormSetting.Count == 0 || isReset == true)
            {


                var planEnumIds = columnProps.Where(x => x.PlanEnumId.HasValue).Select(x => x.PlanEnumId.Value).ToList();
                var planEnums = await _planEnumManager.GetPlanEnumByIdsAsync(planEnumIds);

                var editList = new List<EditFunctionTableEntityDto>();

                var rules = await _editRuleManager.ListAsync(new EditRulePagedInput());

                foreach (var columnPropItem in columnProps)
                {
                    FunctionEditBase settingItem = null;
                    bool isAdd = false;
                    var editFunctionTableEntityDto = new EditFunctionTableEntityDto();
                    editFunctionTableEntityDto.ColumnPropId = columnPropItem.Id.Value;
                    editFunctionTableEntityDto.ColumnPropDto = columnPropItem;
                    editFunctionTableEntityDto.ShowColumnName = columnPropItem.Name;
                    editFunctionTableEntityDto.ShowColumnCode = columnPropItem.Code;
                    editFunctionTableEntityDto.IsForeignKey = columnPropItem.IsForeignKey;

                    foreach (var rule in rules)
                    {
                        var ruleDets = rule.RuleDets.OrderByDescending(x => x.Sort).ToList();
                        foreach (var dets in ruleDets)
                        {
                            if (dets.IsMatchForeignKey && columnPropItem.IsForeignKey == false)
                            {
                                continue;
                            }

                            if (dets.EditRuleMatchType == EditRuleMatchType.匹配属性名称 && !Regex.IsMatch(columnPropItem.Name, dets.MatchRegular))
                            {
                                continue;
                            }
                            else if (dets.EditRuleMatchType == EditRuleMatchType.匹配属性值 && !Regex.IsMatch(columnPropItem.Code, dets.MatchRegular))
                            {
                                continue;
                            }
                            else if (dets.EditRuleMatchType == EditRuleMatchType.匹配属性类型 && !Regex.IsMatch(columnPropItem.ColumnPropTypeFormat, dets.MatchRegular))
                            {
                                continue;
                            }
                            else if (dets.EditRuleMatchType == EditRuleMatchType.匹配属性长度 && columnPropItem.Length.HasValue == false)
                            {
                                continue;
                            }
                            else if (dets.EditRuleMatchType == EditRuleMatchType.匹配属性长度 && !Regex.IsMatch(columnPropItem.Length.Value.ToString(), dets.MatchRegular))
                            {
                                continue;
                            }


                            if (columnPropItem.IsForeignKey)
                            {
                                editFunctionTableEntityDto.ShowSort = sortForgienkeySelect;
                                var setting = FunctionEditSelect.NormalBuilder();
                                var tableNavigateRelative = tableNavigateRelatives.FirstOrDefault(x => x.AssociationAColumnId == columnPropItem.Id && x.TableNavigateType.IsIn(TableNavigateType.OneToOne));
                                if (tableNavigateRelative != null)
                                {
                                    var tableCode = tableNavigateRelative.AssociationATableEntity.Code;
                                    if (tableCode == "AppUser")
                                    {
                                        setting.Url = "/User/List";
                                        setting.ColumnName = "Name";
                                    }
                                    else
                                    {
                                        setting.Url = $"/{tableCode}/List";
                                        
                                        // 优先从列的IspPimaryDisplayColumn字段获取主要列
                                        var primaryColumn = tableNavigateRelative.AssociationATableEntity.Columns
                                            .FirstOrDefault(x => x.IspPimaryDisplayColumn == true && x.ColumnPropType == ColumnPropType.字符串);
                                        
                                        if (primaryColumn != null)
                                        {
                                            setting.ColumnName = primaryColumn.Code;
                                        }
                                        else
                                        {
                                            // 兜底逻辑：从字符串列中选择
                                            var stringCloumnProps = tableNavigateRelative.AssociationATableEntity.Columns.Where(x => x.ColumnPropType == ColumnPropType.字符串).ToList();
                                            if (stringCloumnProps.Count == 0)
                                            {
                                                continue;
                                            }

                                            if (stringCloumnProps.Exists(x => SysConst.SpeColumnCodes.Contains(x.Code)))
                                            {
                                                setting.ColumnName = stringCloumnProps.FirstOrDefault(x => SysConst.SpeColumnCodes.Contains(x.Code)).Code;
                                            }
                                            else
                                            {
                                                setting.ColumnName = stringCloumnProps.OrderBy(x => x.CreationTime).FirstOrDefault().Code;
                                            }
                                        }
                                    }
                                }
                                setting.ValidRules.Clear();
                                setting.SelectLabelType = SelectLabelType.动态;
                                setting.ColumnValue = "Id";
                                if (setting.ColumnName.IsNullOrWhiteSpace())
                                {
                                    setting.ColumnName = "Name";
                                }
                                settingItem = setting;
                            }
                            else if (dets.EditFormType == EditFormType.SigleSelect)
                            {
                                editFunctionTableEntityDto.ShowSort = sortForgienkeySelect;
                                var setting = FunctionEditSelect.NormalBuilder();
                                if (columnPropItem.ColumnPropType == ColumnPropType.枚举型)
                                {
                                    editFunctionTableEntityDto.ShowSort = sortEnumSelect;
                                    var planEnum = planEnums.FirstOrDefault(x => x.Id == columnPropItem.PlanEnumId);

                                    setting.SelectLabelType = SelectLabelType.动态;
                                    setting.ColumnValue = "Code";
                                    setting.ColumnLabel = "Label";
                                    setting.ColumnName = "Name";
                                    setting.ValidRules.Clear();
                                    if (planEnum != null)
                                    {
                                        setting.Url = $"/Select/{planEnum.Code}";
                                    }
                                }

                                settingItem = setting;
                            }
                            else if (dets.EditFormType == EditFormType.MulitSelect)
                            {
                                editFunctionTableEntityDto.ShowSort = sortForgienkeySelect;
                                settingItem = FunctionEditMulitSelect.NormalBuilder();

                            }
                            else if (dets.EditFormType == EditFormType.Image)
                            {
                                editFunctionTableEntityDto.ShowSort = sortImage;
                                settingItem = FunctionEditImage.NormalBuilder();

                            }
                            else if (dets.EditFormType == EditFormType.QtyRange)
                            {
                                editFunctionTableEntityDto.ShowSort = sortImage;
                                settingItem = FunctionEditQtyRange.NormalBuilder();

                            }
                            else if (dets.EditFormType == EditFormType.DateTimePicker)
                            {
                                editFunctionTableEntityDto.ShowSort = sortDateSelect;
                                settingItem = FunctiontEditDateTimePicker.NormalBuilder();

                            }
                            else if (dets.EditFormType == EditFormType.RichText)
                            {
                                settingItem = FunctionEditRichText.NormalBuilder();
                                editFunctionTableEntityDto.ShowSort = sortRichText;

                            }
                            else if (dets.EditFormType == EditFormType.DatePicker)
                            {
                                editFunctionTableEntityDto.ShowSort = sortDateSelect;
                                settingItem = FunctiontEditDatePicker.NormalBuilder();

                            }
                            else if (dets.EditFormType == EditFormType.FileUpload)
                            {
                                editFunctionTableEntityDto.ShowSort = sortFile;
                                settingItem = FunctionEditFileUpload.NormalBuilder();

                            }
                            else if (dets.EditFormType == EditFormType.Switch)
                            {
                                editFunctionTableEntityDto.ShowSort = sortSwitch;
                                settingItem = FunctionEditSwitch.NormalBuilder();

                            }
                            else if (dets.EditFormType == EditFormType.DateRange)
                            {
                                editFunctionTableEntityDto.ShowSort = sortDateSelect;
                                settingItem = FunctiontEditDateRangePicker.NormalBuilder();
                            }
                            else if (dets.EditFormType == EditFormType.DateTimeRange)
                            {
                                editFunctionTableEntityDto.ShowSort = sortDateSelect;
                                settingItem = FunctiontEditDateTimeRangePicker.NormalBuilder();
                            }
                            else if (dets.EditFormType == EditFormType.VideoUpload)
                            {
                                editFunctionTableEntityDto.ShowSort = sortImage;
                                settingItem = FunctionEditVideoUpload.NormalBuilder();
                            }



                            else if (dets.EditFormType == EditFormType.LongText)
                            {
                                editFunctionTableEntityDto.ShowSort = sortLong;
                                var editlongInput = FunctionEditLongInput.NormalBuilder();
                                editlongInput.MaxLength = columnPropItem.Length ?? 1024;//不要超过数据库的长度



                                editlongInput.Rows = 5;

                                settingItem = editlongInput;

                            }
                            else if (dets.EditFormType == EditFormType.Hidden)
                            {

                                var editIuputSetting = new FunctionEditBase()
                                {
                                    EditFormType = EditFormType.Hidden,

                                };
                                settingItem = editIuputSetting;

                            }
                            else if (dets.EditFormType == EditFormType.LatLngSelect)
                            {

                                editFunctionTableEntityDto.ShowSort = sortAddressSelect;
                                settingItem = FunctionEditLatLngSelectInput.NormalBuilder();
                            }
                            else
                            {

                                var editIuputSetting = FunctionEditInput.NormalBuilder();
                                editIuputSetting.MaxLength = columnPropItem.Length ?? 1024;//不要超过数据库的长度
                                settingItem = editIuputSetting;

                            }


                            //处理匹配正则表达式逻辑
                            var functionValidRules = new List<FunctionValidRule>();
                            foreach (var regularExp in dets.RegularExpressionDtos)
                            {

                                var needMatchString = string.Empty;
                                if (regularExp.RegularExpressionMatchType == RegularExpressionMatchType.匹配属性值)
                                {
                                    needMatchString = columnPropItem.Code;
                                }
                                else if (regularExp.RegularExpressionMatchType == RegularExpressionMatchType.匹配属性名称)
                                {
                                    needMatchString = columnPropItem.Name;
                                }
                                else if (regularExp.RegularExpressionMatchType == RegularExpressionMatchType.匹配属性类型)
                                {
                                    needMatchString = columnPropItem.ColumnPropTypeFormat;
                                }

                                if (Regex.IsMatch(needMatchString, regularExp.MatchRegular))
                                {
                                    functionValidRules.Add(new FunctionValidRule()
                                    {
                                        Message = regularExp.Message,
                                        Rule = regularExp.RegularValue,
                                    });
                                }

                            }
                            if (functionValidRules.Count > 0)
                            {
                                settingItem.ValidRules = functionValidRules;
                            }

                            isAdd = true;
                            break;
                        }
                    }


                    if (isAdd == false)
                    {

                        var editIuputSetting = FunctionEditInput.NormalBuilder();
                        editIuputSetting.MaxLength = columnPropItem.Length ?? 1024;//不要超过数据库的长度

                        settingItem = editIuputSetting;

                    }



                    editFunctionTableEntityDto.EditFormType = settingItem.EditFormType;
                    editFunctionTableEntityDto.Setting = settingItem;

                    if (editFunctionTableEntityDto.EditFormType == EditFormType.Input)
                    {
                        //处理排序的顺序
                        if (SysConst.SpeColumnCodes.Contains(columnPropItem.Code))
                        {
                            editFunctionTableEntityDto.ShowSort = sortSpecCode;
                        }
                        else if (editFunctionTableEntityDto.ColumnPropDto.ColumnPropType.IsIn(ColumnPropType.双浮点型, ColumnPropType.小数点, ColumnPropType.双浮点型))
                        {
                            editFunctionTableEntityDto.ShowSort = sortQtyInput;
                        }
                        else
                        {
                            editFunctionTableEntityDto.ShowSort = sortCommInput;
                        }
                    }
                    //如果隐藏则不需要渲染
                    if (editFunctionTableEntityDto.EditFormType == EditFormType.Hidden)
                    {
                        continue;
                    }


                    editList.Add(editFunctionTableEntityDto);
                }
                //editList = editList.OrderByDescending(x => x.ShowSort).ToList();

                return editList;
            }
            else
            {


                return tableSettings.EditFormSetting;
            }



            #endregion


        }
        /// <summary>
        /// 视图下拉
        /// </summary>
        private List<TableFunctionEntityOption> GetViewFunctionOptions(List<ColumnPropDto> columnProps, List<TableNavigateRelativeDto> tableNavigateRelatives)
        {

            //得到可下拉的列表
            var options = new List<TableFunctionEntityOption>();
            //先显示自己的所有列
            foreach (var columnProp in columnProps)
            {
                //结尾是Id的不执行
                if (columnProp.Code.EndsWith("Id") || columnProp.Code == nameof(CreationAuditedAggregateRoot.CreationTime))
                {
                    continue;
                }
                options.Add(new TableFunctionEntityOption()
                {
                    ColumnPropDto = columnProp,
                    ColumnPropId = columnProp.Id.Value,
                    ShowColumnCode = columnProp.Code,
                    ShowColumnName = columnProp.Name,

                    IsForeignKey = false,
                });
            }
            //处理有的外键
            foreach (var columnProp in columnProps.Where(x => x.IsForeignKey == true).ToList())
            {


                var tableNavigateRelative = tableNavigateRelatives.FirstOrDefault(x => x.AssociationAColumnId == columnProp.Id && x.TableNavigateType.IsIn(TableNavigateType.OneToOne));
                foreach (var foreignTableColumns in tableNavigateRelative.AssociationATableEntity.Columns)
                {
                    //结尾是Id的不执行
                    if (foreignTableColumns.Code.EndsWith("Id") || foreignTableColumns.Code == nameof(CreationAuditedAggregateRoot.CreationTime))
                    {
                        continue;
                    }

                    options.Add(new TableFunctionEntityOption()
                    {
                        ColumnPropDto = foreignTableColumns,
                        ColumnPropId = foreignTableColumns.Id.Value,
                        ShowColumnCode = $"{tableNavigateRelative.AssociationATableEntity.Code}.{foreignTableColumns.Code}",
                        ShowColumnName = $"{tableNavigateRelative.AssociationATableEntity.Name}{foreignTableColumns.Name}",
                        RelativeColumnPropDto = columnProp,
                        RelativeColumnPropId = columnProp.Id.Value,
                        IsForeignKey = true,

                    });
                }
            }
            return options;


        }
        /// <summary>
        /// 智能处理视图
        /// <summary>
        /// 智能视图
        /// </summary>
        /// <param name="tableSettings">表设置</param>
        /// <param name="columnProps">列属性</param>
        /// <param name="tableNavigateRelatives">表导航关系</param>
        /// <param name="isReset">是否重置</param>
        /// <returns></returns>
        private async Task<List<ViewFunctionTableEntityDto>> DispatchViewSetting(TableSettingDto tableSettings, List<ColumnPropDto> columnProps, List<TableNavigateRelativeDto> tableNavigateRelatives, bool isReset)
        {
            #region 处理智能视图
            //智能带出来
            if (tableSettings.ViewColumnSetting.Count == 0 || isReset == true)
            {

                var viewList = new List<ViewFunctionTableEntityDto>();

                var rules = await _viewRuleManager.ListAsync(new ViewRulePagedInput());


                foreach (var columnPropItem in columnProps)
                {
                    bool isAdd = false;
                    //默认有个隐藏
                    if (columnPropItem.IsForeignKey)
                    {
                        var viewFunctionTableEntityDto = new ViewFunctionTableEntityDto();
                        viewFunctionTableEntityDto.ColumnPropId = columnPropItem.Id.Value;
                        viewFunctionTableEntityDto.ColumnPropDto = columnPropItem;
                        viewFunctionTableEntityDto.ShowColumnName = columnPropItem.Name;
                        viewFunctionTableEntityDto.ShowColumnCode = columnPropItem.Code;
                        viewFunctionTableEntityDto.ViewColumnType = ViewColumnType.Hidden;
                        viewList.Add(viewFunctionTableEntityDto);
                    }


                    foreach (var rule in rules)
                    {
                        var ruleDets = rule.RuleDets.OrderByDescending(x => x.Sort).ToList();
                        foreach (var dets in ruleDets)
                        {
                            //勾选了是否匹配外键
                            if (dets.IsMatchForeignKey && columnPropItem.IsForeignKey == false)
                            {
                                //跳过此规则
                                continue;
                            }

                            if (dets.ViewRuleMatchType == ViewRuleMatchType.匹配属性名称 && !Regex.IsMatch(columnPropItem.Name, dets.MatchRegular))
                            {
                                continue;
                            }
                            else if (dets.ViewRuleMatchType == ViewRuleMatchType.匹配属性值 && !Regex.IsMatch(columnPropItem.Code, dets.MatchRegular))
                            {
                                continue;
                            }
                            else if (dets.ViewRuleMatchType == ViewRuleMatchType.匹配属性类型 && !Regex.IsMatch(columnPropItem.ColumnPropTypeFormat, dets.MatchRegular))
                            {
                                continue;
                            }
                            if (columnPropItem.IsForeignKey)
                            {
                                var tableNavigateRelative = tableNavigateRelatives.FirstOrDefault(x => x.AssociationAColumnId == columnPropItem.Id && x.TableNavigateType.IsIn(TableNavigateType.OneToOne));
                                if (tableNavigateRelative != null)
                                {
                                    var tableCode = tableNavigateRelative.AssociationATableEntity.Code;
                                    var stringCloumnProps = tableNavigateRelative.AssociationATableEntity.Columns.Where(x => x.ColumnPropType == ColumnPropType.字符串).OrderBy(x => x.CreationTime).ToList();
                                    
                                    if (stringCloumnProps.Count > 0)
                                    {
                                        ColumnPropDto firstColumnProp = null;
                                        
                                        // 优先从列的IspPimaryDisplayColumn字段获取主要列
                                        firstColumnProp = stringCloumnProps.FirstOrDefault(x => x.IspPimaryDisplayColumn == true);
                                        
                                        // 兜底逻辑
                                        if (firstColumnProp == null)
                                        {
                                            var specString = new List<string>() { "No", "Title", "Name", "OrderNo" };
                                            if (stringCloumnProps.Exists(x => specString.Contains(x.Code)))
                                            {
                                                firstColumnProp = stringCloumnProps.FirstOrDefault(x => specString.Contains(x.Code));
                                            }
                                            else
                                            {
                                                firstColumnProp = stringCloumnProps.FirstOrDefault();
                                            }
                                        }
                                        
                                        var viewFunctionTableEntityDto = new ViewFunctionTableEntityDto();
                                        viewFunctionTableEntityDto.ColumnPropId = firstColumnProp.Id.Value;
                                        viewFunctionTableEntityDto.ColumnPropDto = firstColumnProp;
                                        viewFunctionTableEntityDto.IsForeignKey = true;
                                        viewFunctionTableEntityDto.ShowColumnCode = $"{tableCode}.{firstColumnProp.Code}";

                                        viewFunctionTableEntityDto.ShowColumnName = firstColumnProp.Name;

                                        viewFunctionTableEntityDto.RelativeColumnPropId = columnPropItem.Id.Value;
                                        viewFunctionTableEntityDto.RelativeColumnPropDto = columnPropItem;
                                        viewFunctionTableEntityDto.ViewColumnType = ViewColumnType.Text;
                                        viewList.Add(viewFunctionTableEntityDto);
                                        isAdd = true;
                                        break;
                                    }

                                }
                            }
                            else
                            {
                                var viewFunctionTableEntityDto = new ViewFunctionTableEntityDto();
                                viewFunctionTableEntityDto.ColumnPropId = columnPropItem.Id.Value;
                                viewFunctionTableEntityDto.ColumnPropDto = columnPropItem;
                                viewFunctionTableEntityDto.ShowColumnName = columnPropItem.Name;
                                viewFunctionTableEntityDto.ShowColumnCode = columnPropItem.Code;
                                viewFunctionTableEntityDto.ViewColumnType = dets.ViewColumnType;
                                viewList.Add(viewFunctionTableEntityDto);
                                isAdd = true;
                                break;
                            }
                        }
                    }

                    if (isAdd == false && false == columnPropItem.IsForeignKey)
                    {
                        var viewFunctionTableEntityDto = new ViewFunctionTableEntityDto();
                        viewFunctionTableEntityDto.ColumnPropId = columnPropItem.Id.Value;
                        viewFunctionTableEntityDto.ColumnPropDto = columnPropItem;
                        viewFunctionTableEntityDto.ShowColumnName = columnPropItem.Name;
                        viewFunctionTableEntityDto.ShowColumnCode = columnPropItem.Code;
                        viewFunctionTableEntityDto.ViewColumnType = ViewColumnType.Text;
                        viewList.Add(viewFunctionTableEntityDto);


                    }

                }



                //如果大于7列 但是存在没有设置的文本列,默认给值140px
                if (viewList.Count(x => x.ViewColumnType != ViewColumnType.Hidden) > 7)
                {
                    foreach (var item in viewList.Where(x => x.ViewColumnType == ViewColumnType.Text && x.Width.HasValue == false))
                    {
                        item.Width = 160;
                    }
                }


                return viewList;

            }
            else
            {
                return tableSettings.ViewColumnSetting;
            }


            #endregion


        }

        /// <summary>
        /// 搜索视图下拉
        /// </summary>
        private List<TableFunctionEntityOption> GetSearchFunctionOptions(List<ColumnPropDto> columnProps, List<TableNavigateRelativeDto> tableNavigateRelatives)
        {

            //得到可下拉的列表
            var options = new List<TableFunctionEntityOption>();
            //先显示自己的所有列
            foreach (var columnProp in columnProps)
            {

                options.Add(new TableFunctionEntityOption()
                {
                    ColumnPropDto = columnProp,
                    ColumnPropId = columnProp.Id.Value,
                    ShowColumnCode = columnProp.Code,
                    ShowColumnName = columnProp.Name,
                    IsForeignKey = columnProp.IsForeignKey,
                });
            }
            return options;

        }

        /// <summary>
        /// <summary>
        /// 智能处理搜索
        /// </summary>
        /// <param name="tableSettings">表设置</param>
        /// <param name="columnProps">列属性</param>
        /// <param name="tableNavigateRelatives">表导航关系</param>
        /// <param name="isReset">是否重置</param>
        /// <returns></returns>
        private async Task<List<SearchFunctionTableEntityDto>> DispatchSearchSetting(TableSettingDto tableSettings, List<ColumnPropDto> columnProps, List<TableNavigateRelativeDto> tableNavigateRelatives, bool isReset)
        {
            #region 处理智能导出
            //智能带出来
            if (tableSettings.SearchFormSetting.Count == 0 || isReset == true)
            {

                //搜索排序权重
                const int highWeight = 999;
                const int middleWeight = 500;
                const int lowerWeight = 100;


                var searchList = new List<SearchFunctionTableEntityDto>();

                var rules = await _searchRuleManager.ListAsync(new SearchRulePagedInput());

                var planEnumIds = columnProps.Where(x => x.PlanEnumId.HasValue).Select(x => x.PlanEnumId.Value).ToList();
                var planEnums = await _planEnumManager.GetPlanEnumByIdsAsync(planEnumIds);

                foreach (var columnPropItem in columnProps)
                {
                    bool isAdd = false;
                    var searchFunctionTableEntityDto = new SearchFunctionTableEntityDto();
                    searchFunctionTableEntityDto.ColumnPropId = columnPropItem.Id.Value;
                    searchFunctionTableEntityDto.ColumnPropDto = columnPropItem;
                    searchFunctionTableEntityDto.ShowColumnName = columnPropItem.Name;
                    searchFunctionTableEntityDto.ShowColumnCode = columnPropItem.Code;
                    searchFunctionTableEntityDto.IsForeignKey = columnPropItem.IsForeignKey;

                    foreach (var rule in rules)
                    {
                        if (isAdd == true)
                        {
                            break;
                        }
                        var ruleDets = rule.RuleDets.OrderByDescending(x => x.Sort).ToList();
                        foreach (var dets in ruleDets)
                        {
                            if (isAdd == true)
                            {
                                break;
                            }

                            //勾选了是否匹配外键
                            if (dets.IsMatchForeignKey && columnPropItem.IsForeignKey == false)
                            {
                                //跳过此规则
                                continue;
                            }

                            if (dets.SearchRuleMatchType == SearchRuleMatchType.匹配属性名称 && !Regex.IsMatch(columnPropItem.Name, dets.MatchRegular))
                            {
                                continue;
                            }
                            else if (dets.SearchRuleMatchType == SearchRuleMatchType.匹配属性值 && !Regex.IsMatch(columnPropItem.Code, dets.MatchRegular))
                            {
                                continue;
                            }
                            else if (dets.SearchRuleMatchType == SearchRuleMatchType.匹配属性类型 && !Regex.IsMatch(columnPropItem.ColumnPropTypeFormat, dets.MatchRegular))
                            {
                                continue;
                            }
                            if (dets.SearchType == SearchType.Hidden)
                            {
                                break;
                            }


                            #region 单选框额外逻辑补充
                            if (dets.SearchType.IsIn(SearchType.SigleSelect, SearchType.MulitSelect))
                            {

                                if (dets.IsMatchForeignKey)
                                {

                                    var tableNavigateRelative = tableNavigateRelatives.FirstOrDefault(x => x.AssociationAColumnId == columnPropItem.Id && x.TableNavigateType.IsIn(TableNavigateType.OneToOne));
                                    if (tableNavigateRelative != null)
                                    {
                                        var tableCode = tableNavigateRelative.AssociationATableEntity.Code;
                                        var urlTableCode = tableCode == "AppUser" ? "User" : tableCode;
                                        var stringCloumnProps = tableNavigateRelative.AssociationATableEntity.Columns.Where(x => x.ColumnPropType == ColumnPropType.字符串).ToList();
                                        
                                        string primaryColumnCode = null;
                                        
                                        // 优先从列的IspPimaryDisplayColumn字段获取主要列
                                        var primaryColumn = stringCloumnProps.FirstOrDefault(x => x.IspPimaryDisplayColumn == true);
                                        if (primaryColumn != null)
                                        {
                                            primaryColumnCode = primaryColumn.Code;
                                        }
                                        else
                                        {
                                            // 兜底逻辑
                                            var specString = new List<string>() { "No", "Title", "Name", "OrderNo" };
                                            if (stringCloumnProps.Exists(x => specString.Contains(x.Code)))
                                            {
                                                primaryColumnCode = stringCloumnProps.FirstOrDefault(x => specString.Contains(x.Code))?.Code;
                                            }
                                            else if (stringCloumnProps.Count > 0)
                                            {
                                                primaryColumnCode = stringCloumnProps.FirstOrDefault()?.Code;
                                            }
                                        }
                                        
                                        if (!string.IsNullOrWhiteSpace(primaryColumnCode))
                                        {
                                            searchFunctionTableEntityDto.Setting = new FunctionSearchSelect()
                                            {
                                                SelectLabelType = SelectLabelType.动态,
                                                Url = $"/{urlTableCode}/List",
                                                ColumnLabel = primaryColumnCode,
                                                ColumnValue = "Id",
                                                ColumnName = primaryColumnCode,
                                            };
                                        }

                                    }

                                }
                                else if (columnPropItem.ColumnPropType == ColumnPropType.枚举型)
                                {
                                    var planEnum = planEnums.FirstOrDefault(x => x.Id == columnPropItem.PlanEnumId);

                                    if (planEnum != null)
                                    {
                                        searchFunctionTableEntityDto.Setting = new FunctionSearchSelect()
                                        {
                                            SelectLabelType = SelectLabelType.动态,
                                            Url = $"/Select/{planEnum.Code}",
                                            ColumnLabel = "Label",
                                            ColumnValue = "Code",
                                            ColumnName = "Name"
                                        };



                                    }
                                }

                            }
                            #endregion
                            searchFunctionTableEntityDto.SearchType = dets.SearchType;
                            #region 权重处理
                            if (columnPropItem.ColumnPropType == ColumnPropType.字符串 && dets.SearchType.IsIn(SearchType.LikeString, SearchType.EqualString))
                            {
                                searchFunctionTableEntityDto.ShowSort = highWeight;
                            }
                            #endregion
                            else if (dets.SearchType.IsIn(SearchType.SigleSelect, SearchType.MulitSelect))
                            {
                                searchFunctionTableEntityDto.ShowSort = middleWeight;
                            }
                            else if (dets.SearchType.IsIn(SearchType.DateRange, SearchType.DateTimeRange))
                            {
                                searchFunctionTableEntityDto.ShowSort = lowerWeight;
                            }

                            searchList.Add(searchFunctionTableEntityDto);
                            isAdd = true;
                        }

                    }

                }

                //对搜索进行一个排序
                searchList = searchList.OrderByDescending(x => x.ShowSort).ToList();



                return searchList;

            }
            else
            {
                return tableSettings.SearchFormSetting;
            }


            #endregion

        }



        /// <summary>
        /// 处理新创建的计划
        /// </summary>
        /// <returns></returns>
        public async Task ProcessNewPlanAsync(Guid planId)
        {

            //查询对应的planId下所有的表
            var tableEntitys = await _tableEntityManager.GetAllListAsync(new TableEntityPagedInput() { PlanId = planId });

            var tableSettings = new List<TableSettingDto>();

            //得到计划下的表

            foreach (var tableEntity in tableEntitys)
            {
                if (tableEntity.IsExtra == true)
                {
                    continue;
                }


                var tableFunctionBillDto = await GetTableFunctionBillDto(tableEntity.Id.Value, true);

                tableSettings.Add(new TableSettingDto()
                {
                    Add = true,
                    Edit = true,
                    View = true,
                    Search = true,

                    BatchDelete = true,
                    Delete = true,

                    TableEntityId = tableEntity.Id.Value,
                    EditFormSetting = tableFunctionBillDto.EditFunctionDto.EditFunctionTableEntityList,
                    ExportSetting = tableFunctionBillDto.ExportFunction.ExportFunctionTableEntityList,
                    SearchFormSetting = tableFunctionBillDto.SearchFunctionDto.SearchFunctionTableEntityList,
                    ViewColumnSetting = tableFunctionBillDto.ViewFunctionDto.ViewFunctionTableEntityList,
                    PlanId = tableEntity.PlanId,
                    Id = Guid.NewGuid(),
                });

            }
            await _tableSettingManager.BatchCreateTableSettingAsync(tableSettings);

        }


    }
}
