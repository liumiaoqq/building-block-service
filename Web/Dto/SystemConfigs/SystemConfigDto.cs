namespace Web.Dto.SystemConfigs
{
    public class SystemConfigDto : FullBaseDto
    {
        public bool IsAuditEnabled { get; set; }
    }

    public class SystemConfigPagedInput : PagedBaseInput
    {
    }
}
