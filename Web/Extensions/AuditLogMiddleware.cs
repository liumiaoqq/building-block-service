using System.Diagnostics;
using YouJu.Infrastructure;

namespace Web.Extensions
{
    public class AuditLogMiddleware
    {
        private static readonly HashSet<string> IgnoredPathPrefixes = new(StringComparer.OrdinalIgnoreCase)
        {
            "/AuditLog",
            "/SystemConfig",
            "/SystemInfoSetting"
        };

        private readonly RequestDelegate _next;

        public AuditLogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            Exception exception = null;

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                exception = ex;
                throw;
            }
            finally
            {
                stopwatch.Stop();
                await TryWriteAuditLogAsync(context, stopwatch.ElapsedMilliseconds, exception);
            }
        }

        private static async Task TryWriteAuditLogAsync(HttpContext context, long elapsedMilliseconds, Exception exception)
        {
            if (ShouldIgnore(context))
            {
                return;
            }

            try
            {
                using var scope = context.RequestServices.CreateScope();
                var sqlSugarClient = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();
                var auditEnabled = await sqlSugarClient.Queryable<SystemConfig>()
                    .Where(x => x.IsAuditEnabled)
                    .AnyAsync();
                if (!auditEnabled)
                {
                    return;
                }

                var currentUser = scope.ServiceProvider.GetRequiredService<ICurrentUser>();
                var userId = currentUser.GetUserId();
                var userName = currentUser.GetCliam(YouJuClaimTypes.UserName)?.Value;
                var roleName = currentUser.GetRoleType().ToDescription();
                var statusCode = context.Response?.StatusCode ?? 0;

                await sqlSugarClient.Insertable(new AuditLog
                {
                    UserId = userId == Guid.Empty ? null : userId,
                    UserName = userName,
                    RoleName = roleName,
                    HttpMethod = context.Request.Method,
                    Path = context.Request.Path.Value,
                    QueryString = context.Request.QueryString.Value,
                    ClientIp = GetClientIp(context),
                    UserAgent = context.Request.Headers["User-Agent"].ToString(),
                    StatusCode = statusCode,
                    ElapsedMilliseconds = elapsedMilliseconds,
                    IsSuccess = exception == null && statusCode < 400,
                    ErrorMessage = exception?.Message
                }).ExecuteCommandAsync();
            }
            catch
            {
                // 审计日志不能影响正常业务请求。
            }
        }

        private static bool ShouldIgnore(HttpContext context)
        {
            var path = context.Request.Path.Value ?? string.Empty;
            if (string.IsNullOrWhiteSpace(path))
            {
                return true;
            }

            if (path.Contains(".", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return IgnoredPathPrefixes.Any(prefix => path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        }

        private static string GetClientIp(HttpContext context)
        {
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (forwardedFor.IsNotNullOrNotWhiteSpace())
            {
                return forwardedFor.Split(',').FirstOrDefault()?.Trim();
            }

            return context.Connection.RemoteIpAddress?.ToString();
        }
    }
}
