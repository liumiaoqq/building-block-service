using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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

            await this.InitTable();



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
                            sqlSugarClient.CodeFirst.InitTables(tableTypes.ToArray());//Create CodeFirstTable1 

                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }


        }


    }
}
