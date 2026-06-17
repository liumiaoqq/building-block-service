using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;

namespace Web
{
    /// <summary>
    /// 代码迁移
    /// </summary>
    public class CodeFirst
    {
        private IServiceProvider _serviceProvider;

        public CodeFirst(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Init()
        {
            await InitTable();
        }

        public async Task InitTable()
        {
            using (var scope1 = _serviceProvider.CreateScope())
            {
                var configuration = scope1.ServiceProvider.GetService<IConfiguration>();
                if (bool.TryParse(configuration["ConnectionStrings:CodeFisrt"], out bool flag) && flag)
                {
                    try
                    {
                        var tableTypes = DbOptions.GetTables;

                        if (tableTypes != null && tableTypes.Any())
                        {
                            var sqlSugarClient = scope1.ServiceProvider.GetRequiredService<ISqlSugarClient>();
                            sqlSugarClient.DbMaintenance.CreateDatabase();
                            await NormalizeGuidColumnsAsync(sqlSugarClient, tableTypes);
                            await NormalizeSystemConfigAsync(sqlSugarClient);
                            sqlSugarClient.CodeFirst.InitTables(tableTypes.ToArray());
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        private async Task NormalizeGuidColumnsAsync(ISqlSugarClient sqlSugarClient, IEnumerable<Type> tableTypes)
        {
            foreach (var tableType in tableTypes)
            {
                var tableName = GetTableName(tableType);
                if (!sqlSugarClient.DbMaintenance.IsAnyTable(tableName, false))
                {
                    continue;
                }

                var columns = sqlSugarClient.DbMaintenance.GetColumnInfosByTableName(tableName, false);
                var guidPropertyNames = GetGuidPropertyNames(tableType);
                var guidColumns = columns
                    .Where(x => guidPropertyNames.Contains(x.DbColumnName, StringComparer.OrdinalIgnoreCase))
                    .ToList();
                if (!guidColumns.Any())
                {
                    continue;
                }

                foreach (var column in guidColumns)
                {
                    var nullable = column.IsNullable ? "NULL" : "NOT NULL";
                    await sqlSugarClient.Ado.ExecuteCommandAsync($"ALTER TABLE `{tableName}` MODIFY COLUMN `{column.DbColumnName}` CHAR(36) {nullable}");
                }
            }
        }

        private HashSet<string> GetGuidPropertyNames(Type tableType)
        {
            return tableType
                .GetProperties()
                .Where(x => x.PropertyType == typeof(Guid) || x.PropertyType == typeof(Guid?))
                .Select(x => x.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        private string GetTableName(Type tableType)
        {
            return tableType.GetCustomAttributes(true)
                .OfType<YoungTableAttribute>()
                .FirstOrDefault()?.Name ?? tableType.Name;
        }

        private async Task NormalizeSystemConfigAsync(ISqlSugarClient sqlSugarClient)
        {
            const string tableName = "SystemConfig";
            if (!sqlSugarClient.DbMaintenance.IsAnyTable(tableName, false))
            {
                return;
            }

            var columns = sqlSugarClient.DbMaintenance.GetColumnInfosByTableName(tableName, false);
            var hasIsAuditEnabled = columns.Any(x => x.DbColumnName.Equals(nameof(SystemConfig.IsAuditEnabled), StringComparison.OrdinalIgnoreCase));
            var hasOldIsEnabled = columns.Any(x => x.DbColumnName.Equals("IsEnabled", StringComparison.OrdinalIgnoreCase));
            if (!hasIsAuditEnabled)
            {
                await sqlSugarClient.Ado.ExecuteCommandAsync($"ALTER TABLE `{tableName}` ADD COLUMN `{nameof(SystemConfig.IsAuditEnabled)}` TINYINT(1) NOT NULL DEFAULT 0");
            }

            if (hasOldIsEnabled)
            {
                await sqlSugarClient.Ado.ExecuteCommandAsync($"UPDATE `{tableName}` SET `{nameof(SystemConfig.IsAuditEnabled)}` = `IsEnabled`");
                await sqlSugarClient.Ado.ExecuteCommandAsync($"ALTER TABLE `{tableName}` DROP COLUMN `IsEnabled`");
            }

            foreach (var oldColumn in new[] { "Name", "Code", "Remark" })
            {
                if (columns.Any(x => x.DbColumnName.Equals(oldColumn, StringComparison.OrdinalIgnoreCase)))
                {
                    await sqlSugarClient.Ado.ExecuteCommandAsync($"ALTER TABLE `{tableName}` DROP COLUMN `{oldColumn}`");
                }
            }
        }
    }
}
