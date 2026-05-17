namespace Web.Manager
{
    public class ArticleCategoryManager
    {
        protected ISqlSugarClient _sqlSugarClient;

        public ArticleCategoryManager(ISqlSugarClient sqlSugarClient)
        {
            _sqlSugarClient = sqlSugarClient;
        }

        /// <summary>
        /// 获取文章分类列表
        /// </summary>
        public async Task<PagedReuslt<ArticleCategoryDto>> ListAsync(ArticleCategoryPagedInput input)
        {
            RefAsync<int> totalCount = 0;
            var result = await _sqlSugarClient.Queryable<ArticleCategory>()
                .Where(x => x.IsDeleted == false)
                .WhereIF(!string.IsNullOrEmpty(input.Name), x => x.Name.Contains(input.Name))
                .WhereIF(input.IsVisible != null, x => x.IsVisible == input.IsVisible)
                .OrderBy(x => x.Sort)
                .OrderByDescending(x => x.CreationTime)
                .Select(x => new ArticleCategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Sort = x.Sort,
                    IsVisible = x.IsVisible,
                    CreationTime = x.CreationTime
                })
                .ToPageListAsync(input.Page, input.Size, totalCount);

            return new PagedReuslt<ArticleCategoryDto>(result, totalCount);
        }

        /// <summary>
        /// 获取所有可见的分类列表
        /// </summary>
        public async Task<List<ArticleCategoryDto>> GetAllVisibleAsync()
        {
            return await _sqlSugarClient.Queryable<ArticleCategory>()
                .Where(x => x.IsDeleted == false)
                .Where(x => x.IsVisible == true)
                .OrderBy(x => x.Sort)
                .Select(x => new ArticleCategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Sort = x.Sort,
                    IsVisible = x.IsVisible,
                    CreationTime = x.CreationTime
                })
                .ToListAsync();
        }

        /// <summary>
        /// 根据Id获取实体
        /// </summary>
        public async Task<ArticleCategory> GetByIdAsync(Guid id)
        {
            return await _sqlSugarClient.Queryable<ArticleCategory>()
                .FirstAsync(x => x.Id == id);
        }

        /// <summary>
        /// 根据Id获取DTO
        /// </summary>
        public async Task<ArticleCategoryDto> GetDtoByIdAsync(Guid id)
        {
            return await _sqlSugarClient.Queryable<ArticleCategory>()
                .Where(x => x.Id == id)
                .Select(x => new ArticleCategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Sort = x.Sort,
                    IsVisible = x.IsVisible,
                    CreationTime = x.CreationTime
                })
                .FirstAsync();
        }

        /// <summary>
        /// 创建或编辑文章分类
        /// </summary>
        public async Task<ArticleCategoryDto> CreateOrEditAsync(ArticleCategoryDto input)
        {
            var entity = await _sqlSugarClient.Queryable<ArticleCategory>().FirstAsync(x => x.Id == input.Id);
            if (entity == null)
            {
                // 新增
                entity = new ArticleCategory
                {
                    Id = Guid.NewGuid(),
                    Name = input.Name,
                    Description = input.Description,
                    Sort = input.Sort,
                    IsVisible = input.IsVisible,
                    CreationTime = DateTime.Now,
                    IsDeleted = false
                };
                entity = await _sqlSugarClient.Insertable(entity).ExecuteReturnEntityAsync();
            }
            else
            {
                // 编辑
                entity.Name = input.Name;
                entity.Description = input.Description;
                entity.Sort = input.Sort;
                entity.IsVisible = input.IsVisible;
                await _sqlSugarClient.Updateable(entity).ExecuteCommandAsync();
            }

            return await GetDtoByIdAsync(entity.Id);
        }

        /// <summary>
        /// 删除文章分类（软删除）
        /// </summary>
        public async Task DeleteAsync(Guid id)
        {
            await _sqlSugarClient.Updateable<ArticleCategory>()
                .Where(x => x.Id == id)
                .SetColumns(x => x.IsDeleted == true)
                .ExecuteCommandAsync();
        }
    }
}
