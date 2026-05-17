using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web
{
    /// <summary>
    /// 系统Claim常量
    /// </summary>
    public static class YouJuClaimTypes
    {

        public static string UserName
        {
            get;
            set;
        } = "userName";


        public static string UserId
        {
            get;
            set;
        } = "userId";


        public static string OpenId
        {
            get;
            set;
        } = "openId";
        public static string UnionId
        {
            get;
            set;
        } = "unionId";

        public static string RoleIds
        {
            get;
            set;
        } = "roleIds";
  

        public static string TenantId
        {
            get; set;
        } = "tenantId";

        public static string SecuritySource
        {
            get; set;
        } = "securitySource";

    }
}
