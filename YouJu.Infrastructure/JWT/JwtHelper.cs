
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace YouJu.Infrastructure.JWT
{
    /// <summary>
    /// JWT认证实现类
    /// </summary>
    public class JwtHelper : IJwtHelper
    {
        public IConfiguration _configuration;

        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetToken(List<Claim> claims)
        {
           
            return GetToken(claims, 60 * 12);

        }

        public string GetToken(List<Claim> claims, long expiresTime)
        {
            //注：下面调用方法都是使用了默认的header
            //初始化payload

            var Audience = _configuration["JwtAuthentication:Audience"];
            var Issuer = _configuration["JwtAuthentication:Issuer"];
            var SecureKey = _configuration["JwtAuthentication:SecureKey"];
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JwtAuthentication:SecureKey").ToString()));
            /**
           *  Claims (Payload)
              Claims 部分包含了一些跟这个 token 有关的重要信息。 JWT 标准规定了一些字段，下面节选一些字段:
              iss: jwt签发者
              sub: jwt所面向的用户
              aud: 接收jwt的一方
              exp: jwt的过期时间，这个过期时间必须要大于签发时间
              nbf: 定义在什么时间之前，该jwt都是不可用的.
              iat: jwt的签发时间
              jti: jwt的唯一身份标识，主要用来作为一次性token,从而回避重放攻击。
              除了规定的字段外，可以包含其他任何 JSON 兼容的字段。
           * */
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtAuthentication:Issuer"],//设置签发者
                audience: _configuration["JwtAuthentication:Audience"],//设置接收者
                claims: claims,//设置payload

                expires: expiresTime<=0?DateTime.Now.AddMinutes(5):DateTime.Now.AddMinutes(expiresTime),//12个小时有效期 按分钟
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));//初始化安全令牌参数
            //输出token
            string returnToken = new JwtSecurityTokenHandler().WriteToken(token);
            return returnToken;
        }
    }

}
