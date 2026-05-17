using Web.Extensions;
using Web.Service;
using Web.Tables;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArticleController : ControllerBase
    {
        private readonly ArticleService _articleService;

        public ArticleController(ArticleService articleService)
        {
            _articleService = articleService;
        }

        /// <summary>
        /// 获取文章列表（管理端）
        /// </summary>
        [HttpPost("ListAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async Task<PagedReuslt<ArticleDto>> ListAsync(ArticlePagedInput input)
        {
            return await _articleService.ListAsync(input);
        }

        /// <summary>
        /// 获取前台显示的文章列表
        /// </summary>
        [HttpPost("GetVisibleListAsync")]
        public async Task<PagedReuslt<ArticleDto>> GetVisibleListAsync(ArticlePagedInput input)
        {
            return await _articleService.GetVisibleListAsync(input);
        }

        /// <summary>
        /// 获取单个对象（管理端使用）
        /// </summary>
        [HttpPost("GetAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async Task<ArticleDto> GetAsync(IdInput<Guid> input)
        {
            return await _articleService.GetAsync(input.Id);
        }

        /// <summary>
        /// 获取文章详情（会增加浏览次数，前台使用）
        /// </summary>
        [HttpPost("GetDetailAsync")]
        public async Task<ArticleDto> GetDetailAsync(IdInput<Guid> input)
        {
            return await _articleService.GetDetailAsync(input.Id);
        }

        /// <summary>
        /// 创建或编辑文章
        /// </summary>
        [HttpPost("CreateOrEditAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async Task<ArticleDto> CreateOrEditAsync(ArticleDto input)
        {
            return await _articleService.CreateOrEditAsync(input);
        }

        /// <summary>
        /// 删除文章
        /// </summary>
        [HttpPost("DeleteAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async Task DeleteAsync(IdInput<Guid> input)
        {
            await _articleService.DeleteAsync(input.Id);
        }
    }
}
