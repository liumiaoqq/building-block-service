using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Tables;

namespace Web.Manager
{
    public class TableNavigateRelativeManager
    {
        protected ISqlSugarClient _sqlSugarClient;

        public TableNavigateRelativeManager(ISqlSugarClient sqlSugarClient)
        {
            _sqlSugarClient = sqlSugarClient;
        }



        public async Task<PagedReuslt<TableNavigateRelativeDto>> ListAsync(TableNavigateRelativePagedInput input)
        {
            RefAsync<int> totalCount = 0;
            var items = await _sqlSugarClient.Queryable<TableNavigateRelative>()
                .WhereIF(input.UserId.HasValue, x => x.CreatorId == input.UserId)
                .WhereIF(input.RelativeTableId.HasValue, x => x.RelativeTableId == input.RelativeTableId.Value)
                .Select<TableNavigateRelativeDto>()
                .ToPageListAsync(input.Page, input.Size, totalCount);

            var tables = await _sqlSugarClient.Queryable<TableEntity>().Where(x => x.PlanId == input.PlanId).Select<TableEntityDto>().ToListAsync();
            var tableIds = tables.Select(x => x.Id.Value).ToList();
            var columns = await _sqlSugarClient.Queryable<ColumnProp>().Where(x => tableIds.Contains(x.TableEntityId)).Select<ColumnPropDto>().ToListAsync();
            foreach (var item in items)
            {
                item.AssociationATableEntity = tables.FirstOrDefault(x => x.Id == item.AssociationATableId);
                item.AssociationAColumnPropDto = columns.FirstOrDefault(x => x.Id == item.AssociationAColumnId);
                item.AssociationBTableEntity = tables.FirstOrDefault(x => x.Id == item.AssociationBTableId);
                item.AssociationBColumnPropDto = columns.FirstOrDefault(x => x.Id == item.AssociationBColumnId);
            }
            return new PagedReuslt<TableNavigateRelativeDto>(items, totalCount.Value);

        }
        public async Task<TableNavigateRelativeDto> GetAsync(IdInput<Guid> input)
        {
            var dto = await _sqlSugarClient.Queryable<TableNavigateRelative>()
                .Where(x => x.Id == input.Id)
                .Select<TableNavigateRelativeDto>().FirstAsync();
            return dto ?? new TableNavigateRelativeDto();

        }
        public async Task DeleteAsync(IdInput<Guid> input)
        {
            await _sqlSugarClient.Updateable<TableNavigateRelative>().Where(it => it.Id == input.Id).SetColumns(x => x.IsDeleted == true).ExecuteCommandAsync();
        }



        /// <summary>
        /// 查询对应条件的关联规则
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<List<TableNavigateRelativeDto>> GetTableNavigateRelativeDtos(TableNavigateRelativePagedInput input)
        {
            var tableNavigateRelatives = await _sqlSugarClient.Queryable<TableNavigateRelative>()
                 .WhereIF(input.RelativeTableId.HasValue, x => x.RelativeTableId == input.RelativeTableId.Value)
                 .Select<TableNavigateRelativeDto>().ToListAsync();
            //得到需要查询的所有表
            var tableIds = tableNavigateRelatives.Select(x => x.AssociationATableId).Union(tableNavigateRelatives.Select(x => x.AssociationBTableId)).Where(x => x.HasValue).Distinct().ToList();
            var allTableEntityDtos = await _sqlSugarClient.Queryable<TableEntity>().Where(x => tableIds.Contains(x.Id)).Select<TableEntityDto>().ToListAsync();
            //得到这些表的所有列
            var allColumnPropDtos = await _sqlSugarClient.Queryable<ColumnProp>().Where(x => tableIds.Contains(x.TableEntityId)).Select<ColumnPropDto>().ToListAsync();
            //循环这些表
            foreach (var tableEntity in allTableEntityDtos)
            {
                tableEntity.Columns = allColumnPropDtos.Where(x => x.TableEntityId == tableEntity.Id).ToList();
            }
            //循环所有的列让他对应到表
            foreach (var columnPropDto in allColumnPropDtos)
            {
                columnPropDto.TableEntityDto = allTableEntityDtos.FirstOrDefault(x => x.Id == columnPropDto.TableEntityId);
            }

            //循环表和表之间的关系
            foreach (var tableNavigateRelative in tableNavigateRelatives)
            {

                if (tableNavigateRelative.AssociationAColumnId.HasValue)
                {
                    tableNavigateRelative.AssociationAColumnPropDto = allColumnPropDtos.FirstOrDefault(x => x.Id == tableNavigateRelative.AssociationAColumnId);
                }
                if (tableNavigateRelative.AssociationATableId.HasValue)
                {
                    tableNavigateRelative.AssociationATableEntity = allTableEntityDtos.FirstOrDefault(x => x.Id == tableNavigateRelative.AssociationATableId);
                }
                if (tableNavigateRelative.AssociationBColumnId.HasValue)
                {
                    tableNavigateRelative.AssociationBColumnPropDto = allColumnPropDtos.FirstOrDefault(x => x.Id == tableNavigateRelative.AssociationBColumnId);
                }
                if (tableNavigateRelative.AssociationBTableId.HasValue)
                {
                    tableNavigateRelative.AssociationBTableEntity = allTableEntityDtos.FirstOrDefault(x => x.Id == tableNavigateRelative.AssociationBTableId);
                }
            }
            return tableNavigateRelatives;

        }
        /**
            AI自动分析外键关系
            */
        public async Task AIFixRelationShipSaveAsync(AiFixRelationShipTableSubmitDto input)
        {
            //查询计划
            var plan = await _sqlSugarClient.Queryable<Plan>().Where(x => x.Id == input.PlanId).FirstAsync();
            //得到计划下的所有表
            var tableEntities = await _sqlSugarClient.Queryable<TableEntity>().Where(x => x.PlanId == input.PlanId).ToListAsync();
            //删除所有表的关联关系
            var tableIds = tableEntities.Select(x => x.Id).ToList();
            await _sqlSugarClient.Deleteable<TableNavigateRelative>()
                      .Where(x => tableIds.Contains(x.RelativeTableId))
                      .ExecuteCommandAsync();


            var tableNavigateRelatives = new List<TableNavigateRelative>();

            //插入新的关联关系
            foreach (var item in input.RelationShipTables)
            {
                foreach (var relation in item.Relations)
                {
                    tableNavigateRelatives.Add(new TableNavigateRelative()
                    {
                        RelativeTableId = relation.TableId,
                        TableNavigateType = TableNavigateType.OneToOne,
                        AssociationAColumnId = relation.RefColumnId,
                        AssociationATableId = relation.RefTableId,
                    });
                }
            }
            if (tableNavigateRelatives.Count > 0)
            {
                await _sqlSugarClient.Insertable(tableNavigateRelatives).ExecuteCommandAsync();
            }
        }
    }
}
