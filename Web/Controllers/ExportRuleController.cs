
using Web.Dto.Rules;
using Web.Dto.TableEntitys;
using Web.Extensions;
using Web.Manager;
using Web.Tables;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExportRuleController : YouJuController<ExportRule, ExportRuleDto, ExportRulePagedInput>
    {

        private ExportRuleManager _exportRuleManager;
        public ExportRuleController(IServiceProvider serviceProvider, ExportRuleManager exportRuleManager) : base(serviceProvider)
        {
            _exportRuleManager = exportRuleManager;
        }


        [HttpPost("ListAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async override Task<PagedReuslt<ExportRuleDto>> ListAsync(ExportRulePagedInput input)
        {
            RefAsync<int> totalCount = 0;
            var items = await SqlSugarClient.Queryable<ExportRule>()

                .OrderByDescending(x => x.CreationTime)
                .Select<ExportRuleDto>()
                .ToPageListAsync(input.Page, input.Size, totalCount);

            return new PagedReuslt<ExportRuleDto>(items, totalCount.Value);
        }


    }
}
