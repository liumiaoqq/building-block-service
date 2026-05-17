using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Data;
using System.Linq;
using Web.Dto.Modules;
using Web.Dto.Plans;
using Web.Dto.Warehouses;
using Web.Extensions;
using Web.Manager;
using Web.Service;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ModuleController : YouJuController<SystemModule, SystemModuleDto, ModulePagedInput>
    {

        private readonly SystemModuleManager _systemModuleManager;

        private readonly ComponentManager _componentManager;


        private readonly SystemModuleService _systemModuleService;


        public ModuleController(IServiceProvider serviceProvider, SystemModuleManager systemModuleManager, ComponentManager componentManager, SystemModuleService systemModuleService) : base(serviceProvider)
        {
            _systemModuleManager = systemModuleManager;
            _componentManager = componentManager;
            _systemModuleService = systemModuleService;
        }


        [Description("获取模块树")]
        [HttpPost("ListAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public override async Task<PagedReuslt<SystemModuleDto>> ListAsync(ModulePagedInput input)
        {


            RefAsync<int> totalCount = 0;
            var modules = await SqlSugarClient.Queryable<SystemModule>()
                  .WhereIF(input.Label.IsNotNullOrNotWhiteSpace(), x => x.Label.Contains(input.Label))
                  .WhereIF(input.Name.IsNotNullOrNotWhiteSpace(), x => x.Name.Contains(input.Name))
                  .WhereIF(input.WarehouseId.HasValue, x => x.WarehouseId == input.WarehouseId)
                  .WhereIF(input.SystemModuleType.HasValue, x => x.SystemModuleType == input.SystemModuleType)
                  .WhereIF(input.ModuleIds.HasItem(), x => input.ModuleIds.Contains(x.Id))
                  .WhereIF(input.SystemModuleTypeList.HasItem(), x => input.SystemModuleTypeList.Contains(x.SystemModuleType))
                .WhereIF(input.ModuleIds.HasItem(), x => input.ModuleIds.Contains(x.Id)).Select<SystemModuleDto>().OrderBy(x => x.Label)
                .OrderByDescending(x => x.CreationTime)
                .ToPageListAsync(input.Page, input.Size, totalCount);

            var warehouseIds = modules.Where(x => x.WarehouseId.HasValue).Select(x => x.WarehouseId.Value).Distinct().ToList();
            if (warehouseIds.Count > 0)
            {
                var warehouseDtos = await SqlSugarClient.Queryable<Warehouse>().Where(x => warehouseIds.Contains(x.Id)).Select<WarehouseDto>().ToListAsync();
                foreach (var item in modules)
                {
                    item.WarehouseDto = warehouseDtos.FirstOrDefault(x => x.Id == item.WarehouseId) ?? new WarehouseDto();
                }
            }
            return new PagedReuslt<SystemModuleDto>(modules, totalCount);

        }



        [HttpPost("SelectAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public PagedReuslt<SelectResult> SelectAsync(ModulePagedInput input)
        {

            var moduleIds = new List<Guid>();
            if (input.PlanId.HasValue)
            {

                moduleIds = SqlSugarClient.Queryable<PlanModuleRelative>().Where(x => x.PlanId == input.PlanId).Select(x => x.ModuleId).ToList();
            }

            var items = SqlSugarClient.Queryable<SystemModule>()
                .WhereIF(moduleIds.Any(), x => !moduleIds.Contains(x.Id))
                .ToList()
                .OrderBy(x => x.SystemModuleType)
                .Select(x => new SelectResult() { Name = x.Name, Value = x.Id.ToString(), Label = x.Label })
                .ToList();
            return new PagedReuslt<SelectResult>(items.ToList(), items.Count());
        }


        [HttpPost("GetSystemModuleType")]
        [CustomAuthorization(RoleType.系统管理员)]
        public PagedReuslt<SelectResult> GetSystemModuleType(ModulePagedInput input)
        {
            var rs = new List<SelectResult>();

            rs = typeof(SystemModuleType).GetEnumList().Select(x => new SelectResult() { Name = x.Key, Value = x.Value }).ToList();
            return new PagedReuslt<SelectResult>(rs, rs.Count);

        }


        [HttpPost("CopyModuleAsync")]
        [Description("模块的拷贝")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async Task CopyModuleAsync(CopyModuleInput input)
        {
            await _systemModuleService.CopyModuleAsync(input.ModuleIds.ToArray(), input.WarehouseId);

        }



    }
}
