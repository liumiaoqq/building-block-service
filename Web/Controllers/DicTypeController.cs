using Web.Extensions;
using Web.Manager;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DicTypeController : YouJuController<DicType, DicTypeDto, DicTypePagedInput>
    {
        private readonly DicManager _dicManager;
        public DicTypeController(IServiceProvider serviceProvider, DicManager dicManager) : base(serviceProvider)
        {
            _dicManager = dicManager;
        }

        [HttpPost("ListAsync")]
        public async override Task<PagedReuslt<DicTypeDto>> ListAsync(DicTypePagedInput input)
        {
            RefAsync<int> totalCount = 0;
            var items = await SqlSugarClient.Queryable<DicType>()
                .OrderByDescending(x=>x.Sort)
                .Select<DicTypeDto>()
                .ToPageListAsync(input.Page, input.Size, totalCount);

            var ids = items.Select(x => x.Id.Value).ToList();

          

            return new PagedReuslt<DicTypeDto>(items, totalCount.Value);
        }

        [HttpPost("DeleteAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public override async Task DeleteAsync(IdInput<Guid> input)
        {
            var model = await GetAsync(input);
            if (model.IsSystem)
            {
                throw new YouJuException("系统级别,不支持删除");
            }
            await base.DeleteAsync(input);
        }

        [HttpPost("GetTypeList")]
        public async Task<PagedReuslt<SelectResult>> GetTypeList(DicCodePagedInput input)
        {
            var dicCodes = await _dicManager.GetDicCodeList(input);
            if (!dicCodes.HasItem())
            {
                throw new YouJuException("请配置编辑ValidRules");
            }
            var selects = dicCodes.Select(x => new SelectResult() { Name = x.Name, Value = x.Code, Label = x.Remark, Prop = x }).ToList();
            return new PagedReuslt<SelectResult>(selects, selects.Count);
        }
    }
}
