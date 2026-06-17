using YouJu.Infrastructure;

namespace Web.Tables
{
    /// <summary>
    /// 用户
    /// </summary>
    [YoungTable("AppUser")]
    public class AppUser : CreationAuditedAggregateRoot
    {


        public string UserName { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }


        public string Name { get; set; }


        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        public RoleType RoleIds { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string RoleName => RoleIds.ToDescription();

        /// <summary>
        /// 手机号码
        /// </summary>
        public string PhoneNumber { get; set; }


        /// <summary>
        ///  下次登录的时间
        /// </summary>
        public DateTime? LockoutEnd { get; set; }



        public string ImageUrls { get; set; }


        public string Address { get; set; }



        public string PersonalDescription { get; set; }


        /// <summary>
        /// 下载限制
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int? DownLimit { get; set; }

        /// <summary>
        /// 有效时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? ValidTime { get; set; }

        /// <summary>
        /// 最近登录的ip
        /// </summary>
        public string LastLoginIp { get; set; }

        /// <summary>
        /// 最近登录的时间
        /// </summary>
        public DateTime? LastLoginTime { get; set; }

        /// <summary>
        /// 邀请码
        /// </summary>

        public string InviteCode { get; set; }


      


    }



}
