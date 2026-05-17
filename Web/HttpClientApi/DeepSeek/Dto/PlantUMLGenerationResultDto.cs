namespace Web.HttpClientApi.DeepSeek.Dto
{
    /// <summary>
    /// PlantUML生成结果DTO
    /// </summary>
    public class PlantUMLGenerationResultDto
    {
        /// <summary>
        /// 生成的PlantUML代码
        /// </summary>
        public string PlantUMLCode { get; set; }

        /// <summary>
        /// 分析说明
        /// </summary>
        public string Description { get; set; }
    }
}

