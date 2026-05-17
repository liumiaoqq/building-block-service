using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Newtonsoft.Json;
using NPOI.XWPF.UserModel;
using System.Text.RegularExpressions;
using Web.Dto.Plans;
using Web.Dto.Rules;
using Web.Dto.TableEntitys;
using Web.Dto.TableFunctionList;
using Web.Extensions;
using Web.Manager;
using Web.Service;
using Web.Tables;
using YouJu.Infrastructure.Extensions;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TableEntityController : YouJuController<TableEntity, TableEntityDto, PlanPagedInput>
    {

        private readonly TableEntityManager _tableEntityManager;

        private readonly DicManager _dicManager;

        private readonly TableNavigateRelativeManager _tableNavigateRelativeManager;

        private readonly ExportRuleManager _exportRuleManager;

        private readonly ColumnPropManager _columnPropManager;

        private readonly RuleCenterService _ruleCenterService;

        private readonly TableEntityService _tableEntityService;
        public TableEntityController(IServiceProvider serviceProvider, TableEntityManager tableEntityManager, DicManager dicManager, TableNavigateRelativeManager tableNavigateRelativeManager, ColumnPropManager columnPropManager, ExportRuleManager exportRuleManager, RuleCenterService ruleCenterService, TableEntityService tableEntityService) : base(serviceProvider)
        {
            _tableEntityManager = tableEntityManager;
            _dicManager = dicManager;
            _tableNavigateRelativeManager = tableNavigateRelativeManager;
            _columnPropManager = columnPropManager;
            _exportRuleManager = exportRuleManager;
            _ruleCenterService = ruleCenterService;
            _tableEntityService = tableEntityService;
        }
        /// <summary>
        /// 公共获取单个对象
        /// </summary>
        [HttpPost("GetAsync")]
        public override async Task<TableEntityDto> GetAsync(IdInput<Guid> input)
        {
            var dto = await SqlSugarClient.Queryable<TableEntity>().Where(x => x.Id == input.Id).Select<TableEntityDto>().FirstAsync();
            return dto ?? new TableEntityDto();
        }
        [HttpPost("GetTableTreeData")]
        public async Task<List<TableTreeDto>> GetTableTreeData(TableEntityPagedInput input)
        {
            if (input.PlanId.HasValue)
            {
                return await _tableEntityManager.GetTableTreeMenu(input);
            }
            else
            {
                return new List<TableTreeDto>();
            }
        }


        /// <summary>
        /// 创建
        /// </summary>
        [HttpPost("CreateOrEditAsync")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public override async Task<TableEntityDto> CreateOrEditAsync(TableEntityDto input)
        {
            return await _tableEntityService.CreateOrEditAsync(input);

        }
        [HttpPost("DeleteAsync")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public override async Task DeleteAsync(IdInput<Guid> input)
        {
            await _tableEntityService.DeleteAsync(input);

        }
        /// <summary>
        /// 批量创建查询
        /// </summary>
        [HttpPost("GetEditListAsync")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public async Task<List<TableEntityDto>> GetEditListAsync(TableEntityPagedInput input)
        {
            return await _tableEntityService.GetEditListAsync(input);

        }
        /// <summary>
        /// 批量创建
        /// </summary>
        [HttpPost("BatchCreateOrEdit")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public async Task BatchCreateOrEdit(BatchCreateOrEditTableEntityInput input)
        {
            await _tableEntityService.BatchCreateOrEdit(input);
        }

        #region  表分享复制
        /// <summary>
        /// 查询目前可分享的表
        /// </summary>
        [HttpPost("GetSharePlanTableList")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async Task<List<TableEntityShareDto>> GetSharePlanTableList(QuerySharePlanTableInput input)
        {
            return await _tableEntityManager.GetSharePlanTableList(input);
        }
        /// <summary>
        /// 完成分享表的复制
        /// </summary>
        [HttpPost("CopyTableEntityToPlan")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async Task CopyTableEntityToPlan(CopyTableEntityToPlanInput input)
        {
            await _tableEntityManager.CopyTableEntityToPlan(input);
        }
        [HttpPost("GetTableAndColumnList")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public async Task<List<TableEntityDto>> GetTableAndColumnList(TableEntityPagedInput input)
        {
            var lists = await _tableEntityManager.GetAllListAsync(input);
            return lists;
        }
        #endregion



        #region SQL脚本自动识别


        [HttpPost("LoadSqlScript")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public async Task<List<ColumnPropDto>> LoadSqlScript(TableEntitySqlScriptInput input)
        {
            return await _tableEntityService.LoadSqlScript(input);
        }


        [HttpPost("AnalyzeWholeMySqlSqlScript")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public async Task<List<TableEntityDto>> AnalyzeWholeMySqlSqlScript(TableEntitySqlScriptInput input)
        {
            return await _tableEntityService.AnalyzeWholeMySqlSqlScript(input);
        }

        #endregion


        #region 功能编辑

        [HttpPost("GetTableSettingList")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public async Task<List<BatchTableSettingDto>> GetTableSettingList(BatchTableSettingQueryInput input)
        {
            return await _ruleCenterService.GetTableSettingList(input);
        }


        [HttpPost("BatchSaveTableSetting")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public async Task BatchSaveTableSetting(List<BatchTableSettingDto> input)
        {
            await _ruleCenterService.BatchSaveTableSetting(input);
        }




        [HttpPost("GetTableFunctionBillDto")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public async Task<TableFunctionBillDto> GetTableFunctionBillDto(TableEntityPagedInput input)
        {
            var dto = await _ruleCenterService.GetTableFunctionBillDto(input.TableEntityId.Value, input.IsReset);

            return dto;
        }




        [HttpPost("GetTableSettingDto")]
        public async Task<TableSettingDto> GetTableSettingDto(TableEntityPagedInput input)
        {
            var tableSettings = await SqlSugarClient.Queryable<TableSetting>().Where(x => x.TableEntityId == input.TableEntityId).Select<TableSettingDto>().FirstAsync();

            tableSettings = tableSettings ?? new TableSettingDto();
            tableSettings.ViewColumnSetting = tableSettings.ViewColumnSettingJson.DeserializeObject<List<ViewFunctionTableEntityDto>>();
            tableSettings.EditFormSetting = tableSettings.EditFormSettingJson.DeserializeObject<List<EditFunctionTableEntityDto>>();
            tableSettings.SearchFormSetting = tableSettings.SearchFormSettingJson.DeserializeObject<List<SearchFunctionTableEntityDto>>();
            tableSettings.ImportSetting = tableSettings.ImportSettingJson.DeserializeObject<List<ImportSetting>>();
            tableSettings.ExportSetting = tableSettings.ExportSettingJson.DeserializeObject<List<ExportFunctionTableEntityDto>>();

            return tableSettings;
        }

        [HttpPost("BatchCreateTableAsync")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public async Task BatchCreateTableAsync(TableEntitySqlScriptSubmitDto input)
        {
            await _tableEntityService.BatchCreateTableAsync(input);
        }



        [HttpPost("SaveSetting")]
        public async Task SaveSetting(TableSettingDto input)
        {
            await SqlSugarClient.Deleteable<TableSetting>().Where(x => x.TableEntityId == input.TableEntityId).ExecuteCommandAsync();
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
            await SqlSugarClient.Insertable(setting).ExecuteCommandAsync();
        }
        [HttpPost("GetColumnList")]
        public async Task<PagedReuslt<TableColumnFunction>> GetColumnList(TableEntityPagedInput input)
        {

            //得到表对应的所有关联
            var tableNavigateRelatives = await _tableNavigateRelativeManager.GetTableNavigateRelativeDtos(new TableNavigateRelativePagedInput() { RelativeTableId = input.TableEntityId });

            var columnProps = await _columnPropManager.GetColumnPropList(new ColumnPropPagedInput() { TableEntityId = input.TableEntityId });


            var selects = columnProps.OrderBy(x => x.CreationTime)
                .Select(x =>
                  new TableColumnFunction()
                  {
                      Name = x.Code,
                      Value = x.Code,
                      Label = x.Name,
                      Prop = x,
                      TableNavigateRelativeDtos = tableNavigateRelatives.Where(z => z.AssociationAColumnId == x.Id || z.AssociationBColumnId == x.Id).ToList()
                  })
                .ToList();

            return new PagedReuslt<TableColumnFunction>(selects, selects.Count);
        }
        /// <summary>
        /// 得到显示类型
        /// </summary>
        [HttpPost("GetColumnViewTypes")]
        public async Task<PagedReuslt<SelectResult>> GetColumnViewTypes(TableEntityPagedInput input)
        {
            var dicCodes = await _dicManager.GetDicCodeList(new DicCodePagedInput() { DicTypeCode = "ColumnViewType" });
            var selects = dicCodes.Select(x => new SelectResult() { Name = x.Code, Value = x.Code, Label = x.Name }).ToList();
            return new PagedReuslt<SelectResult>(selects, selects.Count);
        }
        /// <summary>
        /// 得到搜索类型
        /// </summary>
        [HttpPost("GetSearchType")]
        public async Task<PagedReuslt<SelectResult>> GetSearchType(TableEntityPagedInput input)
        {
            var dicCodes = await _dicManager.GetDicCodeList(new DicCodePagedInput() { DicTypeCode = "SearchType" });
            if (!dicCodes.HasItem())
            {
                throw new YouJuException("请配置SearchType");
            }
            var selects = dicCodes.Select(x => new SelectResult() { Name = x.Code, Value = x.Code, Label = x.Name }).ToList();
            return new PagedReuslt<SelectResult>(selects, selects.Count);
        }
        /// <summary>
        /// 得到编辑表单类型
        /// </summary>
        [HttpPost("GetEditFormType")]
        public async Task<PagedReuslt<SelectResult>> GetEditFormType(TableEntityPagedInput input)
        {
            var dicCodes = await _dicManager.GetDicCodeList(new DicCodePagedInput() { DicTypeCode = "EditFormType" });
            if (!dicCodes.HasItem())
            {
                throw new YouJuException("请配置编辑EditFormType");
            }
            var selects = dicCodes.Select(x => new SelectResult() { Name = x.Code, Value = x.Code, Label = x.Name }).ToList();
            return new PagedReuslt<SelectResult>(selects, selects.Count);
        }
        /// <summary>
        /// 得到正则表达式
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("GetValidRules")]
        public async Task<PagedReuslt<SelectResult>> GetValidRules(TableEntityPagedInput input)
        {
            var dicCodes = await _dicManager.GetDicCodeList(new DicCodePagedInput() { DicTypeCode = "ValidRules" });
            if (!dicCodes.HasItem())
            {
                throw new YouJuException("请配置编辑ValidRules");
            }
            var selects = dicCodes.Select(x => new SelectResult() { Name = x.Name, Value = x.Code, Label = x.Remark, Prop = x }).ToList();

            return new PagedReuslt<SelectResult>(selects, selects.Count);
        }

        #endregion



        #region AI转换驼峰命名

        [HttpPost("GetAIConvertTableEntityListAsync")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public async Task<List<AIConvertTableEntityNameDto>> GetAIConvertTableEntityListAsync(IdInput<Guid> input)
        {
            return await _tableEntityService.GetAIConvertTableEntityListAsync(input);
        }
        /// <summary>
        /// AI转换驼峰命名
        /// </summary>
        [HttpPost("AIConvertTableEntityNameAsync")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public async Task<List<AIConvertTableEntityNameDto>> AIConvertTableEntityNameAsync(List<AIConvertTableEntityNameDto> input)
        {
            return await _tableEntityService.AIConvertTableEntityName(input);
        }

        [HttpPost("AIConvertTableEntityNameSave")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public async Task AIConvertTableEntityNameSave(List<AIConvertTableEntityNameDto> input)
        {
            await _tableEntityService.AIConvertTableEntityNameSave(input);
        }
        #endregion


        #region AI自动修复长度

        [HttpPost("GetAiFixColumnLengthListAsync")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public async Task<List<AiFixColumnLengthTableDto>> GetAiFixColumnLengthListAsync(IdInput<Guid> input)
        {
            return await _tableEntityService.GetAiFixColumnLengthListAsync(input);
        }

        [HttpPost("AIFixColumnLengthAsync")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public async Task<List<AiFixColumnLengthTableDto>> AIFixColumnLengthAsync(List<AiFixColumnLengthTableDto> input)
        {
            return await _tableEntityService.AIFixColumnLengthAsync(input);
        }
        [HttpPost("AIFixColumnLengthSaveAsync")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public async Task AIFixColumnLengthSaveAsync(List<AiFixColumnLengthTableDto> input)
        {
            await _tableEntityService.AIFixColumnLengthSaveAsync(input);
        }
        #endregion

        #region AI自动修复表关系

        [HttpPost("GetAiFixRelationShipListAsync")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public async Task<List<AiFixRelationShipTableDto>> GetAiFixRelationShipListAsync(IdInput<Guid> input)
        {
            return await _tableEntityService.GetAiFixRelationShipListAsync(input);
        }
        [HttpPost("AiFixRelationShipAsync")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public async Task<List<AiFixRelationShipTableDto>> AiFixRelationShipAsync(List<AiFixRelationShipTableDto> input)
        {
            return await _tableEntityService.AIFixRelationShipAsync(input);
        }

        /**
   手动分析
   */
        [HttpPost("ManualFixRelationShipAsync")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public async Task<List<AiFixRelationShipTableDto>> ManualFixRelationShipAsync(List<AiFixRelationShipTableDto> input)
        {
            return await _tableEntityService.ManualFixRelationShipAsync(input);
        }

        [HttpPost("AIFixRelationShipSaveAsync")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public async Task AIFixRelationShipSaveAsync(AiFixRelationShipTableSubmitDto input)
        {
            await _tableEntityService.AIFixRelationShipSaveAsync(input);
        }
        #endregion

        #region  AI自动解析枚举

        [HttpPost("AiFixEnumBySqlScript")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public async Task<List<AiFixEnumResultDto>> AiFixEnumBySqlScript(AiFixEnumBySqlScriptSubmitDto input)
        {
            return await _tableEntityService.AiFixEnumBySqlScript(input);
        }

        [HttpPost("AiFixEnumSave")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public async Task AiFixEnumSaveAsync(AiFixEnumSaveDto input)
        {
            await _tableEntityService.AiFixEnumSaveAsync(input);
        }

        #endregion

        #region AI自动列排序

        [HttpPost("GetAiSortColumnListAsync")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public async Task<List<AiSortColumnTableDto>> GetAiSortColumnListAsync(IdInput<Guid> input)
        {
            return await _tableEntityService.GetAiSortColumnListAsync(input);
        }

        [HttpPost("AiSortColumnAsync")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public async Task<List<AiSortColumnTableDto>> AiSortColumnAsync(List<AiSortColumnTableDto> input)
        {
            return await _tableEntityService.AiSortColumnAsync(input);
        }

        [HttpPost("AiSortColumnSaveAsync")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public async Task AiSortColumnSaveAsync(List<AiSortColumnTableDto> input)
        {
            await _tableEntityService.AiSortColumnSaveAsync(input);
        }

        #endregion

        #region AI自动绑定枚举

        [HttpPost("GetAiBindEnumListAsync")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public async Task<AiBindEnumResultDto> GetAiBindEnumListAsync(IdInput<Guid> input)
        {
            return await _tableEntityService.GetAiBindEnumListAsync(input);
        }

        [HttpPost("AiBindEnumAsync")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public async Task<AiBindEnumResultDto> AiBindEnumAsync(AiBindEnumResultDto input)
        {
            return await _tableEntityService.AiBindEnumAsync(input);
        }

        [HttpPost("AiBindEnumSaveAsync")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public async Task AiBindEnumSaveAsync(AiBindEnumSaveDto input)
        {
            await _tableEntityService.AiBindEnumSaveAsync(input);
        }

        #endregion

        #region AI自动绑定列类型

        [HttpPost("GetAiBindColumnPropListAsync")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public async Task<List<AiBindColumnPropTableDto>> GetAiBindColumnPropListAsync(IdInput<Guid> input)
        {
            return await _tableEntityService.GetAiBindColumnPropListAsync(input);
        }

        [HttpPost("AiBindColumnPropAsync")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public async Task<List<AiBindColumnPropTableDto>> AiBindColumnPropAsync(List<AiBindColumnPropTableDto> input)
        {
            return await _tableEntityService.AiBindColumnPropAsync(input);
        }

        [HttpPost("AiBindColumnPropSaveAsync")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public async Task AiBindColumnPropSaveAsync(AiBindColumnPropSaveDto input)
        {
            await _tableEntityService.AiBindColumnPropSaveAsync(input);
        }

        #endregion

        #region AI自动识别主要列

        [HttpPost("GetAiPrimaryColumnListAsync")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public async Task<List<AiPrimaryColumnTableDto>> GetAiPrimaryColumnListAsync(IdInput<Guid> input)
        {
            return await _tableEntityService.GetAiPrimaryColumnListAsync(input);
        }

        [HttpPost("AiPrimaryColumnAsync")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public async Task<List<AiPrimaryColumnTableDto>> AiPrimaryColumnAsync(List<AiPrimaryColumnTableDto> input)
        {
            return await _tableEntityService.AiPrimaryColumnAsync(input);
        }

        [HttpPost("AiPrimaryColumnSaveAsync")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public async Task AiPrimaryColumnSaveAsync(AiPrimaryColumnSaveDto input)
        {
            await _tableEntityService.AiPrimaryColumnSaveAsync(input);
        }

        #endregion

        
    }
}
