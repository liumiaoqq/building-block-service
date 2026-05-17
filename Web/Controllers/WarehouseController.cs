
using Web.Dto.Rules;
using Web.Dto.TableEntitys;
using Web.Dto.Warehouses;
using Web.Manager;
using Web.Tables;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WarehouseController : YouJuController<Warehouse, WarehouseDto, WarehousePagedInput>
    {

        private WarehouseManager _WarehouseManager;
        public WarehouseController(IServiceProvider serviceProvider, WarehouseManager WarehouseManager) : base(serviceProvider)
        {
            _WarehouseManager = WarehouseManager;
        }


        [HttpPost("ListAsync")]
        public async override Task<PagedReuslt<WarehouseDto>> ListAsync(WarehousePagedInput input)
        {
            RefAsync<int> totalCount = 0;
            var items = await SqlSugarClient.Queryable<Warehouse>()

                .OrderByDescending(x => x.CreationTime)
                .Select<WarehouseDto>()
                .ToPageListAsync(input.Page, input.Size, totalCount);

            return new PagedReuslt<WarehouseDto>(items, totalCount.Value);
        }


    }
}
