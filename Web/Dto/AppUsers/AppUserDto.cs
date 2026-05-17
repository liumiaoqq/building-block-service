using YouJu.Infrastructure;

namespace Web.Dto.AppUsers
{
    public class AppUserDto : FullBaseDto
    {

        public string UserName { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }


        public string Address { get; set; }


        public string Name { get; set; }

        public RoleType RoleIds { get; set; }

        public string RoleName => RoleIds.ToDescription();

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string PhoneNumber { get; set; }


        /// <summary>
        ///  下次登录的时间
        /// </summary>
        public DateTime? LockoutEnd { get; set; }


        public bool IsLock { get; set; }

        public string ImageUrls { get; set; }

        public string Code { get; set; }


        public string PersonalDescription { get; set; }

        /// <summary>
        /// 下载限制
        /// </summary>
        public int? DownLimit { get; set; }

        /// <summary>
        /// 邀请码
        /// </summary>
        public string InviteCode { get; set; }
        /// <summary>
        /// 有效时间
        /// </summary>

        public DateTime? ValidTime { get; set; }

        /// <summary>
        /// 最近登录的ip
        /// </summary>
        public string LastLoginIp { get; set; }

        /// <summary>
        /// 最近登录的时间
        /// </summary>
        public DateTime? LastLoginTime { get; set; }


    }


}
