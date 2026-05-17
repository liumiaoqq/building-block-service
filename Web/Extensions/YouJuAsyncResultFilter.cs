using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Net;
using System.IO.Compression;

namespace Web.Extensions
{
    public class YouJuAsyncResultFilter : IAsyncResultFilter
    {

        private JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
             Formatting= Formatting.None,
        };

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {

            ApiResult apiResult = null;
            var objResult = context.Result as ObjectResult;


            if (context.Result is BadRequestObjectResult)
            {
                BadRequestObjectResultFilter(context);
            }
            else if (context.Result is EmptyResult)
            {
                context.Result = new JsonResult(new ApiResultData<object>()
                {
                    Code = "200",
                    Msg = "成功",
                    Data = null,
                    Success = true,
                })
                {
                    // 返回状态码设置为200，表示成功
                    StatusCode = (int)HttpStatusCode.OK,
                    // 设置返回格式
                    ContentType = "application/json;charset=utf-8",

                };


            }
            else if (objResult?.Value is ApiResult)
            {

                context.Result = new JsonResult(new ApiResultData<object>()
                {
                    Code = "200",
                    Msg = "成功",
                    Data = objResult.Value,
                    Success = true,
                })
                {
                    // 返回状态码设置为200，表示成功
                    StatusCode = (int)HttpStatusCode.OK,
                    // 设置返回格式
                    ContentType = "application/json;charset=utf-8",

                };



            }
            else if (objResult?.Value != null)
            {

                context.Result = new JsonResult(new ApiResultData<object>()
                {
                    Code = "200",
                    Msg = "成功",
                    Data = objResult.Value,
                    Success = true,
                })
                {
                    // 返回状态码设置为200，表示成功
                    StatusCode = (int)HttpStatusCode.OK,
                    // 设置返回格式
                    ContentType = "application/json;charset=utf-8",

                };


            }



            await next();
        }
        /// <summary>
        /// 400处理过滤
        /// </summary>
        /// <param name="context"></param>

        public void BadRequestObjectResultFilter(ResultExecutingContext context)
        {
            var badRequest = (BadRequestObjectResult)context.Result;
            if (badRequest.Value != null)
            {
                var detail = (ValidationProblemDetails)badRequest.Value;


                context.Result = new ContentResult
                {
                    // 返回状态码设置为200，表示成功
                    StatusCode = (int)HttpStatusCode.OK,

                    // 设置返回格式
                    ContentType = "application/json;charset=utf-8",
                    Content = JsonConvert.SerializeObject(new ApiResultData<object>()
                    {
                        Code = ((int)HttpStatusCode.BadRequest).ToString(),
                        Msg = "请检查参数是否正确",
                        Data = detail.Errors,
                        Success = false,
                    }, Formatting.None, jsonSerializerSettings)
                };
            }




        }

        //public static string CompressJson(string json)
        //{
        
        //    using var memoryStream = new MemoryStream();
        //    using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
        //    {
        //        gzipStream.Write(bytes);
        //    }

        //    return Convert.ToBase64String(memoryStream.ToArray());
        //}

    }
}