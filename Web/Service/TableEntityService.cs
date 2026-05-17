using Web.Manager;
using YouJu.Infrastructure.DbSqlScripts;
using System.Text.RegularExpressions;
using Web.HttpClient;
using Web.HttpClientApi.DeepSeek.Service;
using Web.Dto.Plans;
namespace Web.Service
{
    public class TableEntityService
    {

        private readonly TableEntityManager _tableEntityManager;


        private ISqlSugarClient _sqlSugarClient;

        private ICurrentUser _currentUser;

        private readonly DicManager _dicManager;

        private readonly TableNavigateRelativeManager _tableNavigateRelativeManager;

        private readonly ExportRuleManager _exportRuleManager;

        private readonly ColumnPropManager _columnPropManager;

        private readonly PlanEnumManager _planEnumManager;

        private readonly IDeepSeekService _deepSeekService;

        public TableEntityService(TableEntityManager tableEntityManager, ISqlSugarClient sqlSugarClient, ICurrentUser currentUser, DicManager dicManager, TableNavigateRelativeManager tableNavigateRelativeManager, ExportRuleManager exportRuleManager, ColumnPropManager columnPropManager, IDeepSeekService deepSeekService, PlanEnumManager planEnumManager)
        {
            _tableEntityManager = tableEntityManager;
            _sqlSugarClient = sqlSugarClient;
            _currentUser = currentUser;
            _dicManager = dicManager;
            _tableNavigateRelativeManager = tableNavigateRelativeManager;
            _exportRuleManager = exportRuleManager;
            _columnPropManager = columnPropManager;
            _deepSeekService = deepSeekService;
            _planEnumManager = planEnumManager;
        }


        public async Task<List<TableEntityDto>> GetEditListAsync(TableEntityPagedInput input)
        {
            if (_currentUser.GetRoleType() == RoleType.用户)
            {
                var userId = _currentUser.GetUserId();
                input.UserId = userId;
                if (input.PlanId.HasValue == false)
                {
                    throw new YouJuException("PlanId丢失,请联系字母哥");
                }
            }
            return await _tableEntityManager.GetEditListAsync(input);
        }


        public async Task DeleteAsync(IdInput<Guid> input)
        {
            if (_currentUser.GetRoleType() == RoleType.用户)
            {
                var userId = _currentUser.GetUserId();
                var count = await _sqlSugarClient.Queryable<TableEntity>().CountAsync(x => x.Id == input.Id && x.CreatorId == userId);
                if (count == 0)
                {
                    throw new YouJuException("没有找到可删除的数据");
                }
            }

            await _tableEntityManager.DeleteAsync(input);
        }
        public async Task<TableEntityDto> CreateOrEditAsync(TableEntityDto input)
        {
            TableEntityManager.ValidFiledName(input.Name);
            TableEntityManager.ValidFiledCode(input.Code);
            if (_currentUser.IsUser())
            {
                input.IsExtra = false;
                input.IsOpen = false;

                var exsitCount = await _tableEntityManager.GetTableEntityCountAsync(input.PlanId, _currentUser.GetUserId());
                if (exsitCount > 40)
                {
                    throw new YouJuException("创建表的数量超过上限40个,如有商业需求,请联系字母哥");

                }
            }


            return await _tableEntityManager.CreateOrEditAsync(input);
        }


        public async Task BatchCreateOrEdit(BatchCreateOrEditTableEntityInput input)
        {

            if (_currentUser.IsUser())
            {
                var userId = _currentUser.GetUserId();
                if (input.TableEntities.Count > 40)
                {
                    throw new YouJuException("创建表的数量超过上限40个,如有商业需求,请联系字母哥");

                }
                foreach (var tableEntitie in input.TableEntities)
                {
                    TableEntityManager.ValidFiledName(tableEntitie.Name);
                    TableEntityManager.ValidFiledCode(tableEntitie.Code);
                    if (tableEntitie.IsExtra == true)
                    {
                    }
                    else
                    {
                        tableEntitie.IsExtra = false;
                    }
                    tableEntitie.IsOpen = false;
                }
            }

            await _tableEntityManager.BatchCreateOrEdit(input);

        }

        #region 命名转换

        /// <summary>
        /// 将表名和列名转换为大写驼峰命名（PascalCase）
        /// 支持蛇形命名(user_code)、小写驼峰(userCode)等格式
        /// </summary>
        private void ConvertToPascalCase(List<TableDefinition> tables)
        {
            foreach (var table in tables)
            {
                // 转换表名
                table.TableCode = table.TableCode.ToPascalCase();

                // 转换列名
                foreach (var column in table.Columns)
                {
                    column.Code = column.Code.ToPascalCase();
                    
                    // 处理列名：去掉括号及内容，原始值保留到Content作为解释
                    if (column.Name.HasBracketContent())
                    {
                        column.Content = column.Name;
                        column.Name = column.Name.RemoveBracketContent();
                    }
                }
            }
        }

        #endregion

        #region SQL脚本自动识别
        /// <summary>
        /// 加载sql
        /// </summary>
        public async Task<List<ColumnPropDto>> LoadSqlScript(TableEntitySqlScriptInput input)
        {

            return await this.AnalyzeMySqlSqlScript(input);
        }

        public Task<List<ColumnPropDto>> AnalyzeMySqlSqlScript(TableEntitySqlScriptInput input)
        {

            var tables = SqlParserExtension.ParseMysqlCreateTableStatements(input.Content);
            if (tables.Count == 0)
            {
                throw new YouJuException("没有识别出有效表结构");
            }
            if (tables.Count > 1)
            {
                throw new YouJuException("识别出多张表");
            }
            var result = new List<ColumnPropDto>();
            foreach (var tableEntity in tables)
            {
                foreach (var columnEntity in tableEntity.Columns)
                {
                    var column = ColumnDefinitionCovertToColumnPropDto(columnEntity);
                    if (column != null)
                    {
                        column.PlanId = input.PlanId;
                        result.Add(column);
                    }
                }
            }
            return Task.FromResult(result);


        }





