

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NPOI.HPSF;
using Scriban;
using System.Numerics;
using System.Reflection;
using Web.Dto.Components;
using Web.Dto.Gengerations;
using Web.Dto.Plans;
using Web.Dto.Warehouses;
using Web.GenerationCode.Provider;
using Web.Service;
using Web.Tables;
using YouJu.Infrastructure;

namespace Web.Manager
{
    public class PlanManager
    {

        protected ISqlSugarClient _sqlSugarClient;




        private readonly SystemModuleManager _systemModuleManager;

        private readonly TableEntityManager _tableEntityManager;

        public PlanManager(ISqlSugarClient sqlSugarClient, SystemModuleManager systemModuleManager, TableEntityManager tableEntityManager)
        {
            _sqlSugarClient = sqlSugarClient;
            _systemModuleManager = systemModuleManager;
            _tableEntityManager = tableEntityManager;
        }


        public async Task<PagedReuslt<PlanDto>> ListAsync(PlanPagedInput input)
        {

            List<Guid> whereUserIds = new List<Guid>();
            if (input.UserKeyWord.IsNotNullOrNotWhiteSpace())
            {
                whereUserIds = await _sqlSugarClient.Queryable<AppUser>().Where(x => x.UserName.Contains(input.UserKeyWord) || x.Email.Contains(input.UserKeyWord)).Select(x => x.Id).ToListAsync();
            }


            RefAsync<int> totalCount = 0;
            var plans = await _sqlSugarClient
                .Queryable<Plan>()
                .WhereIF(input.UserId.HasValue, x => x.CreatorId == input.UserId)
                .WhereIF(whereUserIds.HasItem(), x => whereUserIds.Contains(x.CreatorId.Value))
                .WhereIF(input.PlanId.HasValue, x => x.Id == input.PlanId)
                .WhereIF(input.WarehouseId.HasValue, x => x.WarehouseId == input.WarehouseId)
                .WhereIF(input.PlanType.HasValue, x => x.PlanType == input.PlanType)
                .WhereIF(input.PlanTypeList.HasItem(), x => input.PlanTypeList.Contains(x.PlanType.Value))
                .WhereIF(input.PlanName.IsNotNullOrNotWhiteSpace(), x => x.PlanName.Contains(input.PlanName))
                .WhereIF(input.FileName.IsNotNullOrNotWhiteSpace(), x => x.FileName.Contains(input.FileName))
                .OrderByDescending(x => x.CreationTime)
                .Select<PlanDto>()
                .ToPageListAsync(input.Page, input.Size, totalCount);

            var sqlTempleteIds = plans.Where(x => x.SqlTempleteId.HasValue).Select(x => x.SqlTempleteId.Value).ToList();

            var sqlTempletes = await _sqlSugarClient.Queryable<SqlTemplete>().Where(x => sqlTempleteIds.Contains(x.Id)).Select<SqlTempleteDto>().ToListAsync();

            var userIds = plans.Where(x => x.CreatorId.HasValue).Select(x => x.CreatorId.Value).Distinct().ToList();

            var appusers = await _sqlSugarClient.Queryable<AppUser>().Where(x => userIds.Contains(x.Id)).ToListAsync();

            var warehouseIds = plans.Where(x => x.WarehouseId.HasValue).Select(x => x.WarehouseId.Value).Distinct().ToList();
            if (warehouseIds.Count > 0)
            {
                var warehouseDtos = await _sqlSugarClient.Queryable<Warehouse>().Where(x => warehouseIds.Contains(x.Id)).Select<WarehouseDto>().ToListAsync();
                foreach (var item in plans)
                {
                    item.WarehouseDto = warehouseDtos.FirstOrDefault(x => x.Id == item.WarehouseId) ?? new WarehouseDto();
                }
            }

            foreach (var plan in plans)
            {
                plan.CreatorName = appusers.FirstOrDefault(x => x.Id == plan.CreatorId)?.UserName;
                plan.SqlTemplete = sqlTempletes.FirstOrDefault(x => x.Id == plan.SqlTempleteId);
            }

            return new PagedReuslt<PlanDto>(plans, totalCount);
        }


