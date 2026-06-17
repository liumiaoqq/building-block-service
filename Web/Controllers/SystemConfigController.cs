using Web.Dto.SystemConfigs;
using Web.Extensions;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SystemConfigController : YouJuController<SystemConfig, SystemConfigDto, SystemConfigPagedInput>
    {
        public SystemConfigController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpPost("ListAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public override async Task<PagedReuslt<SystemConfigDto>> ListAsync(SystemConfigPagedInput input)
        {
            var config = await EnsureDefaultConfigAsync();
            return new PagedReuslt<SystemConfigDto>(new List<SystemConfigDto> { config.Clone<SystemConfig, SystemConfigDto>() }, 1);
        }

        [HttpPost("GetAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public override async Task<SystemConfigDto> GetAsync(IdInput<Guid> input)
        {
            var config = await EnsureDefaultConfigAsync();
            return config.Clone<SystemConfig, SystemConfigDto>();
        }

        [HttpPost("CreateOrEditAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public override async Task<SystemConfigDto> CreateOrEditAsync(SystemConfigDto input)
        {
            var config = await EnsureDefaultConfigAsync();
            config.IsAuditEnabled = input.IsAuditEnabled;
            await SqlSugarClient.Updateable(config).ExecuteCommandAsync();
            return config.Clone<SystemConfig, SystemConfigDto>();
        }

        [HttpPost("SetAuditEnabled")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async Task SetAuditEnabled(SystemConfigDto input)
        {
            var config = await EnsureDefaultConfigAsync();
            config.IsAuditEnabled = input.IsAuditEnabled;
            await SqlSugarClient.Updateable(config).ExecuteCommandAsync();
        }

        [HttpPost("GetAuditEnabled")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async Task<bool> GetAuditEnabled()
        {
            var config = await EnsureDefaultConfigAsync();
            return config.IsAuditEnabled;
        }

        [NonAction]
        public async Task<SystemConfig> EnsureDefaultConfigAsync()
        {
            var config = await SqlSugarClient.Queryable<SystemConfig>().OrderBy(x => x.CreationTime).FirstAsync();
            if (config != null)
            {
                return config;
            }

            config = new SystemConfig
            {
                IsAuditEnabled = false
            };
            return await SqlSugarClient.Insertable(config).ExecuteReturnEntityAsync();
        }
    }
}
