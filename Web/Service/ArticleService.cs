using Web.Manager;

namespace Web.Service
{
    public class ArticleService
    {
        private readonly ArticleManager _articleManager;

        public ArticleService(ArticleManager articleManager)
        {
            _articleManager = articleManager;
        }

        /// <summary>
        /// 获取文章列表
        /// </summary>
        public async Task<PagedReuslt<ArticleDto>> ListAsync(ArticlePagedInput input)
        {
            return await _articleManager.ListAsync(input);
        }

        /// <summary>
        /// 获取前台显示的文章列表
        /// </summary>
        public async Task<PagedReuslt<ArticleDto>> GetVisibleListAsync(ArticlePagedInput input)
        {
            return await _articleManager.GetVisibleListAsync(input);
        }

        /// <summary>
        /// 获取单个对象（管理端使用）
        /// </summary>
        public async Task<ArticleDto> GetAsync(Guid id)
        {
            return await _articleManager.GetDtoByIdAsync(id);
        }

        /// <summary>
        /// 获取文章详情（会增加浏览次数）
        /// </summary>
        public async Task<ArticleDto> GetDetailAsync(Guid id)
        {
            // 增加浏览次数
            await _articleManager.IncrementViewCountAsync(id);

            return await _articleManager.GetDtoByIdAsync(id);
        }

        /// <summary>
        /// 创建或编辑文章
        /// </summary>
        public async Task<ArticleDto> CreateOrEditAsync(ArticleDto input)
        {
            return await _articleManager.CreateOrEditAsync(input);
        }

        /// <summary>
        /// 删除文章
        /// </summary>
        public async Task DeleteAsync(Guid id)
        {
            await _articleManager.DeleteAsync(id);
        }
    }
}
