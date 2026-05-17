
using Web.Dto.Rules;
using Web.Dto.TableEntitys;
using Web.Manager;
using Web.Tables;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ViewRuleController : YouJuController<ViewRule, ViewRuleDto, ViewRulePagedInput>
    {

        private ViewRuleManager _viewRuleManager;
        public ViewRuleController(IServiceProvider serviceProvider, ViewRuleManager viewRuleManager) : base(serviceProvider)
        {
            _viewRuleManager = viewRuleManager;
        }


        [HttpPost("ListAsync")]
        public async override Task<PagedReuslt<ViewRuleDto>> ListAsync(ViewRulePagedInput input)
        {
            RefAsync<int> totalCount = 0;
            var items = await SqlSugarClient.Queryable<ViewRule>()

                .OrderByDescending(x => x.CreationTime)
                .Select<ViewRuleDto>()
                .ToPageListAsync(input.Page, input.Size, totalCount);

            return new PagedReuslt<ViewRuleDto>(items, totalCount.Value);
        }


    }
}
