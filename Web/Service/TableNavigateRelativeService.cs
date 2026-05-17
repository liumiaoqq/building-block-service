using Web.Manager;

namespace Web.Service
{
    public class TableNavigateRelativeService
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        private readonly ICurrentUser _currentUser;

        private readonly TableNavigateRelativeManager _tableNavigateRelativeManager;

        public TableNavigateRelativeService(ISqlSugarClient sqlSugarClient, ICurrentUser currentUser, TableNavigateRelativeManager tableNavigateRelativeManager)
        {
            _sqlSugarClient = sqlSugarClient;
            _currentUser = currentUser;
            _tableNavigateRelativeManager = tableNavigateRelativeManager;
        }

        public async Task<PagedReuslt<TableNavigateRelativeDto>> ListAsync(TableNavigateRelativePagedInput input)
        {

            if (_currentUser.GetRoleType() == RoleType.用户)
            {
                var userId = _currentUser.GetUserId();
                input.UserId = userId;
                if (input.RelativeTableId.HasValue == false)
                {
                    throw new YouJuException("RelativeTableId丢失,请联系字母哥");
                }
            }
            return await _tableNavigateRelativeManager.ListAsync(input);
        }

        public async Task<TableNavigateRelativeDto> GetAsync(IdInput<Guid> input)
        {
            return await _tableNavigateRelativeManager.GetAsync(input);
        }

        public async Task DeleteAsync(IdInput<Guid> input)
        {

            if (_currentUser.IsUser())
            {
                var userId = _currentUser.GetUserId();
                var count = await _sqlSugarClient.Queryable<TableNavigateRelative>().CountAsync(x => x.Id == input.Id && x.CreatorId == userId);
                if (count == 0)
                {
                    throw new YouJuException("没有找到可删除的数据,请刷新页面");
                }
            }
            await _tableNavigateRelativeManager.DeleteAsync(input);
        }

    }

}
