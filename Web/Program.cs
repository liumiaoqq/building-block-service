
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Web
{
    public class Program
    {

        public static int Main(string[] args)
        {
            try
            {
                Log.Logger = new LoggerConfiguration()
                           .MinimumLevel.Debug()
                           .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                           .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                           .Enrich.FromLogContext()
                            .WriteTo.Async(c => c.File($"Logs/{DateTime.Now.ToString("yyyy-MM-dd")}-logs.txt"))   
                           .WriteTo.Async(c => c.Console())

                           .CreateLogger();
                Log.Debug("Starting web host");
                var host = CreateHostBuilder(args).Build();



                (host.Services.GetService(typeof(CodeFirst)) as CodeFirst).Init().GetAwaiter();

             
                host.Run();


                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly!");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
              Host.CreateDefaultBuilder(args)
                 .ConfigureWebHostDefaults(webBuilder =>
                 {
                     webBuilder.ConfigureKestrel(opt => {
                         opt.Limits.MaxRequestBodySize = 5*1024*1024;
                         opt.Limits.MaxRequestBufferSize = 5 * 1024 * 1024;
                     });
                     webBuilder.UseStartup<Startup>();
                 })
                  .UseSerilog();



    }


}
