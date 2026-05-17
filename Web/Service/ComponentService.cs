using Web.Manager;

namespace Web.Service
{
    public class ComponentService
    {
        private readonly ComponentManager _componentManager;


     
        private readonly SystemModuleManager _systemModuleManager;

        public ComponentService(ComponentManager componentManager, SystemModuleManager systemModuleManager)
        {
            _componentManager = componentManager;
        
            _systemModuleManager = systemModuleManager;
        }





    }
}
