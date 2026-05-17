using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Security.Authentication;
using YouJu.Infrastructure.Expection;

namespace Web.Extensions
{
    /// <summary>
    /// 全局异常拦截器
    /// </summary>
    public class YouJuGlobalExceptionFilter : IAsyncExceptionFilter
    {
        private readonly JsonSerializerSettings _jsonSetting = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver(), NullValueHandling = NullValueHandling.Ignore };

    
        public YouJuGlobalExceptionFilter()
        {
         
        }

        public async Task OnExceptionAsync(ExceptionContext context)
        {


            ILogger<YouJuGlobalExceptionFilter> logger = context.HttpContext.RequestServices.GetService<ILogger<YouJuGlobalExceptionFilter>>();
             ICurrentUser _currentUser = context.HttpContext.RequestServices.GetService<ICurrentUser >();



            if (context.Exception.GetType().IsAssignableTo(typeof(YouJuException)))
            {

                YouJuException ex = (YouJuException)context.Exception;

                context.Result = new JsonResult(new ApiResult()
                {
                    Code = ex.Code,
                    Msg = ex.Message,
                    Success = false,
                })
                {
                    StatusCode = StatusCodes.Status200OK,
                    ContentType = "application/json;charset=utf-8",
                    SerializerSettings = _jsonSetting,

                };

            }
            else if (context.Exception.GetType().IsAssignableTo(typeof(UnAuthenticationException)))
            {
                context.Result = new JsonResult(new ApiResult()
                {
                    Code = "401",
                    Msg = "登录信息已失效",
                    Success = false,
                })
                {
                    StatusCode = StatusCodes.Status200OK,
                    ContentType = "application/json;charset=utf-8",
                    SerializerSettings = _jsonSetting,

                };

            }
            else
            {
                if (_currentUser!=null&&_currentUser.GetRoleType() == RoleType.用户)
                {
                    context.Result = new JsonResult(new ApiResult()
                    {
                        Code = "500",
                        Msg = "系统异常,请联系管理员",
                        Success = false,
                    })
                    {
                        StatusCode = StatusCodes.Status200OK,
                        ContentType = "application/json;charset=utf-8",
                        SerializerSettings = _jsonSetting,

                    };
                }
                else
                {
                    context.Result = new JsonResult(new ApiResult()
                    {
                        Code = "500",
                        Msg = "系统异常,请联系管理员,错误原因:" + context.Exception.Message,
                        Success = false,
                    })
                    {
                        StatusCode = StatusCodes.Status200OK,
                        ContentType = "application/json;charset=utf-8",
                        SerializerSettings = _jsonSetting,

                    };
                }


            }


            context.ExceptionHandled = true;
            logger.LogError(context.Exception, context.Exception.Message);



        }
    }
}