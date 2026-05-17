namespace Web.HttpClientApi.DeepSeek.Dto
{
    /// <summary>
    /// 时序图生成结果DTO
    /// </summary>
    public class SequenceDiagramGenerationResultDto
    {
        /// <summary>
        /// 生成的PlantUML时序图代码
        /// </summary>
        public string PlantUMLCode { get; set; }

        /// <summary>
        /// 分析说明
        /// </summary>
        public string Description { get; set; }
    }
}

