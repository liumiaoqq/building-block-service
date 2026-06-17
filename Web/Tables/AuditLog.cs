namespace Web.Tables
{
    /// <summary>
    /// 审计日志
    /// </summary>
    [YoungTable("AuditLog")]
    public class AuditLog : CreationAuditedAggregateRoot
    {
        [Description("用户Id")]
        [SugarColumn(IsNullable = true)]
        public Guid? UserId { get; set; }

        [Description("用户名")]
        public string UserName { get; set; }

        [Description("角色")]
        public string RoleName { get; set; }

        [Description("请求方法")]
        public string HttpMethod { get; set; }

        [Description("请求路径")]
        public string Path { get; set; }

        [Description("查询参数")]
        public string QueryString { get; set; }

        [Description("客户端IP")]
        public string ClientIp { get; set; }

        [Description("UserAgent")]
        public string UserAgent { get; set; }

        [Description("状态码")]
        public int StatusCode { get; set; }

        [Description("耗时毫秒")]
        public long ElapsedMilliseconds { get; set; }

        [Description("是否成功")]
        public bool IsSuccess { get; set; }

        [Description("错误信息")]
        public string ErrorMessage { get; set; }
    }
}
