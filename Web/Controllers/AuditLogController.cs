using Web.Dto.AuditLogs;
using Web.Extensions;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuditLogController : YouJuController<AuditLog, AuditLogDto, AuditLogPagedInput>
    {
        public AuditLogController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpPost("ListAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public override async Task<PagedReuslt<AuditLogDto>> ListAsync(AuditLogPagedInput input)
        {
            RefAsync<int> totalCount = 0;
            var items = await SqlSugarClient.Queryable<AuditLog>()
                .WhereIF(input.UserName.IsNotNullOrNotWhiteSpace(), x => x.UserName.Contains(input.UserName))
                .WhereIF(input.Path.IsNotNullOrNotWhiteSpace(), x => x.Path.Contains(input.Path))
                .WhereIF(input.HttpMethod.IsNotNullOrNotWhiteSpace(), x => x.HttpMethod == input.HttpMethod)
                .WhereIF(input.ClientIp.IsNotNullOrNotWhiteSpace(), x => x.ClientIp.Contains(input.ClientIp))
                .WhereIF(input.IsSuccess.HasValue, x => x.IsSuccess == input.IsSuccess.Value)
                .OrderByDescending(x => x.CreationTime)
                .Select<AuditLogDto>()
                .ToPageListAsync(input.Page, input.Size, totalCount);

            return new PagedReuslt<AuditLogDto>(items, totalCount.Value);
        }
    }
}