        public async Task<PagedReuslt<PlanDto>> GetListAsync(PlanPagedInput input)
        {
            RefAsync<int> totalCount = 0;
            var plans = await _sqlSugarClient
                .Queryable<Plan>()
                .WhereIF(input.UserId.HasValue, x => x.CreatorId == input.UserId)
                .WhereIF(input.IsTemplete.HasValue, x => x.IsTemplete == input.IsTemplete)
                .WhereIF(input.PlanId.HasValue, x => x.Id == input.PlanId)
                .WhereIF(input.PlanName.IsNotNullOrNotWhiteSpace(), x => x.PlanName.Contains(input.PlanName))
                .WhereIF(input.FileName.IsNotNullOrNotWhiteSpace(), x => x.FileName.Contains(input.FileName))
                .OrderByDescending(x => x.CreationTime)
                .Select<PlanDto>()
                .ToPageListAsync(input.Page, input.Size, totalCount);


            var planIds = plans.Select(x => x.Id.Value).ToList();

            var sqlTempleteIds = plans.Where(x => x.SqlTempleteId.HasValue).Select(x => x.SqlTempleteId.Value).ToList();

            var modules = (await _systemModuleManager.GetAllModule(new ModulePagedInput() { PlanIds = planIds })).Items;

            var tables = await _tableEntityManager.GetAllListAsync(new TableEntityPagedInput() { PlanIds = planIds });

            var enums = await _sqlSugarClient.Queryable<PlanEnum>().Where(x => planIds.Contains(x.PlanId)).Select<PlanEnumDto>().ToListAsync();

            var sqlTempletes = await _sqlSugarClient.Queryable<SqlTemplete>().Where(x => sqlTempleteIds.Contains(x.Id)).Select<SqlTempleteDto>().ToListAsync();

            var tableIds = tables.Select(x => x.Id).Distinct().ToList();



            foreach (var plan in plans)
            {
                plan.SystemModules = modules.Where(x => x.PlanId == plan.Id).ToList();
                plan.TableEntitys = tables.Where(x => x.PlanId == plan.Id).ToList();
                plan.PlanEnums = enums.Where(x => x.PlanId == plan.Id).ToList();
                plan.SqlTemplete = sqlTempletes.FirstOrDefault(x => x.Id == plan.SqlTempleteId);

            }

            return new PagedReuslt<PlanDto>(plans, totalCount);

        }


        /// <summary>
        /// 得到计划的个数
        /// </summary>
        public async Task<long> GetPlanCountAsync(PlanPagedInput input)
        {

            var count = await _sqlSugarClient
                  .Queryable<Plan>()
                  .WhereIF(input.UserId.HasValue, x => x.CreatorId == input.UserId)
                  .WhereIF(input.IsTemplete.HasValue, x => x.IsTemplete == input.IsTemplete)
                  .WhereIF(input.PlanId.HasValue, x => x.Id == input.PlanId)
                  .WhereIF(input.PlanName.IsNotNullOrNotWhiteSpace(), x => x.PlanName.Contains(input.PlanName))
                  .WhereIF(input.FileName.IsNotNullOrNotWhiteSpace(), x => x.FileName.Contains(input.FileName))
                 .CountAsync();
            return count;
        }


        public async Task CheckPlanIsExistAsync(Guid planId, Guid? userId)
        {
            var count = await _sqlSugarClient.Queryable<Plan>()
                 .WhereIF(userId.HasValue, x => x.CreatorId == userId)
                 .Where(x => x.Id == planId)
                 .CountAsync();
            if (count == 0)
            {
                throw new YouJuException("该计划不存在,或者不属于你");
            }
        }


