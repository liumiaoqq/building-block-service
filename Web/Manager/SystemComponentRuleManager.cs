using Web.Dto.Components;
using Web.Dto.Plans;
using Web.Dto.Rules;
using Web.Dto.Warehouses;

namespace Web.Manager
{
    public class SystemComponentRuleManager
    {
        protected ISqlSugarClient _sqlSugarClient;

        public SystemComponentRuleManager(ISqlSugarClient sqlSugarClient)
        {
            _sqlSugarClient = sqlSugarClient;

        }

     

        public async Task<PagedReuslt<SystemComponentRuleDto>> ListAsync(SystemComponentRulePagedInput input)
        {
            RefAsync<int> totalCount = 0;
            var rules = await _sqlSugarClient
                .Queryable<SystemComponentRule>()
                .WhereIF(input.LanguageWay.HasValue, x => x.LanguageWay == input.LanguageWay)
                .WhereIF(input.WarehouseId.HasValue, x => x.WarehouseId == input.WarehouseId)
                .OrderByDescending(x => x.CreationTime)
                .Select<SystemComponentRuleDto>()
                .ToPageListAsync(input.Page, input.Size, totalCount);


            var warehouseIds = rules.Where(x => x.WarehouseId.HasValue).Select(x => x.WarehouseId.Value).Distinct().ToList();

            var ruleIds = rules.Select(x => x.Id).ToList();
            var ruleDets= await _sqlSugarClient.Queryable<SystemComponentRuleDet>().Where(x => ruleIds.Contains(x.SystemComponentRuleId)).Select<SystemComponentRuleDetDto>().ToListAsync();
            if (warehouseIds.Count > 0)
            {
                var warehouseDtos = await _sqlSugarClient.Queryable<Warehouse>().Where(x => warehouseIds.Contains(x.Id)).Select<WarehouseDto>().ToListAsync();
                foreach (var item in rules)
                {
                    item.WarehouseDto = warehouseDtos.FirstOrDefault(x => x.Id == item.WarehouseId) ?? new WarehouseDto();
                }

            }
            foreach (var item in rules)
            {
                item.SystemComponentRuleDetDtos = ruleDets.Where(x => x.SystemComponentRuleId == item.Id).ToList();
            }


            return new PagedReuslt<SystemComponentRuleDto>(rules, totalCount);
        }

        public async Task<SystemComponentRuleDto> GetAsync(Guid? id,Guid? warehouseId,LanguageWay? languageWay)
        {
            RefAsync<int> totalCount = 0;
            var ruleDto = await _sqlSugarClient
                .Queryable<SystemComponentRule>()
                .WhereIF(id!=null, x => x.Id==id)
                .WhereIF(warehouseId!=null, x=>x.WarehouseId==warehouseId)
                .WhereIF(languageWay != null,x=>x.LanguageWay== languageWay)
                .OrderByDescending(x => x.CreationTime)
                .Select<SystemComponentRuleDto>()
                .FirstAsync();

            ruleDto.WarehouseDto= await _sqlSugarClient.Queryable<Warehouse>().Where(x => ruleDto.WarehouseId==x.Id).Select<WarehouseDto>().FirstAsync();
            ruleDto.SystemComponentRuleDetDtos = await _sqlSugarClient.Queryable<SystemComponentRuleDet>().Where(x => x.SystemComponentRuleId == ruleDto.Id).Select<SystemComponentRuleDetDto>().ToListAsync();

            return ruleDto;
        }



        public virtual async Task<SystemComponentRuleDto> CreateOrEditAsync(SystemComponentRuleDto input)
        {

            var exsitCount = await _sqlSugarClient.Queryable<SystemComponentRule>()
                .WhereIF(input.Id != null, x => x.Id != input.Id)
                .WhereIF(input.WarehouseId.HasValue,x => x.WarehouseId == input.WarehouseId && x.LanguageWay == input.LanguageWay).CountAsync();
            


            input.Name = input.Name.Trim();
            input.GuidNullToEmpty();
            var entity = await _sqlSugarClient.Queryable<SystemComponentRule>().FirstAsync(x => x.Id == input.Id);
            if (entity is null)
            {
                input.Id = Guid.Empty;
                entity = input.Clone<SystemComponentRuleDto, SystemComponentRule>();


                entity = await _sqlSugarClient.Insertable(entity).ExecuteReturnEntityAsync();
            }
            else
            {

                entity = input.Clone<SystemComponentRuleDto, SystemComponentRule>();

                await _sqlSugarClient.Updateable(entity).ExecuteCommandAsync();
            }
            return entity.Clone<SystemComponentRule, SystemComponentRuleDto>();
        }

    }
}
