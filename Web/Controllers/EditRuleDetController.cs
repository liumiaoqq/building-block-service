
using System.Linq;
using Web.Dto.Rules;
using Web.Dto.TableEntitys;
using Web.Extensions;
using Web.Manager;
using Web.Tables;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EditRuleDetController : YouJuController<EditRuleDet, EditRuleDetDto, EditRuleDetPagedInput>
    {

        private EditRuleManager _EditRuleManager;
        public EditRuleDetController(IServiceProvider serviceProvider, EditRuleManager EditRuleManager) : base(serviceProvider)
        {
            _EditRuleManager = EditRuleManager;
        }


        [HttpPost("ListAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async override Task<PagedReuslt<EditRuleDetDto>> ListAsync(EditRuleDetPagedInput input)
        {
            RefAsync<int> totalCount = 0;
            var items = await SqlSugarClient.Queryable<EditRuleDet>()
                .WhereIF(input.EditRuleId.HasValue, x => x.EditRuleId == input.EditRuleId)
                .OrderByDescending(x => x.Sort)
                .Select<EditRuleDetDto>()
                .ToPageListAsync(input.Page, input.Size, totalCount);
            foreach (var item in items)
            {
                item.RegularExpressionDtos = item.RegularExpressions.ToList<RegularExpressionDto>();
            }
            return new PagedReuslt<EditRuleDetDto>(items, totalCount.Value);
        }

        [HttpPost("GetEditRuleMatchType")]
        [CustomAuthorization(RoleType.系统管理员)]
        public PagedReuslt<SelectResult> GetEditRuleMatchType()
        {
            var roles = new List<SelectResult>();

            roles = typeof(EditRuleMatchType).GetEnumList().Select(x => new SelectResult() { Name = x.Key, Value = x.Value }).ToList();
            return new PagedReuslt<SelectResult>(roles, roles.Count);

        }
        [HttpPost("GetEditFormType")]
        [CustomAuthorization(RoleType.系统管理员)]
        public PagedReuslt<SelectResult> GetEditFormType()
        {
            var roles = new List<SelectResult>();

            roles = typeof(EditFormType).GetEnumList().Select(x => new SelectResult() { Name = x.Key, Value = x.Value }).ToList();
            return new PagedReuslt<SelectResult>(roles, roles.Count);

        }
        [HttpPost("GetRegularExpressionMatchType")]
        [CustomAuthorization(RoleType.系统管理员)]
        public PagedReuslt<SelectResult> GetRegularExpressionMatchType()
        {
            var roles = new List<SelectResult>();

            roles = typeof(RegularExpressionMatchType).GetEnumList().Select(x => new SelectResult() { Name = x.Key, Value = x.Value }).ToList();
            return new PagedReuslt<SelectResult>(roles, roles.Count);

        }



        /// <summary>
        /// 创建
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="EditRuleDetDto"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("CreateOrEditAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public override async Task<EditRuleDetDto> CreateOrEditAsync(EditRuleDetDto input)
        {

            input.RegularExpressions = input.RegularExpressionDtos.ToJson();
            input.GuidNullToEmpty();



            var entity = await SqlSugarClient.Queryable<EditRuleDet>().FirstAsync(x => x.Id == input.Id);
            if (entity is null)
            {
                input.Id = Guid.Empty;
                entity = input.Clone<EditRuleDetDto, EditRuleDet>();


                entity = await SqlSugarClient.Insertable(entity).ExecuteReturnEntityAsync();
            }
            else
            {

                entity = input.Clone<EditRuleDetDto, EditRuleDet>();

                await SqlSugarClient.Updateable(entity).ExecuteCommandAsync();
            }
            return entity.Clone<EditRuleDet, EditRuleDetDto>();
        }
    }
}
