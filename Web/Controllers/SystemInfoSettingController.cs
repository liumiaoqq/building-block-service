using Web.Dto.TableFunctionList;
using Web.Extensions;
using Web.Service;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SystemInfoSettingController : ControllerBase
    {

        [HttpPost("GetSettings")]
     
        public async Task<object> GetSettings(TableEntityPagedInput input)
        {
            // 获取当前时间
            DateTimeOffset currentTime = DateTimeOffset.Now;

            // 转换为Unix时间戳（以毫秒为单位）
            long unixTimestamp = currentTime.ToUnixTimeMilliseconds();
            return new { 
                Ticks= unixTimestamp,
            };
        }

    }
}
