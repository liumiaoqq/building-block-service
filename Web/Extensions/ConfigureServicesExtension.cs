using AutoMapper;
using Hangfire;
using Hangfire.SQLite;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using YouJu.Infrastructure;

namespace Web
{
    public static class ConfigureServicesExtension
    {
        public static IServiceCollection ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(p => p.AddPolicy("Default",
                policy => policy
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .AllowAnyHeader()));

            return services;
        }

        public static void AutoRegisterMapper(this IServiceCollection services)
        {
            var all = AssemblyExtension.AllAssemblies;
            var allProfile = all.SelectMany(l => l.DefinedTypes.Where(x => x.IsAssignableTo(typeof(Profile)) && x.IsClass)).ToList();

            services.AddAutoMapper(options =>
            {
                foreach (var item in allProfile)
                {
                    options.AddProfile(item.AsType());
                }
            });
        }

        public static IServiceCollection ConfigureAuthentication(this IServiceCollection services)
        {
            IConfiguration configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
               {
                   options.RequireHttpsMetadata = Convert.ToBoolean(configuration["JwtAuthentication:RequireHttpsMetadata"]);
                   options.Audience = configuration["JwtAuthentication:Audience"];
                   options.SaveToken = true;
                   options.Events = new JwtBearerEvents()
                   {
                       OnAuthenticationFailed = (ctx) => Task.CompletedTask,
                       OnTokenValidated = async (ctx) => { },
                       OnForbidden = (ctx) => Task.CompletedTask
                   };
                   options.TokenValidationParameters = new TokenValidationParameters()
                   {
                       ValidateIssuerSigningKey = true,
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidAudience = configuration["JwtAuthentication:Audience"],
                       ValidIssuer = configuration["JwtAuthentication:Issuer"],
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("JwtAuthentication:SecureKey").ToString()))
                   };
               });

            return services;
        }

        public static IServiceCollection ConfigureHangFire(this IServiceCollection services)
        {
            IConfiguration configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
            var connectionString = configuration["Hangfire:ConnectionString"];
            if (connectionString.IsNullOrWhiteSpace())
            {
                connectionString = "Data Source=App_Data/hangfire.db;";
            }

            var dataSource = GetSqliteDataSource(connectionString);
            if (!string.IsNullOrWhiteSpace(dataSource))
            {
                var directory = Path.GetDirectoryName(Path.GetFullPath(dataSource));
                if (!string.IsNullOrWhiteSpace(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }

            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSQLiteStorage(connectionString, new SQLiteStorageOptions
                {
                    QueuePollInterval = TimeSpan.FromSeconds(15),
                    JobExpirationCheckInterval = TimeSpan.FromHours(1),
                    CountersAggregateInterval = TimeSpan.FromMinutes(5)
                }));

            services.AddHangfireServer();

            return services;
        }

        private static string GetSqliteDataSource(string connectionString)
        {
            return connectionString
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Split('=', 2))
                .Where(x => x.Length == 2)
                .FirstOrDefault(x => x[0].Trim().Equals("Data Source", StringComparison.OrdinalIgnoreCase))?[1]
                .Trim();
        }
    }
}
