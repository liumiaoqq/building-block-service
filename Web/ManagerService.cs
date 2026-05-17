using Microsoft.AspNetCore.Mvc;

namespace Web
{
    public class ManagerService<TEntity, TDto>
        where TEntity : CreationAuditedAggregateRoot, new()
         where TDto : FullBaseDto, new()
    {
        protected IServiceProvider ServiceProvider;

        protected ISqlSugarClient SqlSugarClient;



        public ManagerService(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            SqlSugarClient = ServiceProvider.GetRequiredService<ISqlSugarClient>();
        }
        public ICurrentUser CurrentUser => ServiceProvider.GetRequiredService<ICurrentUser>();


        public async Task<PagedReuslt<TDto>> AllListAsync()
        {

            RefAsync<int> totalCount = 0;
            var items = await SqlSugarClient.Queryable<TEntity>()
                .ToListAsync();
            return new PagedReuslt<TDto>(items.Clone<List<TEntity>, List<TDto>>(), items.Count);
        }

        /// <summary>
        /// 公共删除方法
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>

        public async Task<PagedReuslt<TDto>> ListAsync(PagedBaseInput input)
        {
       
            RefAsync<int> totalCount = 0;
            var items = await SqlSugarClient.Queryable<TEntity>()
                .ToPageListAsync(input.Page, input.Size, totalCount);
            return new PagedReuslt<TDto>(items.Clone<List<TEntity>, List<TDto>>(), totalCount.Value);
        }


        /// <summary>
        /// 公共删除方法
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
     
        public virtual async Task DeleteAsync(IdInput<Guid> input)
        {


            await SqlSugarClient.Deleteable<TEntity>().Where(it => it.Id == input.Id).ExecuteCommandAsync();

        }
        /// <summary>
        /// 公共获取单个对象
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
  
        public virtual async Task<TEntity> GetAsync(IdInput<Guid> input)
        {

            var entity = await SqlSugarClient.Queryable<TEntity>().FirstAsync(x => x.Id == input.Id);
            return entity ?? Activator.CreateInstance<TEntity>();
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>

        public virtual async Task<TDto> CreateOrEditAsync(TDto input)
        {
            var entity = await SqlSugarClient.Queryable<TEntity>().FirstAsync(x => x.Id == input.Id);
            if (input.Id == Guid.Empty)
            {

                entity = await SqlSugarClient.Insertable(input.Clone<TDto, TEntity>()).ExecuteReturnEntityAsync();
            }
            else
            {
                await SqlSugarClient.Updateable(entity).ExecuteCommandAsync();
            }
            return entity.Clone<TEntity, TDto>();
        }
    }
}
