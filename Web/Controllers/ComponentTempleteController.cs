using Web.Dto.Components;
using Web.Manager;
using YouJu.Infrastructure.Extensions;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ComponentTempleteController : YouJuController<ComponentTemplete, ComponentTempleteDto, ComponentTempletePagedInput>
    {
        public readonly ComponentManager _componentManager;



        public ComponentTempleteController(IServiceProvider serviceProvider, ComponentManager componentManager) : base(serviceProvider)
        {
            _componentManager = componentManager;
        }

        [HttpPost("CreateOrEditAsync")]
        public override Task<ComponentTempleteDto> CreateOrEditAsync(ComponentTempleteDto input)
        {
            input.FileType = input.FileName.GetFileTypes();

            return base.CreateOrEditAsync(input);
        }


        /// <summary>
        /// 公共获取单个对象
        /// </summary>
        [HttpPost("GetAsync")]
        public override async Task<ComponentTempleteDto> GetAsync(IdInput<Guid> input)
        {

            var dto = await SqlSugarClient.Queryable<ComponentTemplete>().Select<ComponentTempleteDto>().FirstAsync(x => x.Id == input.Id);
            return dto;
        }

    }
}
