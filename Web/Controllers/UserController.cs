
using Microsoft.Extensions.Options;
using NPOI.SS.Formula.Functions;
using System.Data;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Web.Dto.AppUsers;
using Web.Dto.Components;
using Web.Manager;
using Web.Service;
using Web.Tables;
using YouJu.Infrastructure;
using YouJu.Infrastructure.Email;
using YouJu.Infrastructure.JWT;

namespace Web.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class UserController : YouJuController<AppUser, AppUserDto, CommPagedInput>
    {

        public readonly IJwtHelper jwtHelper;

        private readonly ILogger<UserController> _logger;

        private readonly IMailService _mailService;

        private readonly MailSettings _mailSettings;

        private readonly InviteRecordService _inviteRecordService;

        private readonly DrawingBalanceManager _drawingBalanceManager;

        public UserController(IServiceProvider serviceProvider, IMailService mailService, ILogger<UserController> logger, IOptions<MailSettings> mailSettings, InviteRecordService inviteRecordService, DrawingBalanceManager drawingBalanceManager) : base(serviceProvider)
        {
            jwtHelper = ServiceProvider.GetRequiredService<IJwtHelper>();
            _mailService = mailService;
            _logger = logger;
            _mailSettings = mailSettings.Value;
            _inviteRecordService = inviteRecordService;
            _drawingBalanceManager = drawingBalanceManager;
        }


        [HttpPost("ListAsync")]


        public override async Task<PagedReuslt<AppUserDto>> ListAsync(CommPagedInput input)
        {
            RefAsync<int> totalCount = 0;
            var items = await SqlSugarClient.Queryable<AppUser>()
                .WhereIF(input.IsAuth, x => x.CreatorId == CurrentUser.GetUserId())
                .WhereIF(input.RoleIds.HasValue, x => x.RoleIds == input.RoleIds.Value)
            .WhereIF(input.UserName.IsNotNullOrNotWhiteSpace(), x => x.UserName.Contains(input.UserName))
                 .WhereIF(input.Name.IsNotNullOrNotWhiteSpace(), x => x.Name.Contains(input.Name))
                  .WhereIF(input.Email.IsNotNullOrNotWhiteSpace(), x => x.Email.Contains(input.Email))
                        .WhereIF(input.PhoneNumber.IsNotNullOrNotWhiteSpace(), x => x.PhoneNumber.Contains(input.PhoneNumber))

                .OrderByDescending(x => x.CreationTime)
                .Select<AppUserDto>()

                .ToPageListAsync(input.Page, input.Size, totalCount);
            foreach (var item in items)
            {

            }

            return new PagedReuslt<AppUserDto>(items, totalCount.Value);
        }

        [HttpPost("SignIn")]
        public async Task<string> AppUserLoginAsync(AppUserDto input)
        {

            var list = await ManagerService.AllListAsync();

            var appUser = list.Items.FirstOrDefault(x => x.UserName == input.UserName && x.Password == input.Password);
            if (appUser is null) throw new YouJuException("账号密码不对");


            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(YouJuClaimTypes.UserId, appUser.Id.ToString()));
            claims.Add(new Claim(YouJuClaimTypes.UserName, appUser.UserName));
            claims.Add(new Claim(YouJuClaimTypes.RoleIds, ((int)appUser.RoleIds).ToString()));
            var rs = jwtHelper.GetToken(claims, 99999);
            return rs;

        }
        public override async Task<AppUserDto> CreateOrEditAsync(AppUserDto input)
        {
            if (!input.Id.HasValue)
            {
                if (input.Password.IsNullOrWhiteSpace())
                {
                    throw new YouJuException("密码不能为空");
                }

                var user = await SqlSugarClient.Queryable<AppUser>().FirstAsync(x => x.UserName == input.UserName);
                if (user is not null)
                {
                    throw new YouJuException("该用户名称已经存在");
                }
                user = await SqlSugarClient.Queryable<AppUser>().FirstAsync(x => x.PhoneNumber == input.PhoneNumber);
                if (user is not null)
                {
                    throw new YouJuException("该手机号已经存在");
                }
                user = await SqlSugarClient.Queryable<AppUser>().FirstAsync(x => x.UserName == input.UserName);
                if (user is not null)
                {
                    throw new YouJuException("该用户名已存在");
                }
            }
            else
            {
                var user = await SqlSugarClient.Queryable<AppUser>().FirstAsync(x => x.Id == input.Id);
                if (user.PhoneNumber != input.PhoneNumber)
                {
                    if (await SqlSugarClient.Queryable<AppUser>().CountAsync(x => x.Id != input.Id && x.PhoneNumber == input.PhoneNumber) > 0)
                    {
                        throw new YouJuException("该手机号已经存在");
                    }
                }
                if (user.UserName != input.UserName)
                {
                    if (await SqlSugarClient.Queryable<AppUser>().CountAsync(x => x.Id != input.Id && x.UserName == input.UserName) > 0)
                    {
                        throw new YouJuException("该用户名称已存在");
                    }
                }



            }

            return await base.CreateOrEditAsync(input);
        }

        [HttpPost("GetByToken")]

        public async Task<AppUser> GetByToken()
        {
            this.CheckAuth();

            return await ManagerService.GetAsync(new IdInput<Guid>() { Id = CurrentUser.GetUserId() });
        }

        [HttpPost("GetRole")]

        public async Task<PagedReuslt<SelectResult>> GetRole()
        {
            var roles = new List<SelectResult>();

            roles = typeof(RoleType).GetEnumList().Select(x => new SelectResult() { Name = x.Key, Value = x.Value }).ToList();
            return new PagedReuslt<SelectResult>(roles, roles.Count);

        }





        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public string GetVerificationCode()
        {
            Random random = new Random();
            int value = random.Next(100000, 999999);
            return value.ToString();
        }

        #region 用户注册流程


        /// <summary>
        /// 发送邮件
        /// </summary>
        [HttpPost("SendQQEmailAsync")]
        public async Task SendQQEmailAsync(SendQQEmailInput input)
        {


            if (Regex.IsMatch(input.Email, @"^[1-9][0-9]{4,10}@qq\.com$") == false)
            {
                throw new YouJuException("请输入正确的QQ邮箱");
            }

            var now = DateTime.Now.AddMinutes(-5);
            var count = await SqlSugarClient.Queryable<EmailValidCode>().CountAsync(x => x.ToEmail == input.Email && x.CreationTime > now);
            if (count > 3)
            {
                throw new YouJuException("请勿重复发送邮箱,等5分钟后在试");
            }
            var code = GetVerificationCode();
            var emailValidCode = new EmailValidCode()
            {

                Body = $"跟着字母哥从零快速学编程!你的注册验证码:【{code}】",
                Subject = "字母哥低代码制作平台",
                ToEmail = input.Email,
                Code = code,
                ExpireTime = DateTime.Now.AddMilliseconds(5),
                IsUse = false,
                FromEmail = _mailSettings.Mail,
            };
            try
            {
                await _mailService.SendEmailAsync(new MailRequest() { Subject = emailValidCode.Subject, Body = emailValidCode.Body, ToEmail = emailValidCode.ToEmail });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "用户注册发送验证码失败");

                throw new YouJuException("发送验证码失败,请联系字母哥");
            }
            await SqlSugarClient.Insertable(emailValidCode).ExecuteCommandAsync();


        }



        /// <summary>
        /// 随机账号
        /// </summary>
        [HttpPost("RandomUserNameAsync")]
        public async Task<string> RandomUserNameAsync()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            string userName;
            bool exists;
            do
            {
                // 生成6-15位随机长度
                int length = random.Next(6, 16);

                // 生成随机账号
                userName = new string(Enumerable.Repeat(chars, length)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                // 检查数据库中是否已存在该账号
                exists = await SqlSugarClient.Queryable<AppUser>()
                    .Where(x => x.UserName == userName)
                    .AnyAsync();

            } while (exists); // 如果存在则重新生成

            return userName;
        }
        /// <summary>
        /// 随机密码
        /// </summary>
        [HttpPost("RandomPasswordAsync")]
        public async Task<string> RandomPasswordAsync()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789?(@=!#$%&*+-_)";
            var random = new Random();
            string password;

            // 生成10-15位随机长度
            int length = random.Next(10, 16);

            // 生成随机密码
            password = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());


            return password;
        }

        /// <summary>
        /// 通用用户注册
        /// </summary>
        [HttpPost("UserCommRegisterAsync")]

        public async Task CommonUserRegisterAsync(AppUserDto input)
        {
            string ip = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
            if (input.UserName.Length < 6 || input.UserName.Length > 15)
            {
                throw new YouJuException("账号长度请输入在6到15位之间");
            }
            if (input.Password.Length < 6 || input.Password.Length > 15)
            {
                throw new YouJuException("密码长度请输入在6到15位之间");
            }
            //检查今天注册的账号数量是否超过100个
            var count = await SqlSugarClient.Queryable<AppUser>().CountAsync(x => x.CreationTime.Date == DateTime.Now.Date);
            if (count > 100)
            {
                throw new YouJuException("今天可注册的账号数量已经超过100个,请明天再试");
            }
            //判断1个ip下注册的账号是否超过3个
            var ipCount = await SqlSugarClient.Queryable<AppUser>().CountAsync(x => x.LastLoginIp == ip);
            if (ipCount > 2)
            {
                throw new YouJuException("该ip下注册的账号数量已经超过3个,请勿重复注册");
            }

            //判断输入的账号是否存在
            var emailCounts = await SqlSugarClient.Queryable<AppUser>().CountAsync(x => x.Email == input.Email);
            if (emailCounts > 0)
            {
                throw new YouJuException("该邮箱已经被注册");
            }
            var userNameCounts = await SqlSugarClient.Queryable<AppUser>().CountAsync(x => x.UserName == input.UserName);
            if (userNameCounts > 0)
            {
                throw new YouJuException("该用户名已经存在");
            }

            var createUser = new AppUser()
            {
                Name = input.Email,
                LastLoginIp = ip,
                Password = input.Password,
                UserName = input.UserName,
                RoleIds = RoleType.用户,
                ValidTime = DateTime.Now.AddDays(180),//默认180天有效

            };
            await SqlSugarClient.Insertable(createUser).ExecuteCommandAsync();

        }



        /// <summary>
        /// 用户注册
        /// </summary>
        [HttpPost("UserRegisterAsync")]

        public async Task UserRegisterAsync(AppUserDto input)
        {
            if (input.UserName.Length < 6 || input.UserName.Length > 15)
            {
                throw new YouJuException("账号长度请输入在6到15位之间");
            }
            if (input.Password.Length < 6 || input.Password.Length > 15)
            {
                throw new YouJuException("密码长度请输入在6到15位之间");
            }
            if (input.Code.IsNullOrWhiteSpace())
            {
                throw new YouJuException("邮箱验证码不能为空");
            }

            if (Regex.IsMatch(input.Email, @"^[1-9][0-9]{4,10}@qq\.com$") == false)
            {
                throw new YouJuException("请输入正确的QQ邮箱");
            }
            var now = DateTime.Now.AddMinutes(-5);
            var emailValidCode = await SqlSugarClient.Queryable<EmailValidCode>().FirstAsync(x => x.Code == input.Code && x.ToEmail == input.Email && x.CreationTime > now);
            if (emailValidCode == null)
            {
                throw new YouJuException("该邮箱验证码不存在");
            }
            if (emailValidCode.IsUse == true)
            {
                throw new YouJuException("该邮箱验证码已被使用");
            }
            //判断输入的账号是否存在
            var emailCounts = await SqlSugarClient.Queryable<AppUser>().CountAsync(x => x.Email == input.Email);
            if (emailCounts > 0)
            {
                throw new YouJuException("该邮箱已经被注册");
            }
            var userNameCounts = await SqlSugarClient.Queryable<AppUser>().CountAsync(x => x.UserName == input.UserName);
            if (userNameCounts > 0)
            {
                throw new YouJuException("该用户名已经存在");
            }
            //如果邀请码不为空则需要验证是否存在这个邀请码
            AppUser inviteUser = null;
            if (input.InviteCode.IsNotNullOrNotWhiteSpace())
            {

                inviteUser = await SqlSugarClient.Queryable<AppUser>().FirstAsync(x => x.InviteCode == input.InviteCode);
                if (inviteUser == null)
                {
                    throw new YouJuException("该邀请码不存在");
                }
            }

            emailValidCode.IsUse = true;
            await SqlSugarClient.Updateable(emailValidCode).ExecuteCommandAsync();

            var createUser = new AppUser()
            {
                Name = input.Email,
                Email = input.Email,
                Password = input.Password,
                UserName = input.UserName,
                RoleIds = RoleType.用户,
                ValidTime = DateTime.Now.AddDays(180),//默认180天有效
                InviteCode = await GetInviteCodeAsync(),

            };
            await SqlSugarClient.Insertable(createUser).ExecuteCommandAsync();


            //如果存在邀请码
            if (input.InviteCode.IsNotNullOrNotWhiteSpace())
            {

                await _inviteRecordService.RegisterInviteAsync(input.InviteCode, createUser.Id, inviteUser.Id);

            }

        }

        /// <summary>
        /// 发送忘记密码邮件
        /// </summary>
        [HttpPost("SendForgetPasswordEmailAsync")]
        public async Task SendForgetPasswordEmailAsync(SendQQEmailInput input)
        {
            if (Regex.IsMatch(input.Email, @"^[1-9][0-9]{4,10}@qq\.com$") == false)
            {
                throw new YouJuException("请输入正确的QQ邮箱");
            }
            var userCount = await SqlSugarClient.Queryable<AppUser>().CountAsync(x => x.Email == input.Email);
            if (userCount == 0)
            {
                throw new YouJuException("该邮箱没有注册");
            }
            var now = DateTime.Now.AddMinutes(-5);
            var count = await SqlSugarClient.Queryable<EmailValidCode>().CountAsync(x => x.ToEmail == input.Email && x.CreationTime < now);
            if (count > 3)
            {
                throw new YouJuException("请勿重复发送邮箱,等5分钟后在试");
            }
            var code = GetVerificationCode();
            var emailValidCode = new EmailValidCode()
            {

                Body = $"你的修改密码验证码:【{code}】",
                Subject = "字母哥低代码制作平台",
                ToEmail = input.Email,
                ExpireTime = DateTime.Now.AddMilliseconds(5),
                IsUse = false,
                Code = code,
                FromEmail = _mailSettings.Mail,
            };

            try
            {
                await _mailService.SendEmailAsync(new MailRequest() { Subject = emailValidCode.Subject, Body = emailValidCode.Body, ToEmail = emailValidCode.ToEmail });
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "用户修改密码发送验证码失败");

                throw new YouJuException("发送验证码失败,请联系字母哥");
            }
            await SqlSugarClient.Insertable(emailValidCode).ExecuteCommandAsync();

        }
        /// <summary>
        /// 验证忘记密码
        /// </summary>
        [HttpPost("ForgetPasswordValidSubmitAsync")]
        public async Task ForgetPasswordValidSubmitAsync(ForgetPasswordValidSubmitInput input)
        {
            if (Regex.IsMatch(input.Email, @"^[1-9][0-9]{4,10}@qq\.com$") == false)
            {
                throw new YouJuException("请输入正确的QQ邮箱");
            }
            var userCount = await SqlSugarClient.Queryable<AppUser>().CountAsync(x => x.Email == input.Email);
            if (userCount == 0)
            {
                throw new YouJuException("该邮箱没有注册");
            }
            var now = DateTime.Now.AddMinutes(-5);
            var emailValidCode = await SqlSugarClient.Queryable<EmailValidCode>().FirstAsync(x => x.Code == input.Code && x.ToEmail == input.Email && x.CreationTime > now);
            if (emailValidCode == null)
            {
                throw new YouJuException("该邮箱验证码不存在或者已经过期");
            }
            if (emailValidCode.IsUse == true)
            {
                throw new YouJuException("该邮箱验证码已被使用");
            }

        }
        /// <summary>
        /// 忘记密码提交
        /// </summary>
        [HttpPost("ForgetPasswordSubmitAsync")]
        public async Task ForgetPasswordSubmitAsync(ForgetPasswordSubmitInput input)
        {
            if (Regex.IsMatch(input.Email, @"^[1-9][0-9]{4,10}@qq\.com$") == false)
            {
                throw new YouJuException("请输入正确的QQ邮箱");
            }
            if (input.NewPassword.Length < 6 || input.NewPassword.Length > 15)
            {
                throw new YouJuException("密码长度请输入在6到15位之间");
            }
            if (input.Code.IsNullOrWhiteSpace())
            {
                throw new YouJuException("邮箱验证码不能为空");
            }
            var now = DateTime.Now.AddMinutes(-5);
            var emailValidCode = await SqlSugarClient.Queryable<EmailValidCode>().FirstAsync(x => x.Code == input.Code && x.ToEmail == input.Email && x.CreationTime > now);
            if (emailValidCode == null)
            {
                throw new YouJuException("该邮箱验证码不存在或者已经过期");
            }
            if (emailValidCode.IsUse == true)
            {
                throw new YouJuException("该邮箱验证码已被使用");
            }
            var appUser = await SqlSugarClient.Queryable<AppUser>().FirstAsync(x => x.Email == input.Email && x.RoleIds == RoleType.用户);
            if (appUser == null)
            {
                throw new YouJuException("该邮箱没有注册");
            }

            appUser.Password = input.NewPassword;
            await SqlSugarClient.Updateable(appUser).ExecuteCommandAsync();
        }

        /// <summary>
        /// 用户登录提交
        /// </summary>
        [HttpPost("UserLoginAsync")]
        public async Task<string> UserLoginAsync(UserLoginInput input)
        {
            var appUser = await SqlSugarClient.Queryable<AppUser>().FirstAsync(x => x.UserName == input.InputAccount && x.Password == input.Password && x.RoleIds == RoleType.用户);
            if (appUser == null)
            {
                appUser = await SqlSugarClient.Queryable<AppUser>().FirstAsync(x => x.Email == input.InputAccount && x.Password == input.Password && x.RoleIds == RoleType.用户);
            }
            if (appUser == null)
            {
                throw new YouJuException("登录失败,输入的账号或者密码有误");
            }
            if (appUser.ValidTime < DateTime.Now)
            {
                throw new YouJuException("登录失败,账号体验时间已过期,请联系字母哥");
            }
            appUser.LockoutEnd = DateTime.Now;
            await SqlSugarClient.Updateable(appUser).ExecuteCommandAsync();

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(YouJuClaimTypes.UserId, appUser.Id.ToString()));
            claims.Add(new Claim(YouJuClaimTypes.UserName, appUser.UserName));
            claims.Add(new Claim(YouJuClaimTypes.RoleIds, ((int)appUser.RoleIds).ToString()));
            var rs = jwtHelper.GetToken(claims, 60 * 24);
            return rs;
        }




        [HttpPost("UserGetByToken")]

        public async Task<Object> UserGetByToken()
        {
            this.CheckAuth();

            var rs = await SqlSugarClient.Queryable<AppUser>().FirstAsync(x => x.Id == CurrentUser.GetUserId());
            if (rs == null)
            {
                throw new YouJuException("登录失败,账号不存在");
            }

            if (rs.ValidTime < DateTime.Now)
            {
                throw new YouJuException("登录失败,账号体验时间已过期,请联系字母哥");
            }
            rs.LastLoginIp = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
            rs.LastLoginTime = DateTime.Now;

            //每次在登录的时候要判断是否存在
            if (rs.InviteCode.IsNullOrWhiteSpace())
            {
                rs.InviteCode = await GetInviteCodeAsync();
            }

            // 循环所有的DrawingType枚举，为用户创建不存在的余额
            var allDrawingTypes = Enum.GetValues(typeof(DrawingType)).Cast<DrawingType>();
            foreach (var drawingType in allDrawingTypes)
            {
                // 检查该类型的余额是否已存在
                var existingBalance = await _drawingBalanceManager.GetEffectiveBalanceAsync(rs.Id, drawingType);

                var newBalanceList = new List<DrawingBalance>();
                // 如果不存在，则创建新的余额
                if (existingBalance == null)
                {
                    var newBalance = new DrawingBalance
                    {
                        UserId = rs.Id,
                        DrawingType = drawingType,
                        EffectiveTime = DateTime.Now,
                        ExpirationTime = DateTime.Now.AddYears(1), // 有效期7天
                        MaxDailyGenerations = 100, // 每天最大生成次数
                        IsEnabled = true
                    };
                    if (drawingType == DrawingType.ER图)
                    {
                        newBalance.MaxDailyGenerations = 999;
                    }
                    //插入数据库
                    newBalanceList.Add(newBalance);
                }

                if (newBalanceList.Count > 0)
                {
                    await SqlSugarClient.Insertable(newBalanceList).ExecuteCommandAsync();
                }
            }

            await SqlSugarClient.Updateable(rs).ExecuteCommandAsync();

            return new
            {
                Id = rs.Id,
                UserName = rs.UserName,
                Email = rs.Email,
                Name = rs.Name,
                RoleIds = rs.RoleIds,
                InviteCode = rs.InviteCode,

            };
        }


        [NonAction]
        private async Task<string> GetInviteCodeAsync()
        {
            //自动生成1个大写字母4位数的 并且数据库不存在的
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var random = new Random();
            string inviteCode;
            bool exists;
            do
            {
                // 生成随机账号
                inviteCode = new string(Enumerable.Repeat(chars, 4)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                // 检查数据库中是否已存在该账号
                exists = await SqlSugarClient.Queryable<AppUser>()
                    .Where(x => x.InviteCode == inviteCode)
                    .AnyAsync();

            } while (exists); // 如果存在则重新生成
            return inviteCode;
        }
        #endregion





    }






}