        /// <summary>
        /// 分析整个mysql脚本
        /// </summary>
        public async Task<List<TableEntityDto>> AnalyzeWholeMySqlSqlScript(TableEntitySqlScriptInput input)
        {

            //控制input的内容大小不要超过2mb
            if (input.Content.Length > 2 * 1024 * 1024)
            {
                throw new YouJuException("sql脚本内容超过2mb,请检查sql脚本,注意不要传入sql的数据进来");
            }
            //检测方案是否存在用户表
            var existBaseTable = await _tableEntityManager.CheckExistBaseTable(input.PlanId.Value);
            if (existBaseTable == false)
            {
                throw new YouJuException("方案没有用户表,不能够导入,请先完成用户表表的创建!");
            }

            var tableEntityDtos = new List<TableEntityDto>();

            // 将SQL脚本按表分割处理
            var sqlBlocks = SplitSqlByTables(input.Content);

            List<TableDefinition> tables = new List<TableDefinition>();
            foreach (var sqlBlock in sqlBlocks)
            {
                // 对每个表的SQL块单独解析
                tables.AddRange(SqlParserExtension.ParseMysqlCreateTableStatements(sqlBlock));

            }

            // 将表名和列名转换为大写驼峰命名
            ConvertToPascalCase(tables);


            foreach (var tableEntity in tables)
            {
                if (SysConst.BaseTableNames.Exists(x=>x.Equals(tableEntity.TableCode,StringComparison.InvariantCultureIgnoreCase)))
                {
                    continue;
                }

                var tableEntityDto = new TableEntityDto();
                tableEntityDto.Id = Guid.NewGuid();
                tableEntityDto.Name = tableEntity.TableName;
                tableEntityDto.Code = tableEntity.TableCode;
                tableEntityDto.PlanId = input.PlanId.Value;
                tableEntityDto.Columns = new List<ColumnPropDto>();
                var index = 0;
                foreach (var columnEntity in tableEntity.Columns)
                {
                    index++;
                    var column = ColumnDefinitionCovertToColumnPropDto(columnEntity);
                    if (column != null)
                    {
                        column.Id = Guid.NewGuid();
                        column.PlanId = input.PlanId;
                        column.CreationTime = DateTime.Now.AddSeconds(index);
                        tableEntityDto.Columns.Add(column);
                    }
                }
                tableEntityDtos.Add(tableEntityDto);
            }

            //获取项目用户表
            var baseTable = await _tableEntityManager.GetBaseTable(input.PlanId.Value);
            tableEntityDtos.Add(baseTable);

            foreach (var tableEntity in tableEntityDtos)
            {

                var table = tables.Where(x => x.TableName.Equals(tableEntity.Name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                if (table != null)
                {
                    foreach (var relation in table.Relations)
                    {
                        var tableNavigateRelativeDto = new TableNavigateRelativeDto();
                        tableNavigateRelativeDto.Id = Guid.NewGuid();
                        tableNavigateRelativeDto.TableNavigateType = TableNavigateType.OneToOne;
                        tableNavigateRelativeDto.RelativeTableId = tableEntity.Id.Value;
                        tableNavigateRelativeDto.RelativeTable = tableEntity;

                        //得到关联的表
                        var relativeTable = tableEntityDtos.Where(x => x.Code.Equals(relation.RefTableCode, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                        //不存在这个表 则直接跳过关联
                        if (relativeTable == null)
                        {
                            continue;
                        }

                        //得到关联表下面的关联列
                        var tableEntityColumn = tableEntity.Columns.Where(x => x.Code.Equals(relation.ColumnCode, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                        tableNavigateRelativeDto.AssociationATableId = relativeTable.Id.Value;
                        tableNavigateRelativeDto.AssociationATableEntity = relativeTable;
                        if (tableEntityColumn != null)
                        {
                            tableNavigateRelativeDto.AssociationAColumnId = tableEntityColumn.Id.Value;
                            tableNavigateRelativeDto.AssociationAColumnPropDto = tableEntityColumn;
                            tableEntity.TableNavigateRelatives.Add(tableNavigateRelativeDto);
                        }
                    }
                }

            }





            if (tableEntityDtos.Count == 0)
            {
                throw new YouJuException("没有识别出有效表结构");
            }
            tableEntityDtos = tableEntityDtos.OrderBy(x => x.Code).ToList();
            return tableEntityDtos;
        }


        /// <summary>
        /// 批量创建表
        /// </summary>
        public async Task BatchCreateTableAsync(TableEntitySqlScriptSubmitDto input)
        {
            await _tableEntityManager.BatchCreateTableAsync(input);
        }



        /// <summary>
        /// 将SQL脚本按表分割成多个块
        /// </summary>
        private List<string> SplitSqlByTables(string sqlContent)
        {
            var result = new List<string>();

            // 使用正则表达式匹配每个CREATE TABLE语句块
            var pattern = @"DROP\s+TABLE\s+IF\s+EXISTS\s+[^;]+;\s*CREATE\s+TABLE\s+[^;]+;";
            var matches = Regex.Matches(sqlContent, pattern, RegexOptions.Singleline);

            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    result.Add(match.Value);
                }
            }

            // 如果没有匹配到DROP TABLE语句，尝试直接匹配CREATE TABLE语句
            if (result.Count == 0)
            {
                pattern = @"CREATE\s+TABLE\s+[^;]+;";
                matches = Regex.Matches(sqlContent, pattern, RegexOptions.Singleline);

                foreach (Match match in matches)
                {
                    if (match.Success)
                    {
                        result.Add(match.Value);
                    }
                }
            }

            return result;
        }


        private ColumnPropDto ColumnDefinitionCovertToColumnPropDto(ColumnDefinition columnDefinition)
        {
            var filterValues = new List<string>() { "CreatedAt", "ID", "创建时间", "创建人ID", "更新时间", "更新人ID", "CreatorId", "UpdatedAt" };

            if (filterValues.Contains(columnDefinition.Code, StringComparer.OrdinalIgnoreCase))
            {
                return null;
            }

            if (filterValues.Contains(columnDefinition.Name, StringComparer.OrdinalIgnoreCase))
            {
                return null;
            }




            if (columnDefinition.Name != null && columnDefinition.Name.Contains("ID", StringComparison.OrdinalIgnoreCase))
            {
                var start = columnDefinition.Name.ToUpper().IndexOf("ID");
                columnDefinition.Name = columnDefinition.Name.Substring(0, start);
            }

            var column = new ColumnPropDto();

            column.Name = columnDefinition.Name;
            column.Code = columnDefinition.Code;
            column.Display = columnDefinition.Name;
            column.IsNull = true;
            if (int.TryParse(columnDefinition.Length, out int len))
            {
                column.Length = len;
            }



            if ("int".Equals(columnDefinition.Type, StringComparison.OrdinalIgnoreCase))
            {
                column.ColumnPropType = ColumnPropType.整型;
            }
            else if ("varchar".Equals(columnDefinition.Type, StringComparison.OrdinalIgnoreCase))
            {
                column.ColumnPropType = ColumnPropType.字符串;
            }
            else if ("datetime".Equals(columnDefinition.Type, StringComparison.OrdinalIgnoreCase))
            {
                column.ColumnPropType = ColumnPropType.时间;
            }
            else if ("date".Equals(columnDefinition.Type, StringComparison.OrdinalIgnoreCase))
            {
                column.ColumnPropType = ColumnPropType.时间;
            }
            else if ("timestamp".Equals(columnDefinition.Type, StringComparison.OrdinalIgnoreCase))
            {
                column.ColumnPropType = ColumnPropType.时间;
            }
            else if ("time".Equals(columnDefinition.Type, StringComparison.OrdinalIgnoreCase))
            {
                column.ColumnPropType = ColumnPropType.时间;
            }
            else if ("bit".Equals(columnDefinition.Type, StringComparison.OrdinalIgnoreCase))
            {
                column.ColumnPropType = ColumnPropType.布尔型;
            }
            else if ("tinyint".Equals(columnDefinition.Type, StringComparison.OrdinalIgnoreCase))
            {
                column.ColumnPropType = ColumnPropType.布尔型;
            }
            else if ("longtext".Equals(columnDefinition.Type, StringComparison.OrdinalIgnoreCase))
            {
                column.ColumnPropType = ColumnPropType.长文本;
            }
            else if ("text".Equals(columnDefinition.Type, StringComparison.OrdinalIgnoreCase))
            {
                column.ColumnPropType = ColumnPropType.多行文本;
            }

            else if ("double".Equals(columnDefinition.Type, StringComparison.OrdinalIgnoreCase))
            {
                column.ColumnPropType = ColumnPropType.双浮点型;
            }
            else if ("dec".Equals(columnDefinition.Type, StringComparison.OrdinalIgnoreCase))
            {
                column.ColumnPropType = ColumnPropType.双浮点型;
            }
            else
            {

                column.ColumnPropType = ColumnPropType.字符串;
            }
            column.ColumnPropTypeValue = (int)column.ColumnPropType + "";
            return column;
        }

        #endregion



        #region AI转换驼峰命名


        /// <summary>
        /// 获取表实体列表
        /// </summary>
        public async Task<List<AIConvertTableEntityNameDto>> GetAIConvertTableEntityListAsync(IdInput<Guid> input)
        {
            var tableEntityDtoList = await _tableEntityManager.GetListAsync(input.Id);

            var result = tableEntityDtoList.Select(x => new AIConvertTableEntityNameDto()
            {
                TableEntityId = x.Id.Value,
                Name = x.Name,
                Code = x.Code,
            }).ToList();

            return result;
        }

        /// <summary>
        /// AI转换驼峰命名
        /// </summary>
        public async Task<List<AIConvertTableEntityNameDto>> AIConvertTableEntityName(List<AIConvertTableEntityNameDto> input)
        {
            var systemPrompt = _deepSeekService.GetSystemPrompt(DeepSeekSystemPrompt.TableStructureNameCamelCase);


            //把input按10进行分页
            var pageList = input.Chunk(10).ToList();

            //开启异步Parallel.ForEachAsync
            await Parallel.ForEachAsync(pageList, new ParallelOptions() { MaxDegreeOfParallelism = 10 }, async (page, token) =>
            {

                var requestData = input.Select(x => new
                {
                    TableName = x.Name,
                    TableCode = x.Code,
                }).ToList();

                var requestDataJson = requestData.ToJson();

                var result = await _deepSeekService.ChatCompletionAsync<TableStructureNameCamelCaseResultDtoList>(systemPrompt, requestDataJson);


                foreach (var item in input)
                {
                    var resultItem = result.Tables.Where(x => x.TableName == item.Name).FirstOrDefault();
                    if (resultItem != null)
                    {
                        item.NewCode = resultItem.NewTableName;
                    }
                }
            });
            return input;

        }

        /**
        保存到数据库
        */
        public async Task AIConvertTableEntityNameSave(List<AIConvertTableEntityNameDto> input)
        {

            await _tableEntityManager.AIConvertTableEntityNameSave(input);
        }

        #endregion


        #region AI自动修复长度

        /// <summary>
        /// 获取AI自动修复长度列表
        /// </summary>
        public async Task<List<AiFixColumnLengthTableDto>> GetAiFixColumnLengthListAsync(IdInput<Guid> input)
        {

            var tableEntityDtoList = await _tableEntityManager.GetListAsync(input.Id);
            //得到tableEntityDtoList的tableId
            var tableEntityIds = tableEntityDtoList.Select(x => x.Id.Value).ToList();

            //得到下面所有的列
            var columnPropList = await _columnPropManager.GetColumnProps(new ColumnPropPagedInput()
            {
                TableEntityIds = tableEntityIds,
            });

            var result = new List<AiFixColumnLengthTableDto>();

            foreach (var item in tableEntityDtoList)
            {
                item.Columns = columnPropList.Where(x => x.TableEntityId == item.Id.Value).ToList();

                var tableDto = new AiFixColumnLengthTableDto();
                tableDto.TableName = item.Name;
                tableDto.TableId = item.Id.Value;
                tableDto.Columns = new List<AiFixColumnLengthDto>();

                foreach (var column in item.Columns)
                {
                    if (column.ColumnPropType == ColumnPropType.字符串)
                    {
                        tableDto.Columns.Add(new AiFixColumnLengthDto()
                        {
                            ColumnPropId = column.Id.Value,
                            Name = column.Name,
                            Code = column.Code,
                            OldLength = column.Length
                        });
                    }
                }
                if (tableDto.Columns.Count > 0)
                {
                    result.Add(tableDto);
                }
            }

            return result;
        }

        /// <summary>
        /// AI自动修复长度
        /// </summary>
        public async Task<List<AiFixColumnLengthTableDto>> AIFixColumnLengthAsync(List<AiFixColumnLengthTableDto> input)
        {
            var systemPrompt = _deepSeekService.GetSystemPrompt(DeepSeekSystemPrompt.TableStructureLengthAdjustment);


            //把input按3进行分页
            var pageList = input.Chunk(3).ToList();

            //开启异步Parallel.ForEachAsync
            await Parallel.ForEachAsync(pageList, new ParallelOptions() { MaxDegreeOfParallelism = 10 }, async (page, token) =>
            {
                var requestData = page.Select(x => new
                {
                    TableName = x.TableName,
                    Columns = x.Columns.Select(y => new
                    {

                        ColumnName = y.Name,
                        ColumnCode = y.Code,
                        OldLength = y.OldLength,
                    }).ToList()
                }).ToList();

                var requestDataJson = requestData.ToJson();

                var result = await _deepSeekService.ChatCompletionAsync<TableStructureLengthAdjustmentResultDtoList>(systemPrompt, requestDataJson, 2000);

                foreach (var item in input)
                {
                    foreach (var column in item.Columns)
                    {
                        var resultItem = result.Columns.Where(x => x.TableName == item.TableName && x.ColumnCode == column.Code).FirstOrDefault();
                        if (resultItem != null)
                        {
                            column.NewLength = int.Parse(resultItem.NewLength);
                        }
                    }
                }
            });

            return input;
        }

        /**
            保存到数据库
            */
        public async Task AIFixColumnLengthSaveAsync(List<AiFixColumnLengthTableDto> input)
        {

            await _columnPropManager.AIFixColumnLengthSaveAsync(input);
        }
        #endregion

        #region  AI自动分析外键关系

        /// <summary>
        /// 获取AI自动分析外键关系列表
        /// </summary>
        public async Task<List<AiFixRelationShipTableDto>> GetAiFixRelationShipListAsync(IdInput<Guid> input)
        {
            var tableEntityDtoList = await _tableEntityManager.GetListAsync(input.Id);
            //得到tableEntityDtoList的tableId
            var tableEntityIds = tableEntityDtoList.Select(x => x.Id.Value).ToList();

            //得到下面所有的列
            var columnPropList = await _columnPropManager.GetColumnProps(new ColumnPropPagedInput()
            {
                TableEntityIds = tableEntityIds,
            });

            var result = new List<AiFixRelationShipTableDto>();

            foreach (var item in tableEntityDtoList)
            {
                item.Columns = columnPropList.Where(x => x.TableEntityId == item.Id.Value).ToList();

                var tableDto = new AiFixRelationShipTableDto();
                tableDto.TableName = item.Name;
                tableDto.TableCode = item.Code;
                tableDto.TableId = item.Id.Value;
                tableDto.Columns = new List<AiFixRelationShipColumnDto>();
                tableDto.Relations = new List<AiFixRelationShipDto>();

                foreach (var column in item.Columns)
                {
                    if (column.ColumnPropType != ColumnPropType.字符串 && column.ColumnPropType != ColumnPropType.整型)
                    {
                        continue;
                    }
                    if (!column.Code.EndsWith("Id", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    //如果不是Id结尾

                    tableDto.Columns.Add(new AiFixRelationShipColumnDto()
                    {
                        ColumnPropId = column.Id.Value,
                        Name = column.Name,
                        Code = column.Code,
                        ColumnType = column.ColumnPropType.ToString()
                    });

                }

                result.Add(tableDto);

            }

            return result;

        }

        /// <summary>
        /// AI自动分析外键关系
        /// </summary>
        public async Task<List<AiFixRelationShipTableDto>> AIFixRelationShipAsync(List<AiFixRelationShipTableDto> input)
        {
            var systemPrompt = _deepSeekService.GetSystemPrompt(DeepSeekSystemPrompt.TableStructureRelationShip);

            var requestData = input.Select(x => new
            {
                TableName = x.TableName,
                TableCode = x.TableCode,
                Columns = x.Columns.Select(y => new
                {
                    ColumnName = y.Name,
                    ColumnCode = y.Code,
                    ColumnType = y.ColumnType
                }).ToList()

            }).ToList();

            var requestDataJson = requestData.ToJson();

            var result = await _deepSeekService.ChatCompletionAsync<TableStructureRelationShipResultDtoList>(systemPrompt, requestDataJson, 4000);


            foreach (var item in input)
            {
                item.Relations = new List<AiFixRelationShipDto>();
                var resultItems = result.Tables.Where(x => x.TableCode == item.TableCode).ToList();
                if (resultItems != null)
                {

                    foreach (var resultItem in resultItems)
                    {
                        var column = item.Columns.Where(x => x.Code == resultItem.RefColumnCode).FirstOrDefault();
                        if (column == null)
                        {
                            continue;
                        }
                        var refTable = input.Where(x => x.TableCode == resultItem.RefTableCode).FirstOrDefault();
                        if (refTable == null)
                        {
                            continue;
                        }


                        item.Relations.Add(new AiFixRelationShipDto()
                        {

                            TableCode = resultItem.TableCode,
                            TableName = item.TableName,
                            TableId = item.TableId,
                            RefTableCode = resultItem.RefTableCode,
                            RefTableName = resultItem.RefTableName,
                            RefColumnCode = resultItem.RefColumnCode,
                            RefColumnName = resultItem.RefColumnName,
                            RefColumnId = column.ColumnPropId,
                            RefTableId = refTable.TableId
                        });
                    }
                }
            }
            return input;
        }

        /**
        手动分析
        */
        public async Task<List<AiFixRelationShipTableDto>> ManualFixRelationShipAsync(List<AiFixRelationShipTableDto> input)
        {
            var systemPrompt = _deepSeekService.GetSystemPrompt(DeepSeekSystemPrompt.TableStructureRelationShip);


            var result = new TableStructureRelationShipResultDtoList()
            {
                Tables = new List<TableStructureRelationShipResultDto>()
            };

            foreach (var item in input)
            {
                foreach (var column in item.Columns)
                {
                    //判断该列去掉Id之后是否匹配其他表编码
                    var columnName = column.Code.Replace("Id", "");
                    var refTable = input.FirstOrDefault(x => x.TableCode == columnName);
                    if (refTable != null)
                    {
                        result.Tables.Add(new TableStructureRelationShipResultDto()
                        {
                            TableCode = item.TableCode,
                            TableName = item.TableName,
                            RefTableCode = refTable.TableCode,
                            RefTableName = refTable.TableName,
                            RefColumnCode = column.Code,
                            RefColumnName = column.Name,

                        });
                    }
                }
            }


            foreach (var item in input)
            {
                item.Relations = new List<AiFixRelationShipDto>();
                var resultItems = result.Tables.Where(x => x.TableCode == item.TableCode).ToList();
                if (resultItems != null)
                {

                    foreach (var resultItem in resultItems)
                    {
                        var column = item.Columns.Where(x => x.Code == resultItem.RefColumnCode).FirstOrDefault();
                        if (column == null)
                        {
                            continue;
                        }
                        var refTable = input.Where(x => x.TableCode == resultItem.RefTableCode).FirstOrDefault();
                        if (refTable == null)
                        {
                            continue;
                        }

                        item.Relations.Add(new AiFixRelationShipDto()
                        {

                            TableCode = resultItem.TableCode,
                            TableName = item.TableName,
                            TableId = item.TableId,
                            RefTableCode = resultItem.RefTableCode,
                            RefTableName = resultItem.RefTableName,
                            RefColumnCode = resultItem.RefColumnCode,
                            RefColumnName = resultItem.RefColumnName,
                            RefColumnId = column.ColumnPropId,
                            RefTableId = refTable.TableId
                        });
                    }
                }
            }
            return input;
        }



        /**
            保存到数据库
            */
        public async Task AIFixRelationShipSaveAsync(AiFixRelationShipTableSubmitDto input)
        {
            await _tableNavigateRelativeManager.AIFixRelationShipSaveAsync(input);
        }

        #endregion


        #region  AI自动解析枚举

        /// <summary>
        /// AI自动解析枚举
        /// </summary>
        public async Task<List<AiFixEnumResultDto>> AiFixEnumBySqlScript(AiFixEnumBySqlScriptSubmitDto input)
        {
            var sqlScript = input.SqlScript;
            var systemPrompt = _deepSeekService.GetSystemPrompt(DeepSeekSystemPrompt.EnumParse);
            var result = await _deepSeekService.ChatCompletionAsync<TableStructureEnumResultDtoList>(systemPrompt, sqlScript, 2000);

            return result.Enums.Select(x => new AiFixEnumResultDto()
            {
                PlanId = input.PlanId,
                Name = x.Name,
                Code = x.Code,
                EnumPropsList = x.EnumPropsList.Select(y => new AiFixEnumDtoList()
                {
                    Name = y.Name,
                    Value = y.Value
                }).ToList()
            }).ToList();
        }

        /// <summary>
        /// 保存到数据库
        /// </summary>
        public async Task AiFixEnumSaveAsync(AiFixEnumSaveDto input)
        {

            await _planEnumManager.AiFixEnumSaveAsync(input);

        }
        #endregion


        #region AI自动列排序

        /// <summary>
        /// 获取AI自动列排序列表
        /// </summary>
        public async Task<List<AiSortColumnTableDto>> GetAiSortColumnListAsync(IdInput<Guid> input)
        {
            var tableEntityDtoList = await _tableEntityManager.GetListAsync(input.Id);
            //得到tableEntityDtoList的tableId
            var tableEntityIds = tableEntityDtoList.Select(x => x.Id.Value).ToList();

            //得到下面所有的列
            var columnPropList = await _columnPropManager.GetColumnProps(new ColumnPropPagedInput()
            {
                TableEntityIds = tableEntityIds,
            });

            var result = new List<AiSortColumnTableDto>();

            foreach (var item in tableEntityDtoList)
            {
                // 如果有Sort字段则按Sort排序，否则按创建时间排序
                item.Columns = columnPropList
                    .Where(x => x.TableEntityId == item.Id.Value)
                    .OrderBy(x => x.CreationTime)
                    .ToList();

                var tableDto = new AiSortColumnTableDto();
                tableDto.TableName = item.Name;
                tableDto.TableId = item.Id.Value;
                tableDto.Columns = new List<AiSortColumnDto>();

                int sortIndex = 1;
                foreach (var column in item.Columns)
                {
                    tableDto.Columns.Add(new AiSortColumnDto()
                    {
                        ColumnPropId = column.Id.Value,
                        Name = column.Name,
                        Code = column.Code,
                        OldSort = sortIndex,
                        NewSort = null,
                        Reason = ""
                    });
                    sortIndex++;
                }

                if (tableDto.Columns.Count > 0)
                {
                    result.Add(tableDto);
                }
            }

            return result;
        }

        /// <summary>
        /// AI自动列排序
        /// </summary>
        public async Task<List<AiSortColumnTableDto>> AiSortColumnAsync(List<AiSortColumnTableDto> input)
        {
            var systemPrompt = _deepSeekService.GetSystemPrompt(DeepSeekSystemPrompt.TableColumnSort);

            //把input按3进行分页
            var pageList = input.Chunk(3).ToList();

            //开启异步Parallel.ForEachAsync
            await Parallel.ForEachAsync(pageList, new ParallelOptions() { MaxDegreeOfParallelism = 10 }, async (page, token) =>
            {
                var requestData = page.Select(x => new
                {
                    TableName = x.TableName,
                    Columns = x.Columns.Select((y, index) => new
                    {
                        ColumnName = y.Name,
                        ColumnCode = y.Code,
                        CurrentSort = y.OldSort,
                    }).ToList()
                }).ToList();

                var requestDataJson = requestData.ToJson();

                var result = await _deepSeekService.ChatCompletionAsync<TableColumnSortResultDtoList>(systemPrompt, requestDataJson, 2000);

                foreach (var item in page)
                {
                    var resultItem = result.Tables.Where(x => x.TableName == item.TableName).FirstOrDefault();
                    if (resultItem != null)
                    {
                        foreach (var column in item.Columns)
                        {
                            var columnResult = resultItem.Columns.Where(x => x.ColumnCode == column.Code).FirstOrDefault();
                            if (columnResult != null)
                            {
                                column.NewSort = columnResult.NewSort;
                                column.Reason = columnResult.Reason;
                            }
                        }
                    }
                }
            });

            return input;
        }

        /// <summary>
        /// 保存到数据库
        /// </summary>
        public async Task AiSortColumnSaveAsync(List<AiSortColumnTableDto> input)
        {
            await _columnPropManager.AiSortColumnSaveAsync(input);
        }

        #endregion

        #region AI自动绑定枚举

        /// <summary>
        /// 获取AI自动绑定枚举数据
        /// </summary>
        public async Task<AiBindEnumResultDto> GetAiBindEnumListAsync(IdInput<Guid> input)
        {
            // 获取所有表
            var tableEntityDtoList = await _tableEntityManager.GetListAsync(input.Id);
            var tableEntityIds = tableEntityDtoList.Select(x => x.Id.Value).ToList();

            // 获取所有列
            var columnPropList = await _columnPropManager.GetColumnProps(new ColumnPropPagedInput()
            {
                TableEntityIds = tableEntityIds,
            });

            // 获取所有枚举
            var enumList = await _planEnumManager.ListAsync(new PlanEnumPagedInput()
            {
                PlanId = input.Id,
                Page = 1,
                Size = 1000
            });

            var result = new AiBindEnumResultDto();
            result.Tables = new List<AiBindEnumTableDto>();
            result.Enums = new List<AiBindEnumInfoDto>();

            // 构建表和列数据
            foreach (var table in tableEntityDtoList)
            {
                var tableDto = new AiBindEnumTableDto();
                tableDto.TableName = table.Name;
                tableDto.TableCode = table.Code;
                tableDto.TableId = table.Id.Value;
                tableDto.Columns = new List<AiBindEnumColumnDto>();

                // 获取该表的列
                var tableColumns = columnPropList.Where(x => x.TableEntityId == table.Id.Value).ToList();
                foreach (var column in tableColumns)
                {
                    // 只包含整型的列（可能是枚举）
                    // 排除外键：不以 Id 结尾的列
                    if (column.ColumnPropType == ColumnPropType.整型 &&
                        !column.Code.EndsWith("Id", StringComparison.OrdinalIgnoreCase))
                    {
                        var columnDto = new AiBindEnumColumnDto()
                        {
                            ColumnPropId = column.Id.Value,
                            Name = column.Name,
                            Code = column.Code,
                            ColumnType = column.ColumnPropType.ToString(),
                            MatchedEnumId = column.ColumnPropType == ColumnPropType.枚举型 ? column.PlanEnumId : null,
                            MatchedEnumCode = null,
                            MatchedEnumName = null
                        };

                        // 如果已经是枚举类型，获取枚举信息
                        if (column.ColumnPropType == ColumnPropType.枚举型 && column.PlanEnumId.HasValue)
                        {
                            var enumInfo = enumList.Items.FirstOrDefault(x => x.Id == column.PlanEnumId.Value);
                            if (enumInfo != null)
                            {
                                columnDto.MatchedEnumCode = enumInfo.Code;
                                columnDto.MatchedEnumName = enumInfo.Name;
                            }
                        }

                        tableDto.Columns.Add(columnDto);
                    }
                }

                if (tableDto.Columns.Count > 0)
                {
                    result.Tables.Add(tableDto);
                }
            }

            // 构建枚举数据
            foreach (var enumItem in enumList.Items)
            {
                result.Enums.Add(new AiBindEnumInfoDto()
                {
                    EnumId = enumItem.Id.Value,
                    Name = enumItem.Name,
                    Code = enumItem.Code
                });
            }

            return result;
        }

        /// <summary>
        /// AI自动绑定枚举
        /// </summary>
        public async Task<AiBindEnumResultDto> AiBindEnumAsync(AiBindEnumResultDto input)
        {
            var systemPrompt = _deepSeekService.GetSystemPrompt(DeepSeekSystemPrompt.EnumBinding);

            // 构建请求数据
            var requestData = new
            {
                Tables = input.Tables.Select(t => new
                {
                    TableName = t.TableName,
                    TableCode = t.TableCode,
                    Columns = t.Columns.Select(c => new
                    {
                        ColumnName = c.Name,
                        ColumnCode = c.Code,
                        ColumnType = c.ColumnType
                    }).ToList()
                }).ToList(),
                Enums = input.Enums.Select(e => new
                {
                    EnumName = e.Name,
                    EnumCode = e.Code
                }).ToList()
            };

            var requestDataJson = requestData.ToJson();

            // 调用AI
            var result = await _deepSeekService.ChatCompletionAsync<EnumBindingResultDtoList>(systemPrompt, requestDataJson, 3000);

            // 处理结果
            foreach (var binding in result.Bindings)
            {
                var table = input.Tables.FirstOrDefault(t => t.TableCode == binding.TableCode);
                if (table != null)
                {
                    var column = table.Columns.FirstOrDefault(c => c.Code == binding.ColumnCode);
                    if (column != null)
                    {
                        var enumInfo = input.Enums.FirstOrDefault(e => e.Code == binding.MatchedEnumCode);
                        if (enumInfo != null)
                        {
                            column.MatchedEnumId = enumInfo.EnumId;
                            column.MatchedEnumCode = enumInfo.Code;
                            column.MatchedEnumName = enumInfo.Name;
                        }
                    }
                }
            }

            return input;
        }

        /// <summary>
        /// 保存枚举绑定
        /// </summary>
        public async Task AiBindEnumSaveAsync(AiBindEnumSaveDto input)
        {
            var columnsList = input.Tables.SelectMany(x => x.Columns).Where(x => x.MatchedEnumId.HasValue).ToList();
            var columnPropIds = columnsList.Select(x => x.ColumnPropId).ToList();

            var columnProps = await _sqlSugarClient.Queryable<ColumnProp>()
                .Where(x => columnPropIds.Contains(x.Id))
                .ToListAsync();

            foreach (var columnProp in columnProps)
            {
                var matchedColumn = columnsList.FirstOrDefault(x => x.ColumnPropId == columnProp.Id);
                if (matchedColumn != null && matchedColumn.MatchedEnumId.HasValue)
                {
                    columnProp.ColumnPropType = ColumnPropType.枚举型;
                    columnProp.PlanEnumId = matchedColumn.MatchedEnumId.Value;
                }
            }

            await _sqlSugarClient.Updateable(columnProps).ExecuteCommandAsync();
        }

        #endregion

        #region AI自动绑定列类型

        /// <summary>
        /// 获取AI自动绑定列类型数据
        /// </summary>
        public async Task<List<AiBindColumnPropTableDto>> GetAiBindColumnPropListAsync(IdInput<Guid> input)
        {
            // 获取所有表
            var tableEntityDtoList = await _tableEntityManager.GetListAsync(input.Id);
            var tableEntityIds = tableEntityDtoList.Select(x => x.Id.Value).ToList();

            // 获取所有列
            var columnPropList = await _columnPropManager.GetColumnProps(new ColumnPropPagedInput()
            {
                TableEntityIds = tableEntityIds,
            });

            var result = new List<AiBindColumnPropTableDto>();

            // 构建表和列数据
            foreach (var table in tableEntityDtoList)
            {
                var tableDto = new AiBindColumnPropTableDto();
                tableDto.TableName = table.Name;
                tableDto.TableCode = table.Code;
                tableDto.TableId = table.Id.Value;
                tableDto.Columns = new List<AiBindColumnPropColumnDto>();

                // 获取该表的列 - 只包含字符串类型的列
                var tableColumns = columnPropList
                    .Where(x => x.TableEntityId == table.Id.Value &&
                               x.ColumnPropType == ColumnPropType.字符串)
                    .ToList();

                foreach (var column in tableColumns)
                {
                    var columnDto = new AiBindColumnPropColumnDto()
                    {
                        ColumnPropId = column.Id.Value,
                        Name = column.Name,
                        Code = column.Code,
                        CurrentType = column.ColumnPropType.ToDescription(),
                        SuggestedType = null,
                        SuggestedTypeValue = null
                    };

                    tableDto.Columns.Add(columnDto);
                }

                // 只添加有字符串列的表
                if (tableDto.Columns.Count > 0)
                {
                    result.Add(tableDto);
                }
            }

            return result;
        }

        /// <summary>
        /// AI自动绑定列类型
        /// </summary>
        public async Task<List<AiBindColumnPropTableDto>> AiBindColumnPropAsync(List<AiBindColumnPropTableDto> input)
        {
            var systemPrompt = _deepSeekService.GetSystemPrompt(DeepSeekSystemPrompt.ColumnPropTypeBinding);

            // 构建请求数据
            var requestData = new
            {
                Tables = input.Select(t => new
                {
                    TableName = t.TableName,
                    TableCode = t.TableCode,
                    Columns = t.Columns.Select(c => new
                    {
                        ColumnName = c.Name,
                        ColumnCode = c.Code,
                        CurrentType = c.CurrentType
                    }).ToList()
                }).ToList()
            };

            var requestDataJson = requestData.ToJson();

            // 调用AI
            var result = await _deepSeekService.ChatCompletionAsync<ColumnPropTypeBindingResultDtoList>(systemPrompt, requestDataJson, 3000);

            // 类型映射字典
            var typeMapping = new Dictionary<string, ColumnPropType>()
            {
                { "Image", ColumnPropType.图片 },
                { "Video", ColumnPropType.视频 },
                { "Audio", ColumnPropType.音频 },
                { "File", ColumnPropType.文件 },
                { "MultiText", ColumnPropType.多行文本 },
                { "Text", ColumnPropType.长文本 }
            };

            // 处理结果
            foreach (var binding in result.Bindings)
            {
                var table = input.FirstOrDefault(t => t.TableCode == binding.TableCode);
                if (table != null)
                {
                    var column = table.Columns.FirstOrDefault(c => c.Code == binding.ColumnCode);
                    if (column != null && typeMapping.ContainsKey(binding.SuggestedType))
                    {
                        var suggestedType = typeMapping[binding.SuggestedType];
                        column.SuggestedType = suggestedType.ToDescription();
                        column.SuggestedTypeValue = (int)suggestedType;
                    }
                }
            }

            return input;
        }

        /// <summary>
        /// 保存列类型绑定
        /// </summary>
        public async Task AiBindColumnPropSaveAsync(AiBindColumnPropSaveDto input)
        {
            var columnsList = input.Tables.SelectMany(x => x.Columns)
                .Where(x => x.SuggestedTypeValue.HasValue)
                .ToList();

            var columnPropIds = columnsList.Select(x => x.ColumnPropId).ToList();

            var columnProps = await _sqlSugarClient.Queryable<ColumnProp>()
                .Where(x => columnPropIds.Contains(x.Id))
                .ToListAsync();

            foreach (var columnProp in columnProps)
            {
                var matchedColumn = columnsList.FirstOrDefault(x => x.ColumnPropId == columnProp.Id);
                if (matchedColumn != null && matchedColumn.SuggestedTypeValue.HasValue)
                {
                    columnProp.ColumnPropType = (ColumnPropType)matchedColumn.SuggestedTypeValue.Value;
                }
            }

            await _sqlSugarClient.Updateable(columnProps).ExecuteCommandAsync();
        }

        #endregion

        #region AI自动识别主要列

        /// <summary>
        /// 获取AI自动识别主要列数据
        /// </summary>
        public async Task<List<AiPrimaryColumnTableDto>> GetAiPrimaryColumnListAsync(IdInput<Guid> input)
        {
            // 获取所有表
            var tableEntityDtoList = await _tableEntityManager.GetListAsync(input.Id);
            var tableEntityIds = tableEntityDtoList.Select(x => x.Id.Value).ToList();

            // 获取所有列
            var columnPropList = await _columnPropManager.GetColumnProps(new ColumnPropPagedInput()
            {
                TableEntityIds = tableEntityIds,
            });

            var result = new List<AiPrimaryColumnTableDto>();

            // 构建表和列数据
            foreach (var table in tableEntityDtoList)
            {
                var tableDto = new AiPrimaryColumnTableDto();
                tableDto.TableName = table.Name;
                tableDto.TableCode = table.Code;
                tableDto.TableId = table.Id.Value;
                tableDto.Columns = new List<AiPrimaryColumnDto>();

                // 获取该表的列 - 只包含字符串类型的列
                var tableColumns = columnPropList
                    .Where(x => x.TableEntityId == table.Id.Value &&
                               x.ColumnPropType == ColumnPropType.字符串)
                    .ToList();

                foreach (var column in tableColumns)
                {
                    var columnDto = new AiPrimaryColumnDto()
                    {
                        ColumnPropId = column.Id.Value,
                        Name = column.Name,
                        Code = column.Code,
                        IsPrimary = column.IspPimaryDisplayColumn ?? false
                    };

                    tableDto.Columns.Add(columnDto);
                }

                // 只添加有字符串列的表
                if (tableDto.Columns.Count > 0)
                {
                    result.Add(tableDto);
                }
            }

            return result;
        }

        /// <summary>
        /// AI自动识别主要列
        /// </summary>
        public async Task<List<AiPrimaryColumnTableDto>> AiPrimaryColumnAsync(List<AiPrimaryColumnTableDto> input)
        {
            var systemPrompt = _deepSeekService.GetSystemPrompt(DeepSeekSystemPrompt.PrimaryDisplayColumn);

            // 构建请求数据
            var requestData = input.Select(t => new PrimaryDisplayColumnRequestDto
            {
                TableCode = t.TableCode,
                TableName = t.TableName,
                Columns = string.Join(",", t.Columns.Select(c => c.Code))
            }).ToList();

            var requestDataJson = requestData.ToJson();

            // 调用AI
            var result = await _deepSeekService.ChatCompletionAsync<PrimaryDisplayColumnResultDtoList>(systemPrompt, requestDataJson, 3000);

            // 处理结果 - 每个表只有一个主要列
            foreach (var tableResult in result.Tables)
            {
                var table = input.FirstOrDefault(t => t.TableCode == tableResult.TableCode);
                if (table != null)
                {
                    // 先将所有列设为非主要列
                    foreach (var column in table.Columns)
                    {
                        column.IsPrimary = false;
                    }

                    // 设置AI识别的主要列
                    var primaryColumn = table.Columns.FirstOrDefault(c => c.Code == tableResult.PrimaryColumnCode);
                    if (primaryColumn != null)
                    {
                        primaryColumn.IsPrimary = true;
                    }
                }
            }

            return input;
        }

        /// <summary>
        /// 保存主要列识别结果
        /// </summary>
        public async Task AiPrimaryColumnSaveAsync(AiPrimaryColumnSaveDto input)
        {
            // 获取所有需要更新的列ID
            var allColumnIds = input.Tables.SelectMany(x => x.Columns).Select(x => x.ColumnPropId).ToList();

            // 查询这些列
            var columnProps = await _sqlSugarClient.Queryable<ColumnProp>()
                .Where(x => allColumnIds.Contains(x.Id))
                .ToListAsync();

            // 更新主要列标识
            foreach (var columnProp in columnProps)
            {
                var matchedColumn = input.Tables
                    .SelectMany(t => t.Columns)
                    .FirstOrDefault(c => c.ColumnPropId == columnProp.Id);

                if (matchedColumn != null)
                {
                    columnProp.IspPimaryDisplayColumn = matchedColumn.IsPrimary;
                }
            }

            await _sqlSugarClient.Updateable(columnProps).ExecuteCommandAsync();
        }

        #endregion

    }
}
