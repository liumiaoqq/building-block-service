using Web.Dto.Rules;
using Web.Dto.Warehouses;

namespace Web.Manager
{
    public class WarehouseManager
    {

        protected ISqlSugarClient _sqlSugarClient;

        private readonly IWebHostEnvironment _webHostEnvironment;
        public WarehouseManager(ISqlSugarClient sqlSugarClient, IWebHostEnvironment webHostEnvironment)
        {
            _sqlSugarClient = sqlSugarClient;
            _webHostEnvironment = webHostEnvironment;
        }




        /// <summary>
        /// 查询
        /// </summary>
        public async Task<List<WarehouseDto>> ListAsync(WarehousePagedInput input)
        {

            var rules = await _sqlSugarClient.Queryable<Warehouse>()
                .WhereIF(input.Id.HasValue, x => x.Id == input.Id.Value)
                .Select<WarehouseDto>().ToListAsync();
       
            return rules;
        }
    }
}
