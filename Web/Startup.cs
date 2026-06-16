using Hangfire;
using Microsoft.AspNetCore.ResponseCompression;
using Newtonsoft.Json;
using System.IO.Compression;
using Web.Extensions;
using Web.GenerationCode;
using Web.GenerationCode.Manager;
using Web.GenerationCode.Provider;
using Web.HttpClientApi.DeepSeek.Service;
using Web.Jobs;
using Web.Manager;
using Web.Service;
using YouJu.Infrastructure.Email;
using YouJu.Infrastructure.JWT;
using MediatR;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Web.Caches;
using YouJu.Infrastructure.Oss.Qiniu;

namespace Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {


            #region 配置json

            Action<MvcNewtonsoftJsonOptions> setupAction = options =>
            {
                //忽略循环引用
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                ////不使用驼峰样式的key
                // options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                //将long类型转为string
                //设置时间格式
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                //忽略Model中为null的属性
                //options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                //设置本地时间而非UTC时间
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                options.SerializerSettings.Formatting = Formatting.None;
            };

            services.AddMvc().AddNewtonsoftJson(setupAction);

            services.AddControllers().AddNewtonsoftJson(setupAction);


            #endregion

            #region 注册全局控制器过滤器
            services.AddControllers(options =>
            {

                options.Filters.Add(new YouJuUserAuthorizationFilter());
                options.Filters.Add(new YouJuGlobalExceptionFilter());
                options.Filters.Add(new YouJuAsyncResultFilter());

            });

            #endregion

            #region 添加httpcontext上下文
            services.AddHttpContextAccessor();
            #endregion

            #region Redis缓存配置
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration["Redis:ConnectionString"];
                options.InstanceName = Configuration["Redis:InstanceName"];
            });

            // 注册 Redis ConnectionMultiplexer
            services.AddSingleton<StackExchange.Redis.IConnectionMultiplexer>(provider =>
            {
                var connectionString = Configuration["Redis:ConnectionString"];
                return StackExchange.Redis.ConnectionMultiplexer.Connect(connectionString);
            });

            // 注册缓存服务和Redis锁服务
            services.AddScoped<IDistributedCacheService, RedisCacheService>();
            services.AddScoped<IRedisLockService, RedisLockService>();
            #endregion

            #region Swagger配置
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Web", Version = "v1" });
            //});//引入动态api操作

            #endregion


            services.ConfigureAuthentication();


            services.ConfigureHangFire();


            ; services.AddHttpClient("DeepSeek", client =>
                        {
                            var baseUrl = Configuration["HttpClients:DeepSeek:BaseUrl"];
                            var apiKey = Configuration["HttpClients:DeepSeek:ApiKey"];
                            client.BaseAddress = new Uri(baseUrl);
                            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                            client.Timeout = TimeSpan.FromMinutes(5);
                        });

            services.AddTransient<IDeepSeekService, DeepSeekService>();


            //一个policy 有多个Requirements  一个requirements对应一个Handler
            services.AddAuthorizationCore();

            services.ConfigureCors();

            services.AutoRegisterMapper();

            services.AddSqlsugarCore();

            services.AddSingleton<CodeFirst>();

            services.AddScoped<ICurrentUser, CurrentUser>();


            services.AddTransient(typeof(ManagerService<,>), typeof(ManagerService<,>));

            services.AddTransient<IJwtHelper, JwtHelper>();

            services.AddTransient<PlanManager, PlanManager>();
            services.AddTransient<PlanEnumManager, PlanEnumManager>();
            services.AddTransient<SystemModuleManager, SystemModuleManager>();
            services.AddTransient<TableEntityManager, TableEntityManager>();
            services.AddTransient<TableSettingManager, TableSettingManager>();
            services.AddTransient<ComponentManager, ComponentManager>();
            services.AddTransient<DicManager, DicManager>();
            services.AddTransient<PlanModuleRelativeManager, PlanModuleRelativeManager>();
            services.AddTransient<ColumnPropManager, ColumnPropManager>();
            services.AddTransient<TemporaryFileRecordManager, TemporaryFileRecordManager>();
            services.AddTransient<TableNavigateRelativeManager, TableNavigateRelativeManager>();
            services.AddTransient<ExportRuleManager, ExportRuleManager>();
            services.AddTransient<EditRuleManager, EditRuleManager>();
            services.AddTransient<ViewRuleManager, ViewRuleManager>();
            services.AddTransient<SearchRuleManager, SearchRuleManager>();
            services.AddTransient<WarehouseManager, WarehouseManager>();
            services.AddTransient<SystemComponentRuleManager, SystemComponentRuleManager>();
            services.AddTransient<SqlTempleteManager, SqlTempleteManager>();
            services.AddTransient<SqlParseRecordManager>();
            services.AddTransient<ContactInfoManager, ContactInfoManager>();




            services.AddTransient<SystemModuleService, SystemModuleService>();
            services.AddTransient<RuleCenterService, RuleCenterService>();
            services.AddTransient<PlanService, PlanService>();
            services.AddTransient<PlanEnumService, PlanEnumService>();
            services.AddTransient<TableEntityService, TableEntityService>();
            services.AddTransient<ColumnPropService, ColumnPropService>();
            services.AddTransient<TableNavigateRelativeService, TableNavigateRelativeService>();
            services.AddTransient<SystemComponentRuleService, SystemComponentRuleService>();
            services.AddTransient<ComponentService, ComponentService>();
            services.AddTransient<SqlTempleteService, SqlTempleteService>();
            services.AddTransient<SqlParseRecordService>();
            services.AddTransient<ContactInfoService, ContactInfoService>();


            services.AddTransient<IAutoExcuteTemporaryFileRecordJob, AutoExcuteTemporaryFileRecordJob>();


            services.AddTransient<IGenerationCodeManager, GenerationCodeManager>();//代码模板生成类

            services.AddTransient<IGengerationTranslateProvider, NetCoreProvider>();//NetCore实现代理
            services.AddTransient<IGengerationTranslateProvider, UniappProvider>();//Uniapp实现代理
            services.AddTransient<IGengerationTranslateProvider, ElementUIProvider>();//ElementUI实现代理
            services.AddTransient<IGengerationTranslateProvider, SpringBootProvider>();//Java实现代理
            services.AddTransient<IGengerationTranferTreeFileService, GengerationTranferTreeFileService>();






            services.Configure<MailSettings>(Configuration.GetSection(nameof(MailSettings)));
            services.AddTransient<IMailService, MailService>();

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.SmallestSize;
            });

            services.AddMediatR(typeof(Startup).Assembly);


            services.AddTransient<IQiuNiuOssHelper, QiuNiuOssHelper>();


        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                RecurringJob.AddOrUpdate<AutoExcuteTemporaryFileRecordJob>("AutoExcuteTemporaryFileRecordJob", a => a.ExecuteAsync(), Cron.Daily());
            }
            YouJuServiceProvider.ServiceProvider = app.ApplicationServices;

            app.UseResponseCompression();

            // app.UseDeveloperExceptionPage();

            // app.UseSwagger();

            //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UI"));

            app.UseHttpsRedirection();//使用https重定向


            app.UseStaticFiles();//使用静态资源




            app.UseRouting();//使用路由

            app.UseCors("Default");  //使用跨域 - 必须在UseRouting之后

            app.UseAuthentication();//使用认证


            //app.UseMiddleware<TimestampMiddleware>();

            app.UseAuthorization();//使用授权



            app.UseEndpoints(endpoints =>
            {

                endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            });//使用端点路由
        }


    }
}
