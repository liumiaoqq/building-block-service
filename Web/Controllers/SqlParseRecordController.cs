using Web.Extensions;
using Web.Manager;
using Web.Service;
using YouJu.Infrastructure.DbSqlScripts;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SqlParseRecordController : YouJuController<SqlParseRecord, SqlParseRecordDto, SqlParseRecordPagedInput>
    {
        private readonly SqlParseRecordService _sqlParseRecordService;
        public SqlParseRecordController(IServiceProvider serviceProvider, SqlParseRecordService  sqlParseRecordService) : base(serviceProvider)
        {
            _sqlParseRecordService = sqlParseRecordService;
        }

        [HttpPost("ListAsync")]
        public async override Task<PagedReuslt<SqlParseRecordDto>> ListAsync(SqlParseRecordPagedInput input)
        {
            
            RefAsync<int> totalCount = 0;
            var items = await SqlSugarClient.Queryable<SqlParseRecord>()
                .OrderByDescending(x => x.CreationTime)
                .Select<SqlParseRecordDto>()
                .ToPageListAsync(input.Page, input.Size, totalCount);

            var ids = items.Select(x => x.Id.Value).ToList();



            return new PagedReuslt<SqlParseRecordDto>(items, totalCount.Value);
        }

        [HttpPost("DeleteAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public override async Task DeleteAsync(IdInput<Guid> input)
        {

            await base.DeleteAsync(input);
        }

        [HttpPost("ParseSqlAsync")]
        [CustomAuthorization(RoleType.系统管理员, RoleType.用户)]
        public async Task<List<TableDefinition>> ParseSqlAsync(ParseSqlInput input)
        {

            return await _sqlParseRecordService.ParseSql(input);
        }

    }
}
