using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YouJu.Infrastructure;


namespace Web
{
    /// <summary>
    /// sqlsugar扩展实现类
    /// </summary>
    public static class SqlSugarServiceExtension
    {
        /// <summary>
        ///  安全线程根据名称获取反射类型
        /// </summary>
        public static ConcurrentDictionary<string, PropertyInfo> entityPropKV = new ConcurrentDictionary<string, PropertyInfo>();
        public static IServiceCollection AddSqlsugarCore(this IServiceCollection services)

        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            IConfiguration configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();


            services.AddScoped<ISqlSugarClient>(o =>
            {
                var db = new SqlSugarClient(o.GetConnectionConfigList());
                GlobalFilter(db);
                return db;
            });

            return services;
        }







        public static void DataFilter(this IServiceProvider provider, object oldValue, DataFilterModel entityInfo)
        {


            ICurrentUser currentUser = provider.GetRequiredService<ICurrentUser>();

            //inset生效
            if (entityInfo.OperationType == DataFilterType.InsertByObject)
            {
                if (entityInfo.PropertyName == nameof(CreationAuditedAggregateRoot.Id))
                {
                    var id = entityInfo.GetEntityPropertyInfoValue<CreationAuditedAggregateRoot, Guid>(nameof(CreationAuditedAggregateRoot.Id));

                    entityInfo.SetValue(Guid.Empty != id ? id : Guid.NewGuid());

                }
                else if (entityInfo.PropertyName == nameof(CreationAuditedAggregateRoot.CreatorId))
                {
                    var creatorId = entityInfo.GetEntityPropertyInfoValue<CreationAuditedAggregateRoot, Guid?>(nameof(CreationAuditedAggregateRoot.CreatorId));
                    entityInfo.SetValue(creatorId.HasValue && creatorId != Guid.Empty ? creatorId : currentUser?.UserId);
                }
                else if (entityInfo.PropertyName == nameof(CreationAuditedAggregateRoot.CreationTime))
                {
                    var creationTime = entityInfo.GetEntityPropertyInfoValue<CreationAuditedAggregateRoot, DateTime>(nameof(CreationAuditedAggregateRoot.CreationTime));
                    entityInfo.SetValue(creationTime!=DateTime.MinValue? creationTime:DateTime.Now);
                }



            }
        }


        public static T GetEntityPropertyInfoValue<TSource, T>(this DataFilterModel entityInfo, string propName)

        {
            if (!entityPropKV.ContainsKey(propName))
            {
                var type = typeof(TSource);
                entityPropKV[propName] = type.GetProperty(propName);
            }

            var val = entityPropKV[propName].GetValue(entityInfo.EntityValue);
            return (T)val;


        }

        public static List<ConnectionConfig> GetConnectionConfigList(this IServiceProvider provider)
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var allMainConnectionConfigList = new List<ConnectionConfig>();
            allMainConnectionConfigList.Add(new ConnectionConfig()
            {
                ConnectionString = configuration["ConnectionStrings:Default"].ToString(),//Master Connection
                DbType = SqlSugar.DbType.MySql,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true,
                AopEvents = new AopEvents
                {
                    DataExecuting = (oldValue, entityInfo) => DataFilter(provider, oldValue, entityInfo),
                    OnLogExecuting = (sql, p) =>
                    {
                        //Console.WriteLine("执行的sql:" + sql);
                    },
                },

                MoreSettings = new ConnMoreSettings()
                {
                    //IsWithNoLockQuery = true,
                    IsAutoRemoveDataCache = true
                },
                // 从库
                // SlaveConnectionConfigs = new List<SlaveConnectionConfig>()
                //{
                //    new SlaveConnectionConfig() { HitRate = 10, ConnectionString = "server=.;uid=sa;pwd=sasa;database=Appointment_slave_01" },
                //    new SlaveConnectionConfig() { HitRate = 10, ConnectionString = "server=.;uid=sa;pwd=sasa;database=Appointment_slave_02" }
                //},
                // 自定义特性
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                    EntityService = (property, column) =>
                    {
                        //if (column.IsPrimarykey && property.PropertyType == typeof(Guid))
                        //{
                        //    column.IsIdentity = true;
                        //}

                        if (property.PropertyType == typeof(decimal))
                        {

                            column.DataType = "decimal(18, 4)";
                        }

                        if (property.PropertyType == typeof(Guid) || property.PropertyType == typeof(Guid?))
                        {
                            column.DataType = "char(36)";
                        }

                        if (property.PropertyType.IsNullableType())
                        {
                            column.IsNullable = true;
                        }
                        if (property.PropertyType == typeof(string))
                        {

                            column.DataType = "longtext";
                            column.IsNullable = true;
                        }

                    },
                    EntityNameService = (type, entity) =>
                    {
                        var attributes = type.GetCustomAttributes(true);
                        if (attributes.Any(it => it is YoungTableAttribute))
                        {
                            entity.DbTableName = (attributes.First(it => it is YoungTableAttribute) as YoungTableAttribute).Name;
                        }
                    }
                },
            });

            return allMainConnectionConfigList;


        }





        public static void GlobalFilter(ISqlSugarClient db)
        {

            foreach (var entityType in DbOptions.GetTables)
            {
                // 遍历实体类

                if (entityType.GetProperty("IsDeleted") != null)
                { //判断实体类中包含IsDeleted属性
                  //构建动态Lambda

                    var lambda = DynamicExpressionParser.ParseLambda
                    (new[] { Expression.Parameter(entityType, "it") },
                     typeof(bool), $"({nameof(CreationAuditedAggregateRoot.IsDeleted)} ==@0 )",
                      false);
                    db.QueryFilter.Add(new TableFilterItem<object>(entityType, lambda, true)); //将Lambda传入过滤器
                }
                // 租户动态处理，同上

            }

        }

    }
}
