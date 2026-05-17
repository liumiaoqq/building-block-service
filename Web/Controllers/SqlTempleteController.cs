using Web.Extensions;
using Web.Service;
using Web.Tables;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SqlTempleteController : YouJuController<SqlTemplete, SqlTempleteDto, SqlTempletePagedInput>
    {
        private readonly SqlTempleteService _sqlTempleteService;
        public SqlTempleteController(IServiceProvider serviceProvider, SqlTempleteService sqlTempleteService) : base(serviceProvider)
        {
            _sqlTempleteService = sqlTempleteService;
        }

        [HttpPost("ListAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async override Task<PagedReuslt<SqlTempleteDto>> ListAsync(SqlTempletePagedInput input)
        {
            return await _sqlTempleteService.ListAsync(input);
        }
        [HttpPost("DeleteAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public override async Task DeleteAsync(IdInput<Guid> input)
        {
            var model = await GetAsync(input);

            await base.DeleteAsync(input);
        }

        [HttpPost("GetDataBaseType")]
        [CustomAuthorization(RoleType.系统管理员)]
        public PagedReuslt<SelectResult> GetDataBaseType()
        {
            var roles = new List<SelectResult>();

            roles = typeof(DataBaseType).GetEnumList().Select(x => new SelectResult() { Name = x.Key, Value = x.Value }).ToList();
            return new PagedReuslt<SelectResult>(roles, roles.Count);

        }



    }


}
