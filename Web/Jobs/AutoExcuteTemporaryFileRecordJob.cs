
using Microsoft.AspNetCore.Hosting;
using System.IO;
using YouJu.Infrastructure.IO;

namespace Web.Jobs
{
    public class AutoExcuteTemporaryFileRecordJob : IAutoExcuteTemporaryFileRecordJob
    {

        protected ISqlSugarClient _sqlSugarClient;
        private readonly IWebHostEnvironment _environment;

        public AutoExcuteTemporaryFileRecordJob(ISqlSugarClient sqlSugarClient, IWebHostEnvironment environment)
        {
            _sqlSugarClient = sqlSugarClient;
            _environment = environment;
        }

        public async Task ExecuteAsync()
        {
            var now = DateTime.Now.AddHours(-2);
            var list = await _sqlSugarClient.Queryable<TemporaryFileRecord>().Where(x => x.CreationTime < now).ToListAsync();
            foreach (var item in list)
            {
                if (item.NewFileSuffix.IsFolder())
                {
                    if (Directory.Exists(Path.Combine(_environment.WebRootPath, item.NewPath)))
                    {
                        DirectoryHelper.Delete(Path.Combine(_environment.WebRootPath, item.NewPath),true);
                    }
                }
                else
                {
                    FileHelper.DeleteIfExists(Path.Combine(_environment.WebRootPath, item.NewPath));
                }

            }

            await _sqlSugarClient.Deleteable(list).ExecuteCommandAsync();

        }
    }
}
