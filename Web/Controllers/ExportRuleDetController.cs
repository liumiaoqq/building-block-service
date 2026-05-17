
using MathNet.Numerics.Statistics.Mcmc;
using Web.Dto.Rules;
using Web.Dto.TableEntitys;
using Web.Extensions;
using Web.Manager;
using Web.Tables;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExportRuleDetController : YouJuController<ExportRuleDet, ExportRuleDetDto, ExportRuleDetPagedInput>
    {

        private ExportRuleManager _exportRuleManager;
        public ExportRuleDetController(IServiceProvider serviceProvider, ExportRuleManager exportRuleManager) : base(serviceProvider)
        {
            _exportRuleManager = exportRuleManager;
        }


        [HttpPost("ListAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async override Task<PagedReuslt<ExportRuleDetDto>> ListAsync(ExportRuleDetPagedInput input)
        {
            RefAsync<int> totalCount = 0;
            var items = await SqlSugarClient.Queryable<ExportRuleDet>()
                .WhereIF(input.ExportRuleId.HasValue, x => x.ExportRuleId == input.ExportRuleId)
                .OrderByDescending(x => x.Sort)
                .Select<ExportRuleDetDto>()
                .ToPageListAsync(input.Page, input.Size, totalCount);

            return new PagedReuslt<ExportRuleDetDto>(items, totalCount.Value);
        }

        [HttpPost("GetExportRuleMatchType")]
        [CustomAuthorization(RoleType.系统管理员)]
        public PagedReuslt<SelectResult> GetExportRuleMatchType()
        {
            var roles = new List<SelectResult>();

            roles = typeof(ExportRuleMatchType).GetEnumList().Select(x => new SelectResult() { Name = x.Key, Value = x.Value }).ToList();
            return new PagedReuslt<SelectResult>(roles, roles.Count);

        }
        [HttpPost("GetExportRuleDispatchType")]
        [CustomAuthorization(RoleType.系统管理员)]
        public PagedReuslt<SelectResult> GetExportRuleDispatchType()
        {
            var roles = new List<SelectResult>();

            roles = typeof(ExportRuleDispatchType).GetEnumList().Select(x => new SelectResult() { Name = x.Key, Value = x.Value }).ToList();
            return new PagedReuslt<SelectResult>(roles, roles.Count);

        }

       

    }
}
