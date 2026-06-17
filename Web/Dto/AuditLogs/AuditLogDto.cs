namespace Web.Dto.AuditLogs
{
    public class AuditLogDto : FullBaseDto
    {
        public Guid? UserId { get; set; }

        public string UserName { get; set; }

        public string RoleName { get; set; }

        public string HttpMethod { get; set; }

        public string Path { get; set; }

        public string QueryString { get; set; }

        public string ClientIp { get; set; }

        public string UserAgent { get; set; }

        public int StatusCode { get; set; }

        public long ElapsedMilliseconds { get; set; }

        public bool IsSuccess { get; set; }

        public string ErrorMessage { get; set; }
    }

    public class AuditLogPagedInput : PagedBaseInput
    {
        public string UserName { get; set; }

        public string Path { get; set; }

        public string HttpMethod { get; set; }

        public string ClientIp { get; set; }

        public bool? IsSuccess { get; set; }
    }
}
