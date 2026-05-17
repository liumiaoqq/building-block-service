using Kdbndp.KingbaseTypes;
using System.Reflection;
using Web.Dto.Components;
using Web.Manager;

namespace Web.Service
{
    public class SystemModuleService
    {
        protected ISqlSugarClient _sqlSugarClient;

       

        private readonly SystemModuleManager _systemModuleManager;

        private readonly ComponentManager _componentManager;
        public SystemModuleService(ISqlSugarClient sqlSugarClient, ComponentManager componentManager, SystemModuleManager systemModuleManager)
        {
            _sqlSugarClient = sqlSugarClient;
            _componentManager = componentManager;
            _systemModuleManager = systemModuleManager;
        }

        /// <summary>
        /// 批量拷贝
        /// </summary>
        public async Task CopyModuleAsync(Guid[] moduleIds, Guid warehouseId)
        {


            var modulePaging = await _systemModuleManager.GetAllModule(new Dto.Plans.ModulePagedInput() { ModuleIds = moduleIds.ToList() });
            var modules = modulePaging.Items;

            foreach (var item in modules)
            {
                var copySystemModule = new SystemModule()
                {
                    Id = Guid.NewGuid(),
                    Label = item.Label,
                    Name = item.Name + "(拷贝)",
                    SystemModuleType = item.SystemModuleType,
                    
                    WarehouseId = warehouseId,

                };

                await _sqlSugarClient.Insertable(copySystemModule).ExecuteCommandAsync();

                await _componentManager.CopyModuleById(copySystemModule.Id, item.Id.Value);

            }

        }
    }
}
