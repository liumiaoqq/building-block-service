using Web.Manager;
using YouJu.Infrastructure.DbSqlScripts;

namespace Web.Service
{
    public class SqlParseRecordService
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        private readonly ICurrentUser _currentUser;

        private readonly SqlParseRecordManager _sqlParseRecordManager;

        public SqlParseRecordService(ISqlSugarClient sqlSugarClient, ICurrentUser currentUser, SqlParseRecordManager sqlParseRecordManager)
        {
            _sqlSugarClient = sqlSugarClient;
            _currentUser = currentUser;
            _sqlParseRecordManager = sqlParseRecordManager;
        }

        public async Task<List<TableDefinition>> ParseSql(ParseSqlInput input)
        {
            if (string.IsNullOrWhiteSpace(input.InputSql))
            {
                throw new YouJuException("输入的SQL不能为空");
            }
            List<TableDefinition> tables = new List<TableDefinition>();
            var sqlParseRecord = new SqlParseRecord();

            sqlParseRecord.InputSql = input.InputSql;
            sqlParseRecord.DbType = input.DbType;
            sqlParseRecord.SqlParseType = SqlParseType.SqlParseEr;
            sqlParseRecord.IsSuccess = true;
            try
            {
                tables = SqlParserExtension.ParseMysqlCreateTableBySigleBatchStatements(input.InputSql);
                sqlParseRecord.ParseResultJson = tables.ToJson();
                await _sqlParseRecordManager.CreateAsync(sqlParseRecord);
                return tables;

            }
            catch (Exception ex)
            {
                sqlParseRecord.IsSuccess = false;
                sqlParseRecord.Error = ex.Message;
                await _sqlParseRecordManager.CreateAsync(sqlParseRecord);
                throw ex;
            }

        }

    }

}
