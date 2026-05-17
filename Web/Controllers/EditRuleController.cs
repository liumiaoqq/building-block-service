
using Web.Dto.Rules;
using Web.Dto.TableEntitys;
using Web.Extensions;
using Web.Manager;
using Web.Tables;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EditRuleController : YouJuController<EditRule, EditRuleDto, EditRulePagedInput>
    {

        private EditRuleManager _EditRuleManager;
        public EditRuleController(IServiceProvider serviceProvider, EditRuleManager EditRuleManager) : base(serviceProvider)
        {
            _EditRuleManager = EditRuleManager;
        }


        [HttpPost("ListAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async override Task<PagedReuslt<EditRuleDto>> ListAsync(EditRulePagedInput input)
        {
            RefAsync<int> totalCount = 0;
            var items = await SqlSugarClient.Queryable<EditRule>()

                .OrderByDescending(x => x.CreationTime)
                .Select<EditRuleDto>()
                .ToPageListAsync(input.Page, input.Size, totalCount);

            return new PagedReuslt<EditRuleDto>(items, totalCount.Value);
        }


    }
}
