namespace Web.Dto.Components
{
    public class SystemComponentSettingDetDto:FullBaseDto
    {

        
        [Description("键")]
        public string Key { get; set; }

        [Description("值")]
        public string Value { get; set; }

        [Description("唯一码")]
        public string Code { get; set; }
        public Guid SystemComponentId { get; set; }
    }
}
