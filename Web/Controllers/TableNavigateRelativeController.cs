using K4os.Hash.xxHash;
using System.Text;
using Web.Dto.TableEntitys;
using Web.Extensions;
using Web.Service;
using YouJu.Infrastructure;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TableNavigateRelativeController : YouJuController<TableNavigateRelative, TableNavigateRelativeDto, TableNavigateRelativePagedInput>
    {

        private readonly TableNavigateRelativeService _tableNavigateRelativeService;
        public TableNavigateRelativeController(IServiceProvider serviceProvider, TableNavigateRelativeService tableNavigateRelativeService) : base(serviceProvider)
        {
            _tableNavigateRelativeService = tableNavigateRelativeService;
        }

        [HttpPost("GetTableNavigateRelativeList")]
        public async Task<List<TableNavigateRelativeDto>> GetTableNavigateRelativeList(TableNavigateRelativePagedInput input)
        {
            var props = await SqlSugarClient.Queryable<TableNavigateRelative>().Where(x => x.RelativeTableId == input.RelativeTableId).Select<TableNavigateRelativeDto>().ToListAsync();

            return props;
        }

        [HttpPost("ListAsync")]
        [CustomAuthorization(RoleType.系统管理员, RoleType.用户)]
        public override async Task<PagedReuslt<TableNavigateRelativeDto>> ListAsync(TableNavigateRelativePagedInput input)
        {

            return await _tableNavigateRelativeService.ListAsync(input);
            ;
        }
        [HttpPost("GetAsync")]
        [CustomAuthorization(RoleType.系统管理员, RoleType.用户)]
        public override async Task<TableNavigateRelativeDto> GetAsync(IdInput<Guid> input)
        {

            return await _tableNavigateRelativeService.GetAsync(input);
        }
        [HttpPost("DeleteAsync")]
        [CustomAuthorization(RoleType.系统管理员, RoleType.用户)]
        public override async Task DeleteAsync(IdInput<Guid> input)
        {
            await _tableNavigateRelativeService.DeleteAsync(input);
        }


        [HttpPost("CreateOrEditAsync")]
        [CustomAuthorization(RoleType.系统管理员, RoleType.用户)]
        public async override Task<TableNavigateRelativeDto> CreateOrEditAsync(TableNavigateRelativeDto input)
        {
            if (input.AssociationAColumnId.HasValue)
            {
                //查询这个列
                var column = await SqlSugarClient.Queryable<ColumnProp>().Where(x => x.Id == input.AssociationAColumnId.Value).FirstAsync();
                if (column.ColumnPropType != ColumnPropType.整型)
                {
                    throw new YouJuException($"外键列{column.Code}的类型不为int");
                }
            }


            var count = await SqlSugarClient.Queryable<TableNavigateRelative>().Where(x => x.RelativeTableId == input.RelativeTableId && (x.AssociationAColumnId == input.AssociationAColumnId
               || x.AssociationBColumnId == input.AssociationBColumnId))
                 .WhereIF(input.IsEidt, x => x.Id != input.Id)
                 .CountAsync();
            if (count > 0)
            {
                throw new YouJuException("请勿添加相同的配置");
            }

            return await base.CreateOrEditAsync(input);
        }

        [HttpPost("GetTableNavigateType")]

        public PagedReuslt<SelectResult> GetTableNavigateType()
        {
            var selects = new List<SelectResult>();

            selects = typeof(TableNavigateType).GetEnumList().Select(x => new SelectResult() { Name = x.Key, Value = x.Value }).ToList();
            return new PagedReuslt<SelectResult>(selects, selects.Count);

        }

    }
}
