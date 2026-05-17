using Microsoft.AspNetCore.Mvc;
using Web.HttpClient;
using Web.HttpClientApi.DeepSeek.Dto;
using Web.HttpClientApi.DeepSeek.Service;

namespace Web.Controllers
{
    /// <summary>
    /// 项目结构生成控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectStructureController : ControllerBase
    {
        private readonly IDeepSeekService _deepSeekService;
        private readonly ILogger<ProjectStructureController> _logger;

        public ProjectStructureController(
            IDeepSeekService deepSeekService,
            ILogger<ProjectStructureController> logger)
        {
            _deepSeekService = deepSeekService;
            _logger = logger;
        }

        /// <summary>
        /// 根据用户输入的需求描述生成项目结构树
        /// </summary>
        /// <param name="request">需求描述</param>
        /// <returns>项目结构树</returns>
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateProjectStructure([FromBody] GenerateProjectStructureRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Description))
                {
                    return BadRequest(new { message = "需求描述不能为空" });
                }

                _logger.LogInformation($"开始生成项目结构，需求描述：{request.Description}");

                // 获取系统提示
                var systemPrompt = _deepSeekService.GetSystemPrompt(DeepSeekSystemPrompt.ProjectStructureGeneration);

                // 调用AI服务生成项目结构
                var result = await _deepSeekService.ChatCompletionAsync<ProjectStructureGenerationResultDto>(
                    systemPrompt,
                    request.Description,
                    4000); // 增加token限制以支持更复杂的结构

                if (result == null || result.ProjectTree == null)
                {
                    return BadRequest(new { message = "生成项目结构失败，请重试" });
                }

                _logger.LogInformation($"项目结构生成成功，根节点：{result.ProjectTree.Name}");

                return Ok(new
                {
                    success = true,
                    data = result.ProjectTree,
                    message = "项目结构生成成功"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "生成项目结构时发生错误");
                return StatusCode(500, new
                {
                    success = false,
                    message = $"生成项目结构时发生错误: {ex.Message}"
                });
            }
        }
    }

    /// <summary>
    /// 生成项目结构请求DTO
    /// </summary>
    public class GenerateProjectStructureRequest
    {
        /// <summary>
        /// 需求描述
        /// </summary>
        public string Description { get; set; }
    }
}

