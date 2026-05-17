using Web.Dto.Components;
using Web.Manager;

namespace Web.Service
{
    public class SystemComponentRuleService
    {
        protected ISqlSugarClient _sqlSugarClient;



        private readonly SystemComponentRuleManager _systemComponentSettingManager;
        public SystemComponentRuleService(ISqlSugarClient sqlSugarClient, SystemComponentRuleManager systemComponentSettingManager)
        {
            _sqlSugarClient = sqlSugarClient;
            _systemComponentSettingManager = systemComponentSettingManager;
        }

        public async Task<PagedReuslt<SystemComponentRuleDto>> ListAsync(SystemComponentRulePagedInput input)
        {
            return await _systemComponentSettingManager.ListAsync(input);

        }
        public async Task<SystemComponentRuleDto> GetAsync(SystemComponentRulePagedInput input)
        {

            return await _systemComponentSettingManager.GetAsync(input.Id,input.WarehouseId,input.LanguageWay);
        }

    }

        
}
