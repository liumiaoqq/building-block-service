using Web.Dto.Plans;

namespace Web.Dto.Modules
{
    /// <summary>
    /// 计划-模块关联传输模型
    /// </summary>
    public class ModuleRelativeDtos
    {
    

        /// <summary>
        /// 模块类型
        /// </summary>
        public SystemModuleType SystemModuleType { get; set; }


        public string SortLabel { get; set; }

        /// <summary>
        /// 模块类型描述
        /// </summary>
        public string SystemModuleTypeFormat=> SystemModuleType.ToDescription();

        public List<Guid> CheckdModuleIds { get; set; }

        /// <summary>
        /// 对应的模块
        /// </summary>
        public List<SystemModuleWapperDto> SystemModuleWapperDtoList { get; set; }


    }
}
