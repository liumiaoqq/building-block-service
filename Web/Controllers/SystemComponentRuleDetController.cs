
using MathNet.Numerics.Statistics.Mcmc;
using Web.Dto.Components;
using Web.Dto.Rules;
using Web.Dto.TableEntitys;
using Web.Manager;
using Web.Tables;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SystemComponentRuleDetController : YouJuController<SystemComponentRuleDet, SystemComponentRuleDetDto, SystemComponentRuleDetPagedInput>
    {

        private SearchRuleManager _searchRuleManager;
        public SystemComponentRuleDetController(IServiceProvider serviceProvider, SearchRuleManager searchRuleManager) : base(serviceProvider)
        {
            _searchRuleManager = searchRuleManager;
        }

        [HttpPost("ListAsync")]
        
        public async override Task<PagedReuslt<SystemComponentRuleDetDto>> ListAsync(SystemComponentRuleDetPagedInput input)
        {
            RefAsync<int> totalCount = 0;
            var items = await SqlSugarClient.Queryable<SystemComponentRuleDet>()
                .WhereIF(input.SystemComponentRuleId.HasValue, x => x.SystemComponentRuleId == input.SystemComponentRuleId)
                .OrderByDescending(x => x.Sort)
                .Select<SystemComponentRuleDetDto>()
                .ToPageListAsync(input.Page, input.Size, totalCount);

            return new PagedReuslt<SystemComponentRuleDetDto>(items, totalCount.Value);
        }


    }
}
