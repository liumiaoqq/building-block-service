using AutoMapper;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SqlSugar;
using System.Collections.Concurrent;
using System.Configuration;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using YouJu.Infrastructure;

namespace Web
{
    public static class ConfigureServicesExtension
    {
        public static IServiceCollection ConfigureCors(this IServiceCollection services)
        {
            IConfiguration configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

            services.AddCors(p => p.AddPolicy("Default",
           policy => policy

             .AllowAnyMethod()
             .AllowAnyOrigin()
             .AllowAnyHeader()
            ));

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
            IConfiguration Configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
            var Audience = Configuration["JwtAuthentication:Audience"];
            var Issuer = Configuration["JwtAuthentication:Issuer"];
            var SecureKey = Configuration["JwtAuthentication:SecureKey"];
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
               {

                   options.RequireHttpsMetadata = Convert.ToBoolean(Configuration["JwtAuthentication:RequireHttpsMetadata"]);
                   options.Audience = Configuration["JwtAuthentication:Audience"];
                   options.SaveToken = true;
                   options.Events = new JwtBearerEvents()
                   {
                       OnAuthenticationFailed = (ctx) =>
                       {

                           return Task.CompletedTask;
                       },
                       OnTokenValidated = async (ctx) =>
                       {
                           //自定义验证逻辑写入当前上下文
                           //   await ctx.OnTokenValidated();
                       },
                       OnForbidden = (ctx) =>
                       {

                           return Task.CompletedTask;
                       }
                   };
                   options.TokenValidationParameters = new TokenValidationParameters()
                   {
                       ValidateIssuerSigningKey = true,
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidAudience = Configuration["JwtAuthentication:Audience"],
                       ValidIssuer = Configuration["JwtAuthentication:Issuer"],

                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("JwtAuthentication:SecureKey").ToString()))
                   };
               });


            return services;
        }
        public static IServiceCollection ConfigureHangFire(this IServiceCollection services)
        {
            IConfiguration configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();


            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(configuration["Hangfire:ConnectionStrings"], new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

            // Add the processing server as IHostedService
            services.AddHangfireServer();
          


            return services;
        }



    }
}
