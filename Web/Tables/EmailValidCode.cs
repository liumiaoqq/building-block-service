namespace Web.Tables
{
    [Description("邮箱验证码")]
    [YoungTable("EmailValidCode")]
    public class EmailValidCode : CreationAuditedAggregateRoot
    {

        public string FromEmail { get; set; }


        public string ToEmail { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public string Code { get; set; }

        /// <summary>
        /// 失效时间
        /// </summary>
        public DateTime ExpireTime  { get; set; }


        public bool IsUse { get; set; }

    }
}
