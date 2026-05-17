
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace YouJu.Infrastructure.JWT
{
    /// <summary>
    /// JWT认证接口
    /// </summary>
     public  interface  IJwtHelper
    {
        abstract string GetToken(List<Claim> claims);


        abstract string GetToken(List<Claim> claims,long expiresTime);
    }
}
