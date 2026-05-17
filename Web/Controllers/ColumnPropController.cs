using System.Text;
using Web.Dto.TableEntitys;
using Web.Extensions;
using Web.Manager;
using Web.Service;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ColumnPropController : YouJuController<ColumnProp, ColumnPropDto, ColumnPropPagedInput>
    {

   
        private readonly ColumnPropService _columnPropService;
        public ColumnPropController(IServiceProvider serviceProvider,  ColumnPropService columnPropService) : base(serviceProvider)
        {
        
            _columnPropService = columnPropService;
        }

        [HttpPost("GetColumnProps")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public async Task<List<ColumnPropDto>> GetColumnProps(ColumnPropPagedInput input)
        {
            return await _columnPropService.GetColumnProps(input);

        }

        [HttpPost("GetColumnPropTypes")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]

        public PagedReuslt<SelectResult> GetColumnPropTypes(ColumnPropPagedInput input)
        {
            return _columnPropService.GetColumnPropTypes(input);

        }
        [HttpPost("BatchCreateOrEditColumnProp")]
        [CustomAuthorization(RoleType.用户, RoleType.系统管理员)]
        public async Task BatchCreateOrEditColumnProp(List<ColumnPropDto> input)
        {
            await _columnPropService.BatchCreateOrEditColumnProp(input);
        }





    }
}
