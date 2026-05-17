using Web.Manager;

namespace Web.Service
{
    public class SqlTempleteService
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        private readonly ICurrentUser _currentUser;

        private readonly SqlTempleteManager _sqlTempleteManager;

        public SqlTempleteService(ISqlSugarClient sqlSugarClient, ICurrentUser currentUser, SqlTempleteManager sqlTempleteManager)
        {
            _sqlSugarClient = sqlSugarClient;
            _currentUser = currentUser;
            _sqlTempleteManager = sqlTempleteManager;
        }

        public async Task<PagedReuslt<SqlTempleteDto>> ListAsync(SqlTempletePagedInput input)
        {

            return await _sqlTempleteManager.ListAsync(input);
        }


    }

}
