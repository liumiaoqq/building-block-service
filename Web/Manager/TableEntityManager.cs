using MathNet.Numerics.Statistics.Mcmc;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Crypto;
using System.Numerics;
using System.Text.RegularExpressions;
using Web.Dto.Plans;
using Web.Dto.TableEntitys;
using Web.Tables;
using YouJu.Infrastructure;
using YouJu.Infrastructure.Dto;

namespace Web.Manager
{
    public class TableEntityManager
    {
        protected ISqlSugarClient _sqlSugarClient;

        public TableEntityManager(ISqlSugarClient sqlSugarClient)
        {
            _sqlSugarClient = sqlSugarClient;
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public virtual async Task<List<TableEntityDto>> GetListAsync(Guid planId)
        {

            var entitys = await _sqlSugarClient.Queryable<TableEntity>()
                .Where(x => x.PlanId == planId)
                .OrderBy(x => x.Code)
                .Select<TableEntityDto>().ToListAsync();
            return entitys;
        }


        /// <summary>
        /// 检测方案是否存在用户表
        /// </summary>
        public async Task<bool> CheckExistBaseTable(Guid planId)
        {
            return await _sqlSugarClient.Queryable<TableEntity>().Where(x => x.PlanId == planId && SysConst.BaseTableNames.Contains(x.Code)).CountAsync() > 0;
        }

        /// <summary>
        /// 获取项目用户表
        /// </summary>
        public async Task<TableEntityDto> GetBaseTable(Guid planId)
        {

            var tableEntity = await _sqlSugarClient.Queryable<TableEntity>().Where(x => x.PlanId == planId && SysConst.BaseTableNames.Contains(x.Code)).Select<TableEntityDto>().FirstAsync();

            var colums = await _sqlSugarClient.Queryable<ColumnProp>().Where(x => x.TableEntityId == tableEntity.Id).OrderBy(x => x.CreationTime).Select<ColumnPropDto>().ToListAsync();
            tableEntity.Columns = colums;
            return tableEntity;
        }



        #region 批量编辑表模块
        /// <summary>
        /// 批量编辑列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public virtual async Task<List<TableEntityDto>> GetEditListAsync(TableEntityPagedInput input)
        {

            var entitys = await _sqlSugarClient.Queryable<TableEntity>()
                .WhereIF(input.UserId.HasValue, x => x.CreatorId == input.UserId)
                  .Where(x => input.PlanId == x.PlanId)
                  .OrderBy(x => x.Code)
                 .Select<TableEntityDto>().ToListAsync();
            return entitys;
        }

        /// <summary>
        /// 批量创建
        /// </summary>
        public async Task BatchCreateOrEdit(BatchCreateOrEditTableEntityInput input)
        {
            foreach (var item in input.TableEntities)
            {
                item.Code = item.Code.Trim();
                item.Name = item.Name.Trim();

                //编码只能是字母
                if (!Regex.IsMatch(item.Code, "^[a-zA-Z]+$"))
                {
                    throw new YouJuException("表编码只能是字母,请勿输入数字或则其他特殊符号");
                }
                //编码首字母只能是大写
                if (!char.IsUpper(item.Code[0]))
                {
                    throw new YouJuException("表编码首字母只能是大写,请使用首字母大写的驼峰命名规则");
                }
            }


            if (input.TableEntities.GroupBy(x => x.Name).Count(x => x.Count() > 1) > 0)
            {
                throw new YouJuException("存在2个相同的表名称");
            }
            if (input.TableEntities.GroupBy(x => x.Code).Count(x => x.Count() > 1) > 0)
            {
                throw new YouJuException("存在2个相同的表字段");
            }
            foreach (var item in input.TableEntities.Where(x => x.Id == null || x.Id == Guid.Empty))
            {
                item.Id = Guid.NewGuid();
            }
            var inputTableEntityIds = input.TableEntities.Select(x => x.Id.Value).ToList();


            //得到计划下的所有表
            var allTableEntitys = _sqlSugarClient.Queryable<TableEntity>().Where(x => x.PlanId == input.PlanId).ToList();
            var tableEntityIds = allTableEntitys.Select(x => x.Id).Distinct().ToList();

            //得到2个的交集 不需要动
            var intersectIds = inputTableEntityIds.Intersect(tableEntityIds);
            //传入的排除交集就是需要新增的
            var newAddIds = inputTableEntityIds.Where(x => !intersectIds.Contains(x)).ToList();
            //数据库原始的排除交集就是需要删除的
            var deleteIds = tableEntityIds.Where(x => !intersectIds.Contains(x)).ToList();


            if (intersectIds.Count() > 0)
            {
                foreach (var item in intersectIds)
                {
                    var newTableEntity = input.TableEntities.First(x => x.Id == item);
                    var oldTableEntity = allTableEntitys.First(x => x.Id == item);

                    oldTableEntity.Name = newTableEntity.Name;
                    oldTableEntity.IsExtra = newTableEntity.IsExtra;
                    oldTableEntity.IsOpen = newTableEntity.IsOpen;
                    oldTableEntity.PlanId = input.PlanId;
                    oldTableEntity.Code = newTableEntity.Code;
                    await _sqlSugarClient.Updateable(oldTableEntity).ExecuteCommandAsync();

                }

            }
            if (newAddIds.Count > 0)
            {

                var newAddTableEntityes = input.TableEntities.Where(x => newAddIds.Contains(x.Id.Value)).Select(x => new TableEntity()
                {
                    Code = x.Code,
                    IsExtra = x.IsExtra,
                    IsOpen = x.IsOpen,
                    RelativeCount = 0,
                    Name = x.Name,
                    PlanId = input.PlanId,


                }).ToList();
                await _sqlSugarClient.Insertable(newAddTableEntityes).ExecuteCommandAsync();

            }
            if (deleteIds.Count > 0)
            {
                await _sqlSugarClient.Deleteable<TableEntity>().Where(x => deleteIds.Contains(x.Id)).ExecuteCommandAsync();
                await _sqlSugarClient.Deleteable<TableSetting>().Where(x => deleteIds.Contains(x.TableEntityId)).ExecuteCommandAsync();
                await _sqlSugarClient.Deleteable<ColumnProp>().Where(x => deleteIds.Contains(x.TableEntityId)).ExecuteCommandAsync();
                await _sqlSugarClient.Deleteable<TableNavigateRelative>().Where(x => deleteIds.Contains(x.RelativeTableId)).ExecuteCommandAsync();
            }

        }
        #endregion

        #region 分享拷贝表模块
        /// <summary>
        /// 查询目前可分享的表
        /// </summary>
        public async Task<List<TableEntityShareDto>> GetSharePlanTableList(QuerySharePlanTableInput input)
        {

            //得到当前已经存在的表
            var existTableEntitys = await _sqlSugarClient.Queryable<TableEntity>().Where(x => x.PlanId == input.PlanId).ToListAsync();

            var shareTables = await _sqlSugarClient.Queryable<TableEntity>().Where(x => x.IsOpen == true).Select<TableEntityShareItemDto>().ToListAsync();

            var planIds = shareTables.Select(x => x.PlanId).Distinct().ToList();

            var planDtos = await _sqlSugarClient.Queryable<Plan>().WhereIF(input.WarehouseId.HasValue, x => x.WarehouseId == input.WarehouseId).Where(x => planIds.Contains(x.Id)).Select<PlanDto>().ToListAsync();



            var rs = planDtos.Select(x => new TableEntityShareDto()
            {
                PlanName = x.PlanName,
                PlanId = x.Id.Value,

                TableEntityList = shareTables.Where(h => h.PlanId == x.Id).Select(h => new TableEntityShareItemDto()
                {
                    Code = h.Code,
                    Name = h.Name,
                    Id = h.Id,
                    PlanId = h.PlanId,
                    RelativeCount = h.RelativeCount,
                    IsForbidden = existTableEntitys.Exists(j => j.Name.Equals(h.Name) || j.Code.Equals(h.Code))

                }).ToList()
            }).ToList();
            foreach (var item in rs)
            {
                item.CheckIds = new List<Guid>();
                item.ShareCount = item.TableEntityList.Sum(h => h.RelativeCount);
            }

            return rs.OrderByDescending(x => x.ShareCount).ToList();
        }

        /// <summary>
        /// 检测是否存在相同的计划枚举
        /// </summary>
        private async Task<bool> CheckExistEnumsByPlan(Guid planId, string code)
        {

            return (await _sqlSugarClient.Queryable<PlanEnum>().CountAsync(x => x.PlanId == planId && x.Code == code)) > 0;

        }

        /// <summary>
        /// 拷贝前进行检查
        /// </summary>
        private async Task CheckCopyTableEntityToPlan(CopyTableEntityToPlanInput input)
        {

            //得到当前已经存在的表
            var existTableEntitys = await _sqlSugarClient.Queryable<TableEntity>().Where(x => x.PlanId == input.TargetPlanId).ToListAsync();





            var checkTableIds = input.Dets.SelectMany(x => x.CheckIds).Distinct().ToList();
            //得到选择的表
            var checkTableEntitys = await _sqlSugarClient.Queryable<TableEntity>().Where(x => checkTableIds.Contains(x.Id)).ToListAsync();


            if (existTableEntitys.Count == 0 && checkTableEntitys.Count != 1 && checkTableEntitys.Exists(x => SysConst.BaseTableNames.Contains(x.Code)) == false)
            {
                throw new YouJuException("方案没有用户表,不能够选择，并且第一次选择只能选择一个用户表");
            }


            //得到所有的name拼接起来
            var allName = existTableEntitys.Select(x => x.Name).Union(checkTableEntitys.Select(x => x.Name)).ToList();
            var sameName = allName.GroupBy(x => x).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
            if (sameName.Count > 0)
            {
                throw new YouJuException("以下表的名称存在相同," + sameName.JoinAsString(","));
            }
            //得到所有的code拼接起来
            var allCode = existTableEntitys.Select(x => x.Code).Union(checkTableEntitys.Select(x => x.Code)).ToList();
            var sameCode = allCode.GroupBy(x => x).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
            if (sameCode.Count > 0)
            {
                throw new YouJuException("以下表的表英文名称存在相同," + sameCode.JoinAsString(","));
            }
        }


        /// <summary>
        /// 拷贝
        /// </summary>
        public async Task CopyTableEntityToPlan(CopyTableEntityToPlanInput input)
        {

            await this.CheckCopyTableEntityToPlan(input);


            var targetPlanId = await GetAllListAsync(new TableEntityPagedInput() { PlanId = input.TargetPlanId });

            var userTable = targetPlanId.FirstOrDefault(x => SysConst.BaseTableNames.Contains(x.Code));

            var planIds = input.Dets.Select(x => x.PlanId).ToList();
            //得到计划下的所有表
            var allTables = await GetAllListAsync(new TableEntityPagedInput() { PlanIds = planIds });


            if (userTable == null)
            {
                await CopyUserTableEntity(input, allTables);
            }
            else
            {

                #region  检查每个实体的主外键关联是否正常
                foreach (var planId in planIds)
                {
                    var relativeTables = allTables.Where(x => x.PlanId == planId).ToList();
                    //当前计划选中的表
                    var iterChckedTableIds = input.Dets.First(x => x.PlanId == planId).CheckIds;
                    foreach (var tableId in iterChckedTableIds)
                    {
                        var tableNavigateRelatives = relativeTables.FirstOrDefault(x => x.Id == tableId).TableNavigateRelatives;

                        //排除用户表
                        tableNavigateRelatives = tableNavigateRelatives.Where(x => x.TableNavigateType == TableNavigateType.OneToOne &&!SysConst.BaseTableNames.Contains(x.AssociationATableEntity.Code)).ToList();

                        var noAttachTables = tableNavigateRelatives.Where(x => iterChckedTableIds.Exists(h => h == x.AssociationATableId.Value) == false).ToList();

                        if (noAttachTables.Count > 0)
                        {
                            var exContent = noAttachTables.Select(x => $"表:{x.AssociationATableEntity.Name}({x.AssociationATableEntity.Code})").JoinAsString(",");
                            throw new YouJuException("存在关联但是没有选中的表如下:" + exContent);
                        }

                    }

                }
                #endregion

                foreach (var planId in planIds)
                {
                    var relativeTables = allTables.Where(x => x.PlanId == planId).ToList();
                    //当前计划选中的表
                    var iterCheckedTableIds = input.Dets.First(x => x.PlanId == planId).CheckIds;

                    var checkTables = allTables.Where(x => iterCheckedTableIds.Contains(x.Id.Value)).ToList();



                    var targetTableEntityList = new List<TableEntity>();
                    var tableSettingList = new List<TableSetting>();
                    var targetColumnPropList = new List<ColumnProp>();

                    foreach (var orginTableEntity in checkTables)
                    {

                        //分享次数+1 不考虑性能 这个地方性能不会拉跨
                        await _sqlSugarClient.Updateable<TableEntity>().Where(x => x.Id == orginTableEntity.Id).SetColumns(x => x.RelativeCount == x.RelativeCount + 1).ExecuteCommandAsync();

                        var targetTableEntity = new TableEntity()
                        {
                            Id = Guid.NewGuid(),
                            Code = orginTableEntity.Code,
                            PlanId = input.TargetPlanId,
                            IsOpen = false,
                            RelativeCount = 0,
                            Name = orginTableEntity.Name,
                            IsBuiltin = orginTableEntity.IsBuiltin,
                            IsExtra = orginTableEntity.IsExtra,

                        };


                        if (orginTableEntity.TableSettingDto != null)
                        {
                            var tableSetting = new TableSetting()
                            {
                                TableEntityId = targetTableEntity.Id,
                                PlanId = input.TargetPlanId,
                                Id = Guid.NewGuid(),
                                Add = orginTableEntity.TableSettingDto.Add,
                                BatchDelete = orginTableEntity.TableSettingDto.BatchDelete,
                                Delete = orginTableEntity.TableSettingDto.Delete,
                                Edit = orginTableEntity.TableSettingDto.Edit,
                                Export = orginTableEntity.TableSettingDto.Export,
                                Import = orginTableEntity.TableSettingDto.Import,
                                Search = orginTableEntity.TableSettingDto.Search,
                                View = orginTableEntity.TableSettingDto.View,
                                ExportSettingJson = orginTableEntity.TableSettingDto.ExportSettingJson,
                                EditFormSettingJson = orginTableEntity.TableSettingDto.EditFormSettingJson,
                                ImportSettingJson = orginTableEntity.TableSettingDto.ImportSettingJson,
                                SearchFormSettingJson = orginTableEntity.TableSettingDto.SearchFormSettingJson,
                                ViewColumnSettingJson = orginTableEntity.TableSettingDto.ViewColumnSettingJson,

                            };
                            tableSettingList.Add(tableSetting);


                        }

                        if (orginTableEntity.Columns.HasItem())
                        {
                            foreach (var columns in orginTableEntity.Columns)
                            {
                                var targetColumn = new ColumnProp()
                                {
                                    Code = columns.Code,
                                    Name = columns.Name,
                                    Display = columns.Display,
                                    IsNull = columns.IsNull,
                                    Length = columns.Length,
                                    TableEntityId = targetTableEntity.Id,
                                    Id = Guid.NewGuid(),
                                    ColumnPropType = columns.ColumnPropType,

                                };



                                #region 如果存在枚举进行copy并且替换
                                if (columns.ColumnPropType == ColumnPropType.枚举型 && columns.PlanEnumId.HasValue)
                                {
                                    var planEnums = await _sqlSugarClient.Queryable<PlanEnum>().Where(x => x.Id == columns.PlanEnumId).ToListAsync();

                                    if (planEnums != null && planEnums.Count > 0)
                                    {
                                        if (!await CheckExistEnumsByPlan(input.TargetPlanId, planEnums.First().Code))
                                        {

                                            var newPlanEnum = new PlanEnum()
                                            {
                                                Code = planEnums.First().Code,
                                                EnumProps = planEnums.First().EnumProps,
                                                Name = planEnums.First().Name,
                                                PlanId = input.TargetPlanId,

                                            };

                                            newPlanEnum = await _sqlSugarClient.Insertable(newPlanEnum).ExecuteReturnEntityAsync();
                                            targetColumn.PlanEnumId = newPlanEnum.Id;
                                        }


                                    }

                                }
                                #endregion


                                targetColumnPropList.Add(targetColumn);
                            }

                        }


                        targetTableEntityList.Add(targetTableEntity);
                    }
                    //插入表
                    if (targetTableEntityList.Count > 0)
                    {

                        await _sqlSugarClient.Insertable(targetTableEntityList).ExecuteCommandAsync();
                    }
                    //插入表配置
                    if (tableSettingList.Count > 0)
                    {
                        await _sqlSugarClient.Insertable(tableSettingList).ExecuteCommandAsync();
                    }
                    //插入表列
                    if (targetColumnPropList.HasItem())
                    {
                        await _sqlSugarClient.Insertable(targetColumnPropList).ExecuteCommandAsync();
                    }


                    var copyTableNavigateRelatives = new List<TableNavigateRelative>();


                    #region 如果存在外键进行copy并且替换
                    foreach (var orginTableEntity in checkTables)
                    {
                        var tableNavigateRelatives = orginTableEntity.TableNavigateRelatives;

                        tableNavigateRelatives = tableNavigateRelatives.Where(x => x.TableNavigateType == TableNavigateType.OneToOne).ToList();
                        if (tableNavigateRelatives.Count > 0)
                        {
                            //老表的关系
                            foreach (var tableNavigateRelative in tableNavigateRelatives)
                            {
                                //如果关联的是用户表
                                if (SysConst.BaseTableNames.Contains(tableNavigateRelative.AssociationATableEntity.Code))
                                {

                                    //查询到对应的新表
                                    var targetTable = targetTableEntityList.First(x => x.Code == tableNavigateRelative.RelativeTable.Code);

                                    var targetColumn = targetColumnPropList.Where(x => x.TableEntityId == targetTable.Id).First(x => x.Code == tableNavigateRelative.AssociationAColumnPropDto.Code);

                                    var newTableNavigateRelative = new TableNavigateRelative()
                                    {
                                        RelativeTableId = targetTable.Id,
                                        TableNavigateType = TableNavigateType.OneToOne,
                                        AssociationAColumnId = targetColumn.Id,
                                        AssociationATableId = userTable.Id,

                                    };

                                    copyTableNavigateRelatives.Add(newTableNavigateRelative);
                                }
                                else
                                {


                                    //查询关联对应的新表
                                    var targetAssociationATable = targetTableEntityList.First(x => x.Code == tableNavigateRelative.AssociationATableEntity.Code);
                                    //查询到对应的新表
                                    var targetTable = targetTableEntityList.First(x => x.Code == tableNavigateRelative.RelativeTable.Code);

                                    var targetColumn = targetColumnPropList.Where(x => x.TableEntityId == targetTable.Id).First(x => x.Code == tableNavigateRelative.AssociationAColumnPropDto.Code);

                                    var newTableNavigateRelative = new TableNavigateRelative()
                                    {
                                        RelativeTableId = targetTable.Id,
                                        TableNavigateType = TableNavigateType.OneToOne,
                                        AssociationAColumnId = targetColumn.Id,
                                        AssociationATableId = targetAssociationATable.Id,
                                    };

                                    copyTableNavigateRelatives.Add(newTableNavigateRelative);
                                }

                            }



                        }

                    }
                    //插入表列
                    if (copyTableNavigateRelatives.HasItem())
                    {
                        await _sqlSugarClient.Insertable(copyTableNavigateRelatives).ExecuteCommandAsync();
                    }


                    #endregion



                }
            }


        }



        public async Task CopyUserTableEntity(CopyTableEntityToPlanInput input, List<TableEntityDto> allTables)
        {
            //得到用户勾选了的表
            var checkTableIds = input.Dets.SelectMany(x => x.CheckIds).Distinct().ToList();

            var checkTables = allTables.Where(x => checkTableIds.Contains(x.Id.Value)).ToList();



            foreach (var item in checkTables)
            {
                var orginTableEntity = item;

                //分享次数+1 不考虑性能 这个地方性能不会拉跨
                await _sqlSugarClient.Updateable<TableEntity>().Where(x => x.Id == orginTableEntity.Id).SetColumns(x => x.RelativeCount == x.RelativeCount + 1).ExecuteCommandAsync();

                orginTableEntity.Id = Guid.NewGuid();
                orginTableEntity.PlanId = input.TargetPlanId;
                orginTableEntity.IsOpen = false;
                orginTableEntity.RelativeCount = 0;
                if (orginTableEntity.TableSettingDto != null)
                {

                    orginTableEntity.TableSettingDto.TableEntityId = orginTableEntity.Id.Value;
                    orginTableEntity.TableSettingDto.PlanId = input.TargetPlanId;
                    orginTableEntity.TableSettingDto.Id = Guid.NewGuid();
                }

                if (orginTableEntity.Columns.HasItem())
                {
                    foreach (var columns in orginTableEntity.Columns)
                    {

                        columns.Id = Guid.NewGuid();
                        columns.TableEntityId = orginTableEntity.Id.Value;
                        columns.PlanId = input.TargetPlanId;

                        #region 如果存在枚举进行copy并且替换
                        if (columns.ColumnPropType == ColumnPropType.枚举型 && columns.PlanEnumId.HasValue)
                        {
                            var planEnums = await _sqlSugarClient.Queryable<PlanEnum>().Where(x => x.Id == columns.PlanEnumId).ToListAsync();

                            if (planEnums != null && planEnums.Count > 0)
                            {
                                if (!await CheckExistEnumsByPlan(input.TargetPlanId, planEnums.First().Code))
                                {

                                    var newPlanEnum = new PlanEnum()
                                    {
                                        Code = planEnums.First().Code,
                                        EnumProps = planEnums.First().EnumProps,
                                        Name = planEnums.First().Name,
                                        PlanId = input.TargetPlanId,
                                    };

                                    newPlanEnum = await _sqlSugarClient.Insertable(newPlanEnum).ExecuteReturnEntityAsync();
                                    columns.PlanEnumId = newPlanEnum.Id;
                                }


                            }

                        }
                        #endregion
                    }
                }

                //插入表
                await _sqlSugarClient.Insertable(orginTableEntity.Clone<TableEntityDto, TableEntity>()).ExecuteCommandAsync();

                //插入表配置
                if (orginTableEntity.TableSettingDto != null)
                {
                    await _sqlSugarClient.Insertable(orginTableEntity.TableSettingDto.Clone<TableSettingDto, TableSetting>()).ExecuteCommandAsync();
                }
                //插入表列
                if (orginTableEntity.Columns.HasItem())
                {
                    await _sqlSugarClient.Insertable(orginTableEntity.Columns.Clone<List<ColumnPropDto>, List<ColumnProp>>()).ExecuteCommandAsync();
                }
            }
        }

        #endregion



        #region 批量导入创建表

        public async Task BatchCreateTableAsync(TableEntitySqlScriptSubmitDto input)
        {

            var baseTable = await GetBaseTable(input.PlanId.Value);

            //得到计划下的所有表
            var allTables = await GetAllListAsync(new TableEntityPagedInput() { PlanIds = new List<Guid>() { input.PlanId.Value } });

            List<String> errorList = new List<String>();
            foreach (var table in input.TableEntityDtos)
            {
                //检测是否存在相同的表名字或者表编码 并且忽略大小写
                if (SysConst.BaseTableNames.Contains(table.Code))
                {
                    continue;
                }
                if (allTables.Any(x => x.Name.Equals(table.Name, StringComparison.OrdinalIgnoreCase) || x.Code.Equals(table.Code, StringComparison.OrdinalIgnoreCase)))
                {
                    errorList.Add($"表{table.Name}或者表编码{table.Code}已存在,请勿重复导入!");
                }
                ValidFiledCode(table.Code);
                ValidFiledName(table.Name);
            }
            if (errorList.Count > 0)
            {
                throw new YouJuException(errorList.JoinAsString(","));
            }

            //开始完成导入 
            //1.先完成表的创建
            //2.在完成表的列创建
            //3.在进行表和表之间的关联

            // 1. 创建表实体
            var tableEntities = new List<TableEntity>();

            var columnProps = new List<ColumnProp>();
            var tableRelatives = new List<TableNavigateRelative>();


            foreach (var tableDto in input.TableEntityDtos)
            {
                if (SysConst.BaseTableNames.Contains(tableDto.Code))
                {
                    continue;
                }

                // 创建表实体
                var tableEntity = new TableEntity
                {
                    Id = tableDto.Id.Value,
                    Code = tableDto.Code,
                    Name = tableDto.Name,
                    PlanId = input.PlanId.Value,
                    IsOpen = tableDto.IsOpen,
                    IsExtra = tableDto.IsExtra,
                    IsBuiltin = false,
                    RelativeCount = 0
                };
                tableEntities.Add(tableEntity);

                // 2. 创建列
                if (tableDto.Columns != null && tableDto.Columns.Count > 0)
                
                {
                    var index = 0;
                    foreach (var columnDto in tableDto.Columns)
                    {
                        index++;

                        var columnProp = new ColumnProp
                        {
                            Id = columnDto.Id.Value,
                            TableEntityId = tableDto.Id.Value,
                            Code = columnDto.Code,
                            Name = columnDto.Name,
                            ColumnPropType = columnDto.ColumnPropType,
                            IsNull = columnDto.IsNull,
                            Length = columnDto.Length,
                            Display = columnDto.Display,
                            PlanEnumId = columnDto.PlanEnumId,
                            CreationTime=DateTime.Now.AddSeconds(index),
                        };


                        columnProps.Add(columnProp);
                    }
                }
            }

            // 3. 创建表之间的关联
            foreach (var tableDto in input.TableEntityDtos)
            {

                foreach (var relativeDto in tableDto.TableNavigateRelatives)
                {
                    // 跳过无效的关联
                    if (relativeDto.AssociationATableId == null || relativeDto.AssociationAColumnId == null)
                    {
                        continue;
                    }


                    var tableRelative = new TableNavigateRelative
                    {
                        Id = relativeDto.Id.Value,
                        RelativeTableId = relativeDto.RelativeTableId,
                        TableNavigateType = relativeDto.TableNavigateType,
                        AssociationATableId = relativeDto.AssociationATableId,
                        AssociationAColumnId = relativeDto.AssociationAColumnId,
                        AssociationBTableId = relativeDto.AssociationBTableId,
                        AssociationBColumnId = relativeDto.AssociationBColumnId
                    };

                    tableRelatives.Add(tableRelative);

                }
            }

            // 批量插入数据
            if (tableEntities.Count > 0)
            {
                await _sqlSugarClient.Insertable(tableEntities).ExecuteCommandAsync();
            }



            if (columnProps.Count > 0)
            {
                await _sqlSugarClient.Insertable(columnProps).ExecuteCommandAsync();
            }

            if (tableRelatives.Count > 0)
            {
                await _sqlSugarClient.Insertable(tableRelatives).ExecuteCommandAsync();
            }
        }
        #endregion


        #region AI转换表实体名称

        public async Task AIConvertTableEntityNameSave(List<AIConvertTableEntityNameDto> input)
        {

            var tableEntityIds = input.Select(x => x.TableEntityId).ToList();

            //得到这个计划下的所有实体
            var tableEntitys = _sqlSugarClient.Queryable<TableEntity>().Where(x => tableEntityIds.Contains(x.Id)).ToList();
            //循环input
            foreach (var item in input)
            {
                //找到对应的实体
                var tableEntity = tableEntitys.Where(x => x.Id == item.TableEntityId).FirstOrDefault();
                //更新实体的编码
                tableEntity.Code = item.NewCode;
            }
            await _sqlSugarClient.Updateable(tableEntitys).ExecuteCommandAsync();
        }

        #endregion

        public virtual async Task<TableEntityDto> CreateOrEditAsync(TableEntityDto input)
        {


            input.Code = input.Code.Trim();
            input.Name = input.Name.Trim();
            input.GuidNullToEmpty();
            var entity = await _sqlSugarClient.Queryable<TableEntity>().FirstAsync(x => x.Id == input.Id);
            if (entity is null)
            {
                input.Id = Guid.Empty;
                entity = input.Clone<TableEntityDto, TableEntity>();


                entity = await _sqlSugarClient.Insertable(entity).ExecuteReturnEntityAsync();
            }
            else
            {

                entity = input.Clone<TableEntityDto, TableEntity>();

                await _sqlSugarClient.Updateable(entity).ExecuteCommandAsync();
            }
            return entity.Clone<TableEntity, TableEntityDto>();
        }

        public async Task DeleteAsync(IdInput<Guid> input)
        {

            await _sqlSugarClient.Deleteable<TableEntity>().Where(x => x.Id == input.Id).ExecuteCommandAsync();
            await _sqlSugarClient.Deleteable<ColumnProp>()
                .Where(x => x.TableEntityId == input.Id)
                .ExecuteCommandAsync();
            await _sqlSugarClient.Deleteable<TableNavigateRelative>()
                .Where(x => x.AssociationATableId == input.Id || x.AssociationBTableId == input.Id)
                .ExecuteCommandAsync();
            await _sqlSugarClient.Deleteable<TableSetting>().Where(x => x.TableEntityId == input.Id).ExecuteCommandAsync();
        }


        public async Task<List<TableEntityDto>> GetAllListAsync(TableEntityPagedInput input)
        {

            var entitys = await _sqlSugarClient.Queryable<TableEntity>()
                .WhereIF(input.PlanId.HasValue, x => input.PlanId == x.PlanId)
                .WhereIF(input.PlanIds.HasItem(), x => input.PlanIds.Contains(x.PlanId))
                .WhereIF(input.TableEntityId.HasValue, x => input.TableEntityId == x.Id)
                .WhereIF(input.TableEntityIds.Count > 0, x => input.TableEntityIds.Contains(x.Id))
                .OrderBy(x => x.Code)
               .Select<TableEntityDto>().ToListAsync();

            var entityIds = entitys.Select(x => x.Id).ToList();

            var colums = await _sqlSugarClient.Queryable<ColumnProp>().Where(x => entityIds.Contains(x.TableEntityId)).OrderBy(x => x.CreationTime).Select<ColumnPropDto>().ToListAsync();

            var settings = await _sqlSugarClient.Queryable<TableSetting>().Where(x => entityIds.Contains(x.TableEntityId)).Select<TableSettingDto>().ToListAsync();

            var tableNavigateRelative = await _sqlSugarClient.Queryable<TableNavigateRelative>().Where(x => entityIds.Contains(x.RelativeTableId)).Select<TableNavigateRelativeDto>().ToListAsync();

            foreach (var entity in entitys)
            {

                entity.TableNavigateRelatives = tableNavigateRelative.Where(x => x.RelativeTableId == entity.Id).ToList();

                foreach (var item in entity.TableNavigateRelatives)
                {
                    item.RelativeTable = entity;
                    item.AssociationATableEntity = entitys.FirstOrDefault(x => x.Id == item.AssociationATableId);
                    item.AssociationAColumnPropDto = colums.FirstOrDefault(x => x.Id == item.AssociationAColumnId);

                    item.AssociationBTableEntity = entitys.FirstOrDefault(x => x.Id == item.AssociationBTableId);
                    item.AssociationBColumnPropDto = colums.FirstOrDefault(x => x.Id == item.AssociationBColumnId);
                }
                entity.Columns = colums.Where(x => x.TableEntityId == entity.Id).ToList();
                entity.TableSettingDto = settings.Where(x => x.TableEntityId == entity.Id).FirstOrDefault();
            }

            return entitys;
        }

        public async Task<List<TableTreeDto>> GetTableTreeMenu(TableEntityPagedInput input)
        {
            var lists = await GetAllListAsync(input);
            List<TableTreeDto> tree = new List<TableTreeDto>();
            foreach (var table in lists)
            {
                var tb = new TableTreeDto()
                {
                    Code = table.Code,
                    Label = $"{table.Name}",
                    IsTable = true,
                    PlanId = input.PlanId,
                    IsExtra = table.IsExtra,
                    IsBuiltin = table.IsBuiltin,
                    IsCanEdit = !table.IsBuiltin,
                    IsCanDelete = !table.IsBuiltin,
                    Id = table.Id.Value,
                    IsSeetingSaved = table.TableSettingDto?.Id != null,
                    TableNavigateRelativeCount = table.TableNavigateRelatives.Count,
                    ColumnCount = table.Columns.Count,
                };
                foreach (var column in table.Columns)
                {
                    var col = new TableTreeDto()
                    {
                        Code = column.Code,
                        Label = $"{column.Code}({column.ColumnPropType.ToDescription()})",
                        IsCol = true,
                        ParentId = tb.Id,
                        PlanId = input.PlanId,
                        Id = column.Id.Value,
                    };
                    tb.Children.Add(col);
                }
                tree.Add(tb);
            }
            return tree;



        }



        public async Task<long> GetTableEntityCountAsync(Guid planId, Guid? userId)
        {
            return await _sqlSugarClient.Queryable<TableEntity>()
                 .WhereIF(userId.HasValue, x => x.CreatorId == userId)
                 .Where(x => x.PlanId == planId)
                 .CountAsync();

        }


        public static void ValidFiledName(string value)
        {
            if (value.IsNullOrWhiteSpace())
            {
                throw new YouJuException("实体名称不能为空");
            }
            if (value.Length > 40)
            {
                throw new YouJuException("实体名称长度不能超过40个字符");
            }

            var existChar = value.CheckExistChar(SysConst.FilterChar);
            if (existChar.Count > 0)
            {
                throw new YouJuException("不能包含如下字符" + existChar.JoinAsString(","));
            }

        }
        public static void ValidFiledCode(string value)
        {
            if (value.IsNullOrWhiteSpace())
            {
                throw new YouJuException("实体编码不能为空");
            }
            if (value.Length > 40)
            {
                throw new YouJuException("实体编码长度不能超过40个字符");
            }

            var existChar = value.CheckExistChar(SysConst.FilterChar);
            if (existChar.Count > 0)
            {
                throw new YouJuException("不能包含如下字符" + existChar.JoinAsString(","));
            }

        }


    }
}
