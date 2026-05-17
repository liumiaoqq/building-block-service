using Web.Extensions;
using Web.Service;
using Web.Tables;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArticleCategoryController : ControllerBase
    {
        private readonly ArticleCategoryService _articleCategoryService;

        public ArticleCategoryController(ArticleCategoryService articleCategoryService)
        {
            _articleCategoryService = articleCategoryService;
        }

        /// <summary>
        /// 获取文章分类列表（管理端）
        /// </summary>
        [HttpPost("ListAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async Task<PagedReuslt<ArticleCategoryDto>> ListAsync(ArticleCategoryPagedInput input)
        {
            return await _articleCategoryService.ListAsync(input);
        }

        /// <summary>
        /// 获取所有可见的分类列表（前台使用）
        /// </summary>
        [HttpPost("GetAllVisibleAsync")]
        public async Task<List<ArticleCategoryDto>> GetAllVisibleAsync()
        {
            return await _articleCategoryService.GetAllVisibleAsync();
        }

        /// <summary>
        /// 获取单个对象
        /// </summary>
        [HttpPost("GetAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async Task<ArticleCategoryDto> GetAsync(IdInput<Guid> input)
        {
            return await _articleCategoryService.GetAsync(input.Id);
        }

        /// <summary>
        /// 创建或编辑文章分类
        /// </summary>
        [HttpPost("CreateOrEditAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async Task<ArticleCategoryDto> CreateOrEditAsync(ArticleCategoryDto input)
        {
            return await _articleCategoryService.CreateOrEditAsync(input);
        }

        /// <summary>
        /// 删除文章分类
        /// </summary>
        [HttpPost("DeleteAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async Task DeleteAsync(IdInput<Guid> input)
        {
            await _articleCategoryService.DeleteAsync(input.Id);
        }
    }
}
