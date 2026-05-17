using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
namespace Web
{
    /// <summary>
    /// 当前请求管道用户上下文信息
    /// </summary>
    public class CurrentUser : ICurrentUser
    {
        private HttpContext _httpContext;
        public CurrentUser()
        {
            _httpContext = YouJuServiceProvider.GetContext();
        }
        public Guid? UserId
        {
            get
            {
                if (Guid.TryParse(GetCliam(YouJuClaimTypes.UserId)?.Value, out Guid userId))
                {
                    return userId;
                }
               
                return null;
            }

        }
        public RoleType? RoleIds
        {
            get
            {
                if (int.TryParse(GetCliam(YouJuClaimTypes.RoleIds)?.Value, out int roleIds))
                {
                    return (RoleType)roleIds;
                }

                return null;
            }

        }



        public Guid GetUserId()
        {
            return UserId ?? Guid.Empty;
        }
        public RoleType GetRoleType()
        {
            return RoleIds ?? RoleType.用户;
        }

        public bool IsUser() {
            return RoleIds == RoleType.用户;
        }

        public Claim GetCliam(string type)
        {
            return _httpContext?.User?.Claims?.FirstOrDefault(x => x.Type == type);
        }


    }
}