        /// <summary>
        /// 生成配置
        /// </summary>
        private JObject GetSystemComponentSetting(PlanDto plan)
        {

            var componentSettingDetDtos = plan.SystemModules.SelectMany(x => x.Components).SelectMany(x => x.ComponentSettingDetDtos).ToList();
            //去掉空缺值
            componentSettingDetDtos = componentSettingDetDtos.Where(x => x.Key.IsNotNullOrNotWhiteSpace() && x.Value.IsNotNullOrNotWhiteSpace()).ToList();
            //去掉相同的key并且唯一码也相同的  不同的组件引用了相同功能
            componentSettingDetDtos = componentSettingDetDtos.GroupBy(x => new { x.Key, x.Code }).Select(x => x.First()).ToList();


            var combineSystemComponentSettingDetList = new List<SystemComponentSettingDetDto>();
            //对相同的key进行内容合并
            var componentSettingDeGroup = componentSettingDetDtos.GroupBy(x => new { x.Key }).ToList();
            foreach (var item in componentSettingDeGroup)
            {
                var iterItem = item.ToList();

                combineSystemComponentSettingDetList.Add(new SystemComponentSettingDetDto()
                {
                    Key = item.Key.Key,
                    Value = iterItem.Select(x => x.Value).JoinAsString(Environment.NewLine),

                });

            }
            JObject jsonObject = new JObject();
            foreach (var item in combineSystemComponentSettingDetList)
            {
                jsonObject[item.Key] = item.Value;
            }
            return jsonObject;

        }
        public PlanGenerationWriteInput GetPlanGenerationWriteInput(PlanDto plan)
        {

            #region 值初始化
            plan.PlanEnums = plan.PlanEnums.HasItem() ? plan.PlanEnums : new List<PlanEnumDto>();
            plan.TableEntitys = plan.TableEntitys.HasItem() ? plan.TableEntitys : new List<TableEntityDto>();
            foreach (var item in plan.TableEntitys)
            {
                item.TableSettingDto = item.TableSettingDto ?? new TableSettingDto();
                item.Columns = item.Columns.HasItem() ? item.Columns : new List<ColumnPropDto>();
            }
            #endregion


            //写入模型
            var writeInput = new PlanGenerationWriteInput()
            {
                BackFileName = plan.BackFileName ?? "",
                BackLanguageWay = plan.BackLanguageWay,
                DatabaseConnection = plan.DatabaseConnection ?? "",
                FileName = plan.FileName ?? "",
                SystemComponentSetting = GetSystemComponentSetting(plan),
                PlanName = plan.PlanName ?? "",
                IsMiniProgram = plan.IsMiniProgram,
                BackPort = plan.BackPort,
                PlanEnums = plan.PlanEnums.Select(x => new PlanEnumWriteInput()
                {
                    Code = x.Code,
                    EnumProps = x.EnumProps,
                    Name = x.Name,

                }).ToList(),

                TableEntitys = plan.TableEntitys.Select(x => new TableEntityWriteInput()
                {
                    Code = x.Code ?? "",
                    Name = x.Name ?? "",
                    PlanId = plan.Id.Value,
                    TableEntityId = x.Id.Value,
                    IsExtra = x.IsExtra,

                    TableSetting = new TableSettingInput()
                    {
                        Add = x.TableSettingDto.Add,
                        BatchDelete = x.TableSettingDto.BatchDelete,
                        Delete = x.TableSettingDto.Delete,
                        Edit = x.TableSettingDto.Edit,
                        Export = x.TableSettingDto.Export,
                        Import = x.TableSettingDto.Import,
                        Search = x.TableSettingDto.Search,
                        View = x.TableSettingDto.View,
                        EditFormSettingJson = x.TableSettingDto.EditFormSettingJson,
                        ExportSettingJson = x.TableSettingDto.ExportSettingJson,
                        ImportSettingJson = x.TableSettingDto.ImportSettingJson,
                        SearchFormSettingJson = x.TableSettingDto.SearchFormSettingJson,
                        ViewColumnSettingJson = x.TableSettingDto.ViewColumnSettingJson,

                    },
                    ColumnProps = x.Columns.Select(h => new ColumnPropWriteInput()
                    {
                        ColumnPropId = h.Id.Value,
                        Code = h.Code ?? "",
                        ColumnPropType = h.ColumnPropType,
                        Display = h.Display ?? "",
                        IsNull = h.IsNull,
                        Length = h.Length ?? 0,
                        Name = h.Name ?? "",
                        TableEntityId = h.TableEntityId.Value,
                        EnumCode = h.PlanEnumId.HasValue ? plan.PlanEnums.FirstOrDefault(p => p.Id == h.PlanEnumId.Value)?.Code : "",

                    }).ToList()

                }).ToList()

            };

            var cloneWirteInput = writeInput.Clone<PlanGenerationWriteInput, PlanGenerationWriteInput>();

            var cloneColumns = cloneWirteInput.TableEntitys.SelectMany(x => x.ColumnProps).ToList();

            foreach (var item in writeInput.TableEntitys)
            {

                item.TableNavigateRelatives = plan.TableEntitys.SelectMany(x => x.TableNavigateRelatives).Where(x => x.RelativeTableId == item.TableEntityId).Select(h => new TableNavigateRelativeWirteInput()
                {
                    RelativeTableId = h.RelativeTableId,
                    TableNavigateType = h.TableNavigateType,
                    AssociationATableId = h.AssociationATableId,
                    AssociationATableEntity = cloneWirteInput.TableEntitys.FirstOrDefault(j => j.TableEntityId == h.AssociationATableId),
                    AssociationAColumnId = h.AssociationAColumnId,
                    AssociationAColumnProp = cloneColumns.FirstOrDefault(j => j.ColumnPropId == h.AssociationAColumnId),

                    AssociationBTableId = h.AssociationBTableId,
                    AssociationBTableEntity = cloneWirteInput.TableEntitys.FirstOrDefault(j => j.TableEntityId == h.AssociationBTableId),
                    AssociationBColumnId = h.AssociationBColumnId,
                    AssociationBColumnProp = cloneColumns.FirstOrDefault(j => j.ColumnPropId == h.AssociationBColumnId),

                }).ToList();
            }
            return writeInput;
        }

