using Org.BouncyCastle.Utilities;
using Web.Dto.TableEntitys;
using Web.Dto.Warehouses;

namespace Web.Manager
{
    public class SqlTempleteManager
    {
        protected ISqlSugarClient _sqlSugarClient;

        private readonly IWebHostEnvironment _webHostEnvironment;



        public SqlTempleteManager(ISqlSugarClient sqlSugarClient, IWebHostEnvironment webHostEnvironment)
        {
            _sqlSugarClient = sqlSugarClient;
            _webHostEnvironment = webHostEnvironment;
        }


        public async Task<PagedReuslt<SqlTempleteDto>> ListAsync(SqlTempletePagedInput input)
        {
            RefAsync<int> totalCount = 0;
            var items = await _sqlSugarClient.Queryable<SqlTemplete>()
                .WhereIF(input.WarehouseId.HasValue, x => x.WarehouseId == input.WarehouseId)
                .Select<SqlTempleteDto>()
                .ToPageListAsync(input.Page, input.Size, totalCount);

            //得到仓库ids
            var warehouseIds = items.Where(x => x.WarehouseId.HasValue).Select(x => x.WarehouseId.Value).Distinct().ToList();
            //查询出对应的仓库集合
            var warehouseDtos = await _sqlSugarClient.Queryable<Warehouse>().Where(x => warehouseIds.Contains(x.Id)).Select<WarehouseDto>().ToListAsync();
            foreach (var item in items)
            {
                item.WarehouseDto = warehouseDtos.FirstOrDefault(x => x.Id == item.WarehouseId);
            }
            return new PagedReuslt<SqlTempleteDto>(items, totalCount.Value);



        }
    }
}
