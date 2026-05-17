using Web.Extensions;
using Web.Tables;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DicCodeController : YouJuController<DicCode, DicCodeDto, DicCodePagedInput>
    {
        public DicCodeController(IServiceProvider serviceProvider) : base(serviceProvider)
        {


        }

        [HttpPost("ListAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async override Task<PagedReuslt<DicCodeDto>> ListAsync(DicCodePagedInput input)
        {
            RefAsync<int> totalCount = 0;
            var items = await SqlSugarClient.Queryable<DicCode>()
                .WhereIF(input.DicTypeId.HasValue, x => x.DicTypeId == input.DicTypeId)
                .OrderByDescending(x => x.Sort)
                .Select<DicCodeDto>()
                .ToPageListAsync(input.Page, input.Size, totalCount);

            var ids = items.Select(x => x.DicTypeId).ToList();
            var dicTypeDtos = await SqlSugarClient.Queryable<DicType>().Where(x => ids.Contains(x.Id)).Select<DicTypeDto>().ToListAsync();


            foreach (var item in items)
            {
                item.DicTypeDto = dicTypeDtos.FirstOrDefault(x => x.Id == item.DicTypeId);
            }


            return new PagedReuslt<DicCodeDto>(items, totalCount.Value);
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

    }


}