        /// <summary>
        /// 部分模板生成效果
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PlanGengerationDto> GewtGenerationModelAndTempleteByModuleId(PlanPagedInput input)
        {
            var plan = (await GetListAsync(input)).Items.FirstOrDefault();

            var writeInput = GetPlanGenerationWriteInput(plan);

            var moduleIds = input.ModuleIds;


            var templeteData = await _systemModuleManager.GetGengerationComponentTreeData(moduleIds);

            return new PlanGengerationDto()
            {
                GengerationComponentTreeDatas = templeteData,
                PlanGenerationWriteInput = writeInput
            };
        }

        /// <summary>
        /// 代码生成的实体和模板树的核心逻辑
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PlanGengerationDto> GetGenerationModelAndTempleteByPlan(PlanPagedInput input)
        {
            var plan = (await GetListAsync(input)).Items.FirstOrDefault();

            var writeInput = GetPlanGenerationWriteInput(plan);

            var moduleIds = plan.SystemModules.Select(x => x.Id.Value).ToArray();

            var templeteData = await _systemModuleManager.GetGengerationComponentTreeData(moduleIds);

            return new PlanGengerationDto()
            {
                GengerationComponentTreeDatas = templeteData,
                PlanGenerationWriteInput = writeInput
            };
        }

        /// <summary>
        /// 得到生成数据库模板
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PlanGengerationSqlDto> PlanSqlTempletePagedInput(PlanPagedInput input)
        {
            var plan = (await GetListAsync(input)).Items.FirstOrDefault();
            if (plan.SqlTemplete == null)
            {
                return new PlanGengerationSqlDto();
            }
            var writeInput = GetPlanGenerationWriteInput(plan);

            var sqlResult = new PlanGengerationSqlDto()
            {
                DataBaseType = plan.SqlTemplete.DataBaseType, 
                CreateDbContentResult = plan.SqlTemplete.CreateDbContent.IsNotNullOrNotWhiteSpace() ? ScribanHelper.Render(plan.SqlTemplete.CreateDbContent, new { Model = writeInput }, memberRenamer: (m) => m.Name) : ""
            };




            return sqlResult;
        }


        public static void ValidFiledPlanName(string planName)
        {
            if (planName.IsNullOrWhiteSpace())
            {
                throw new YouJuException("计划名称不能为空");
            }
            if (planName.Length > 50)
            {
                throw new YouJuException("计划名称长度不能超过50个字符");
            }

            var planNameExistChar = planName.CheckExistChar(SysConst.FilterChar);
            if (planNameExistChar.Count > 0)
            {
                throw new YouJuException("不能包含如下字符" + planNameExistChar.JoinAsString(","));
            }

        }
        public static void ValidFiledFileName(string fileName)
        {
            if (fileName.IsNullOrWhiteSpace())
            {
                throw new YouJuException("文件名称不能为空");
            }
            if (fileName.Length > 30)
            {
                throw new YouJuException("文件名称长度不能超过30个字符");
            }
            var fileNameExistChar = fileName.CheckExistChar(SysConst.FilterChar);
            if (fileNameExistChar.Count > 0)
            {
                throw new YouJuException("不能包含如下字符" + fileNameExistChar.JoinAsString(","));
            }
        }
    }
}
