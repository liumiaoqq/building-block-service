
using MathNet.Numerics.Statistics.Mcmc;
using Web.Dto.Rules;
using Web.Dto.TableEntitys;
using Web.Manager;
using Web.Tables;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchRuleDetController : YouJuController<SearchRuleDet, SearchRuleDetDto, SearchRuleDetPagedInput>
    {

        private SearchRuleManager _searchRuleManager;
        public SearchRuleDetController(IServiceProvider serviceProvider, SearchRuleManager searchRuleManager) : base(serviceProvider)
        {
            _searchRuleManager = searchRuleManager;
        }

        [HttpPost("ListAsync")]
        public async override Task<PagedReuslt<SearchRuleDetDto>> ListAsync(SearchRuleDetPagedInput input)
        {
            RefAsync<int> totalCount = 0;
            var items = await SqlSugarClient.Queryable<SearchRuleDet>()
                .WhereIF(input.SearchRuleId.HasValue, x => x.SearchRuleId == input.SearchRuleId)
                .OrderByDescending(x => x.Sort)
                .Select<SearchRuleDetDto>()
                .ToPageListAsync(input.Page, input.Size, totalCount);

            return new PagedReuslt<SearchRuleDetDto>(items, totalCount.Value);
        }

        [HttpPost("GetSearchRuleMatchType")]
        public PagedReuslt<SelectResult> GetSearchRuleMatchType()
        {
            var roles = new List<SelectResult>();

            roles = typeof(SearchRuleMatchType).GetEnumList().Select(x => new SelectResult() { Name = x.Key, Value = x.Value }).ToList();
            return new PagedReuslt<SelectResult>(roles, roles.Count);

        }
        [HttpPost("GetSearchType")]
        public PagedReuslt<SelectResult> GetSearchType()
        {
            var roles = new List<SelectResult>();

            roles = typeof(SearchType).GetEnumList().Select(x => new SelectResult() { Name = x.Key, Value = x.Value }).ToList();
            return new PagedReuslt<SelectResult>(roles, roles.Count);

        }



    }
}
