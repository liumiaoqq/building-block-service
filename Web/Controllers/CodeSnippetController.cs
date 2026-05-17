using Web.Extensions;
using Web.Service;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CodeSnippetController : ControllerBase
    {
        private readonly CodeSnippetService _codeSnippetService;

        public CodeSnippetController(CodeSnippetService codeSnippetService)
        {
            _codeSnippetService = codeSnippetService;
        }

        /// <summary>
        /// 获取代码片段树形结构
        /// </summary>
        [HttpPost("GetTreeAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<CodeSnippetTreeOutput> GetTreeAsync(IdInput<Guid> input)
        {
            return await _codeSnippetService.GetTreeAsync(input.Id);
        }

        /// <summary>
        /// 根据ID获取代码片段
        /// </summary>
        [HttpPost("GetByIdAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<CodeSnippetDto> GetByIdAsync(IdInput<Guid> input)
        {
            return await _codeSnippetService.GetByIdAsync(input.Id);
        }

        /// <summary>
        /// 根据ID获取代码片段内容
        /// </summary>
        [HttpPost("GetFileContentAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<string> GetFileContentAsync(IdInput<Guid> input)
        {
            return await _codeSnippetService.GetFileContentAsync(input.Id);
        }

        /// <summary>
        /// 批量创建代码片段
        /// </summary>
        [HttpPost("BatchCreateAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<int> BatchCreateAsync(BatchCreateCodeSnippetsInput input)
        {
            return await _codeSnippetService.BatchCreateAsync(input);
        }

        /// <summary>
        /// 更新代码片段
        /// </summary>
        [HttpPost("UpdateAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task UpdateAsync(UpdateCodeSnippetInput input)
        {
            await _codeSnippetService.UpdateAsync(input);
        }

        /// <summary>
        /// 删除代码片段
        /// </summary>
        [HttpPost("DeleteAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task DeleteAsync(IdInput<Guid> input)
        {
            await _codeSnippetService.DeleteAsync(input.Id);
        }

        /// <summary>
        /// 复制到新项目
        /// </summary>
        [HttpPost("CopyToNewProjectAsync")]
        [CustomAuthorization(RoleType.用户)]
        public async Task<Guid> CopyToNewProjectAsync(CopyToNewProjectInput input)
        {
            return await _codeSnippetService.CopyToNewProjectAsync(input);
        }
    }
}

