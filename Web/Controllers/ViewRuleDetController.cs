
using MathNet.Numerics.Statistics.Mcmc;
using Web.Dto.Rules;
using Web.Dto.TableEntitys;
using Web.Manager;
using Web.Tables;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ViewRuleDetController : YouJuController<ViewRuleDet, ViewRuleDetDto, ViewRuleDetPagedInput>
    {

        private ViewRuleManager _ViewRuleManager;
        public ViewRuleDetController(IServiceProvider serviceProvider, ViewRuleManager ViewRuleManager) : base(serviceProvider)
        {
            _ViewRuleManager = ViewRuleManager;
        }


        [HttpPost("ListAsync")]
        public async override Task<PagedReuslt<ViewRuleDetDto>> ListAsync(ViewRuleDetPagedInput input)
        {
            RefAsync<int> totalCount = 0;
            var items = await SqlSugarClient.Queryable<ViewRuleDet>()
                .WhereIF(input.ViewRuleId.HasValue, x => x.ViewRuleId == input.ViewRuleId)
                .OrderByDescending(x => x.Sort)
                .Select<ViewRuleDetDto>()
                .ToPageListAsync(input.Page, input.Size, totalCount);

            return new PagedReuslt<ViewRuleDetDto>(items, totalCount.Value);
        }

        [HttpPost("GetViewRuleMatchType")]
        public PagedReuslt<SelectResult> GetViewRuleMatchType()
        {
            var roles = new List<SelectResult>();

            roles = typeof(ViewRuleMatchType).GetEnumList().Select(x => new SelectResult() { Name = x.Key, Value = x.Value }).ToList();
            return new PagedReuslt<SelectResult>(roles, roles.Count);

        }
        [HttpPost("GetViewColumnType")]
        public PagedReuslt<SelectResult> GetViewColumnType()
        {
            var roles = new List<SelectResult>();

            roles = typeof(ViewColumnType).GetEnumList().Select(x => new SelectResult() { Name = x.Key, Value = x.Value }).ToList();
            return new PagedReuslt<SelectResult>(roles, roles.Count);

        }



    }
}
