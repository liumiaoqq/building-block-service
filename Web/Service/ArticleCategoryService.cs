using Web.Manager;

namespace Web.Service
{
    public class ArticleCategoryService
    {
        private readonly ArticleCategoryManager _articleCategoryManager;

        public ArticleCategoryService(ArticleCategoryManager articleCategoryManager)
        {
            _articleCategoryManager = articleCategoryManager;
        }

        /// <summary>
        /// 获取文章分类列表
        /// </summary>
        public async Task<PagedReuslt<ArticleCategoryDto>> ListAsync(ArticleCategoryPagedInput input)
        {
            return await _articleCategoryManager.ListAsync(input);
        }

        /// <summary>
        /// 获取所有可见的分类列表（前台使用）
        /// </summary>
        public async Task<List<ArticleCategoryDto>> GetAllVisibleAsync()
        {
            return await _articleCategoryManager.GetAllVisibleAsync();
        }

        /// <summary>
        /// 获取单个对象
        /// </summary>
        public async Task<ArticleCategoryDto> GetAsync(Guid id)
        {
            return await _articleCategoryManager.GetDtoByIdAsync(id);
        }

        /// <summary>
        /// 创建或编辑文章分类
        /// </summary>
        public async Task<ArticleCategoryDto> CreateOrEditAsync(ArticleCategoryDto input)
        {
            return await _articleCategoryManager.CreateOrEditAsync(input);
        }

        /// <summary>
        /// 删除文章分类
        /// </summary>
        public async Task DeleteAsync(Guid id)
        {
            await _articleCategoryManager.DeleteAsync(id);
        }
    }
}
