namespace Web.Manager
{
    public class ArticleManager
    {
        protected ISqlSugarClient _sqlSugarClient;

        public ArticleManager(ISqlSugarClient sqlSugarClient)
        {
            _sqlSugarClient = sqlSugarClient;
        }

        /// <summary>
        /// 获取文章列表
        /// </summary>
        public async Task<PagedReuslt<ArticleDto>> ListAsync(ArticlePagedInput input)
        {
            RefAsync<int> totalCount = 0;
            var result = await _sqlSugarClient.Queryable<Article>()
                .LeftJoin<ArticleCategory>((a, c) => a.CategoryId == c.Id)
                .Where((a, c) => a.IsDeleted == false)
                .WhereIF(!string.IsNullOrEmpty(input.Title), (a, c) => a.Title.Contains(input.Title))
                .WhereIF(input.CategoryId != null, (a, c) => a.CategoryId == input.CategoryId)
                .WhereIF(input.IsVisible != null, (a, c) => a.IsVisible == input.IsVisible)
                .OrderBy((a, c) => a.Sort)
                .OrderByDescending((a, c) => a.CreationTime)
                .Select((a, c) => new ArticleDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Content = a.Content,
                    Summary = a.Summary,
                    CoverImage = a.CoverImage,
                    CategoryId = a.CategoryId,
                    CategoryName = c.Name,
                    Sort = a.Sort,
                    IsVisible = a.IsVisible,
                    ViewCount = a.ViewCount,
                    CreationTime = a.CreationTime
                })
                .ToPageListAsync(input.Page, input.Size, totalCount);

            return new PagedReuslt<ArticleDto>(result, totalCount);
        }

        /// <summary>
        /// 获取前台显示的文章列表
        /// </summary>
        public async Task<PagedReuslt<ArticleDto>> GetVisibleListAsync(ArticlePagedInput input)
        {
            RefAsync<int> totalCount = 0;
            var result = await _sqlSugarClient.Queryable<Article>()
                .LeftJoin<ArticleCategory>((a, c) => a.CategoryId == c.Id)
                .Where((a, c) => a.IsDeleted == false)
                .Where((a, c) => a.IsVisible == true)
                .WhereIF(!string.IsNullOrEmpty(input.Title), (a, c) => a.Title.Contains(input.Title))
                .WhereIF(input.CategoryId != null, (a, c) => a.CategoryId == input.CategoryId)
                .OrderBy((a, c) => a.Sort)
                .OrderByDescending((a, c) => a.CreationTime)
                .Select((a, c) => new ArticleDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Summary = a.Summary,
                    CoverImage = a.CoverImage,
                    CategoryId = a.CategoryId,
                    CategoryName = c.Name,
                    Sort = a.Sort,
                    IsVisible = a.IsVisible,
                    ViewCount = a.ViewCount,
                    CreationTime = a.CreationTime
                })
                .ToPageListAsync(input.Page, input.Size, totalCount);

            return new PagedReuslt<ArticleDto>(result, totalCount);
        }

        /// <summary>
        /// 根据Id获取实体
        /// </summary>
        public async Task<Article> GetByIdAsync(Guid id)
        {
            return await _sqlSugarClient.Queryable<Article>()
                .FirstAsync(x => x.Id == id);
        }

        /// <summary>
        /// 根据Id获取DTO
        /// </summary>
        public async Task<ArticleDto> GetDtoByIdAsync(Guid id)
        {
            return await _sqlSugarClient.Queryable<Article>()
                .LeftJoin<ArticleCategory>((a, c) => a.CategoryId == c.Id)
                .Where((a, c) => a.Id == id)
                .Select((a, c) => new ArticleDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Content = a.Content,
                    Summary = a.Summary,
                    CoverImage = a.CoverImage,
                    CategoryId = a.CategoryId,
                    CategoryName = c.Name,
                    Sort = a.Sort,
                    IsVisible = a.IsVisible,
                    ViewCount = a.ViewCount,
                    CreationTime = a.CreationTime
                })
                .FirstAsync();
        }

        /// <summary>
        /// 创建或编辑文章
        /// </summary>
        public async Task<ArticleDto> CreateOrEditAsync(ArticleDto input)
        {
            var entity = await _sqlSugarClient.Queryable<Article>().FirstAsync(x => x.Id == input.Id);
            if (entity == null)
            {
                // 新增
                entity = new Article
                {
                    Id = Guid.NewGuid(),
                    Title = input.Title,
                    Content = input.Content,
                    Summary = input.Summary,
                    CoverImage = input.CoverImage,
                    CategoryId = input.CategoryId,
                    Sort = input.Sort,
                    IsVisible = input.IsVisible,
                    ViewCount = 0,
                    CreationTime = DateTime.Now,
                    IsDeleted = false
                };
                entity = await _sqlSugarClient.Insertable(entity).ExecuteReturnEntityAsync();
            }
            else
            {
                // 编辑
                entity.Title = input.Title;
                entity.Content = input.Content;
                entity.Summary = input.Summary;
                entity.CoverImage = input.CoverImage;
                entity.CategoryId = input.CategoryId;
                entity.Sort = input.Sort;
                entity.IsVisible = input.IsVisible;
                await _sqlSugarClient.Updateable(entity).ExecuteCommandAsync();
            }

            return await GetDtoByIdAsync(entity.Id);
        }

        /// <summary>
        /// 删除文章（软删除）
        /// </summary>
        public async Task DeleteAsync(Guid id)
        {
            await _sqlSugarClient.Updateable<Article>()
                .Where(x => x.Id == id)
                .SetColumns(x => x.IsDeleted == true)
                .ExecuteCommandAsync();
        }

        /// <summary>
        /// 增加浏览次数
        /// </summary>
        public async Task IncrementViewCountAsync(Guid id)
        {
            await _sqlSugarClient.Updateable<Article>()
                .SetColumns(x => x.ViewCount == x.ViewCount + 1)
                .Where(x => x.Id == id)
                .ExecuteCommandAsync();
        }
    }
}
