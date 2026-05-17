using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace Web.Extensions
{
    public class TimestampMiddleware
    {
        private readonly RequestDelegate _next;




        private readonly IConfiguration _configuration;


        private readonly JsonSerializerSettings _jsonSetting = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver(), NullValueHandling = NullValueHandling.Ignore };



        public TimestampMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }
        public async Task InvokeAsync(HttpContext context)
        {

            //
            if (context.Request.Headers.TryGetValue(SysConst.XRequestSource, out var xs))
            {
                var xrWhiteUrls = _configuration.GetSection(SysConst.RequestSourceWhilteUrls).AsEnumerable().Where(x => x.Value.IsNotNullOrNotWhiteSpace()).Select(x => x.Value).ToList();
                if (xrWhiteUrls.Exists(x => x == xs)) {
                    await _next(context);
                    return;
                }
            }


            var url = context.Request.Path.ToString();
            var whiteUrls = _configuration.GetSection(SysConst.TimestampWhilteUrls).AsEnumerable().Where(x=>x.Value.IsNotNullOrNotWhiteSpace()).Select(x => x.Value).ToList();
            if (whiteUrls.Exists(x => url.Contains(x, StringComparison.OrdinalIgnoreCase)))
            {
                await _next(context);
                return;
            }


            if (!context.Request.Headers.TryGetValue(SysConst.XTicks, out var timestampStr))
            {
                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.ContentType = "application/json;charset=utf-8";

                await context.Response.WriteAsync(JsonConvert.SerializeObject(new ApiResult() { Code = StatusCodes.Status500InternalServerError.ToString(), Msg = "缺少时间戳", Success = false }, _jsonSetting));

                return;
            }

            if (!long.TryParse(timestampStr, out long timestamp))
            {
                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.ContentType = "application/json;charset=utf-8";

                await context.Response.WriteAsync(JsonConvert.SerializeObject(new ApiResult() { Code = StatusCodes.Status500InternalServerError.ToString(), Msg = "无效的时间戳", Success = false }, _jsonSetting));

                return;
            }


            // 转换为Unix时间戳（以毫秒为单位）
            long currentUnixTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            if (Math.Abs(currentUnixTimestamp - timestamp) > 10 * 1000)
            {
                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.ContentType = "application/json;charset=utf-8";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new ApiResult() { Code = StatusCodes.Status500InternalServerError.ToString(), Msg = "本次请求已经失效,请勿重复请求", Success = false }, _jsonSetting));

                return;
            }

            await _next(context);
        }
    }
}
