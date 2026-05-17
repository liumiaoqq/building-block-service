
using Web.Dto.Rules;
using Web.Dto.TableEntitys;
using Web.Manager;
using Web.Tables;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchRuleController : YouJuController<SearchRule, SearchRuleDto, SearchRulePagedInput>
    {

        private SearchRuleManager _searchRuleManager;
        public SearchRuleController(IServiceProvider serviceProvider, SearchRuleManager searchRuleManager) : base(serviceProvider)
        {
            _searchRuleManager = searchRuleManager;
        }


        [HttpPost("ListAsync")]
        public async override Task<PagedReuslt<SearchRuleDto>> ListAsync(SearchRulePagedInput input)
        {
            RefAsync<int> totalCount = 0;
            var items = await SqlSugarClient.Queryable<SearchRule>()

                .OrderByDescending(x => x.CreationTime)
                .Select<SearchRuleDto>()
                .ToPageListAsync(input.Page, input.Size, totalCount);

            return new PagedReuslt<SearchRuleDto>(items, totalCount.Value);
        }


    }
}
