using System.Text;

namespace Web.Manager
{
    public class SqlParseRecordManager
    {
        protected ISqlSugarClient _sqlSugarClient;

        public SqlParseRecordManager(ISqlSugarClient sqlSugarClient)
        {
            _sqlSugarClient = sqlSugarClient;
        }


        public async Task<SqlParseRecord> CreateAsync(SqlParseRecord sqlParseRecord)
        {
            var result = await _sqlSugarClient.Insertable(sqlParseRecord).ExecuteReturnEntityAsync();
            return result;
        }


    }
}
