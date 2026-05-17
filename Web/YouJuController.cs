

using Web.Extensions;

namespace YouJu.AspNetCore.Controllers
{
    /// <summary>
    /// 控制器基类
    /// </summary>
    public class YouJuController<TEntity, TDto, TSearch> : ControllerBase
         where TEntity : CreationAuditedAggregateRoot, new()
         where TDto : FullBaseDto, new()
        where TSearch : PagedBaseInput, new()
    {

        protected IServiceProvider ServiceProvider;

        protected ISqlSugarClient SqlSugarClient;

        protected ManagerService<TEntity, TDto> ManagerService;


        [NonAction]
        public void CheckAuth()
        {
            if (CurrentUser.GetUserId() == Guid.Empty)
            {
                throw new UnAuthenticationException();
            }

        }

        public YouJuController(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            SqlSugarClient = ServiceProvider.GetRequiredService<ISqlSugarClient>();
            ManagerService = ServiceProvider.GetRequiredService<ManagerService<TEntity, TDto>>();
        }
        public ICurrentUser CurrentUser => ServiceProvider.GetRequiredService<ICurrentUser>();

        /// <summary>
        /// 公共查询方法
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("ListAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public virtual async Task<PagedReuslt<TDto>> ListAsync(TSearch input)
        {

            if (input.IsAuth && CurrentUser.GetUserId() == Guid.Empty)
            {
                throw new UnAuthenticationException();
            }

            RefAsync<int> totalCount = 0;
            var items = await SqlSugarClient.Queryable<TEntity>()
                .WhereIF(input.IsAuth, x => x.CreatorId == CurrentUser.GetUserId())
                .Select<TDto>()
                .ToPageListAsync(input.Page, input.Size, totalCount);
            return new PagedReuslt<TDto>(items, totalCount.Value);
        }



        /// <summary>
        /// 公共删除方法
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("DeleteAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public virtual async Task DeleteAsync(IdInput<Guid> input)
        {
            await SqlSugarClient.Updateable<TEntity>().Where(it => it.Id == input.Id).SetColumns(x=>x.IsDeleted==true).ExecuteCommandAsync();

        }
        /// <summary>
        /// 公共获取单个对象
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("GetAsync")]
        public virtual async Task<TDto> GetAsync(IdInput<Guid> input)
        {

            var dto = (await ListAsync(new TSearch())).Items.Where(x => x.Id == input.Id).FirstOrDefault();
            return dto ?? Activator.CreateInstance<TDto>();
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("CreateOrEditAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public virtual async Task<TDto> CreateOrEditAsync(TDto input)
        {
            input.GuidNullToEmpty();
            var entity = await SqlSugarClient.Queryable<TEntity>().FirstAsync(x => x.Id == input.Id);
            if (entity is null)
            {
                input.Id = Guid.Empty;
                entity = input.Clone<TDto, TEntity>();

              
                entity = await SqlSugarClient.Insertable(entity).ExecuteReturnEntityAsync();
            }
            else
            {

                entity = input.Clone<TDto, TEntity>();
               
                await SqlSugarClient.Updateable(entity).ExecuteCommandAsync();
            }
            return entity.Clone<TEntity, TDto>();
        }






    }
}
