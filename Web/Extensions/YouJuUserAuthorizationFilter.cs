

using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace Web.Extensions
{
    public class YouJuUserAuthorizationFilter : IAsyncAuthorizationFilter
    {
        private readonly JsonSerializerSettings _jsonSetting = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver(), NullValueHandling = NullValueHandling.Ignore };

        public YouJuUserAuthorizationFilter()
        {
        }



        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            ICurrentUser _currentUser = context.HttpContext.RequestServices.GetService<ICurrentUser>();
            var  userId=_currentUser.GetUserId();

            var customAuthorizationAttribute = context.ActionDescriptor.EndpointMetadata.FirstOrDefault(x => x.GetType().IsAssignableTo(typeof(CustomAuthorizationAttribute))) as CustomAuthorizationAttribute;
            if (customAuthorizationAttribute != null)
            {
                if (userId == Guid.Empty) {
                    context.Result = new JsonResult(new ApiResult()
                    {
                        Code = "401",
                        Msg = "你无权访问此接口",
                        Success = false,
                    })
                    {
                        StatusCode = StatusCodes.Status200OK,
                        ContentType = "application/json;charset=utf-8",
                        SerializerSettings = _jsonSetting,
                    };
                    return Task.CompletedTask;
                }    


                if ( !customAuthorizationAttribute.RoleTypes.Exists(x => x == _currentUser.GetRoleType()))
                {
                 
                    context.Result = new JsonResult(new ApiResult()
                    {
                        Code = "403",
                        Msg = "你无权访问此接口",
                        Success = false,
                    })
                    {
                        StatusCode = StatusCodes.Status200OK,
                        ContentType = "application/json;charset=utf-8",
                        SerializerSettings = _jsonSetting,
                    };
                }
            }
          

            return Task.CompletedTask;

        }
    }
}
