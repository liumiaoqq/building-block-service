using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Web.Caches
{
    /// <summary>
    /// Redis缓存服务实现
    /// </summary>
    public class RedisCacheService : IDistributedCacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly ILogger<RedisCacheService> _logger;
        private readonly IDatabase _database;

        // Lua脚本用于原子性地释放锁
        private const string ReleaseLockScript = @"
            if redis.call('get', KEYS[1]) == ARGV[1] then
                return redis.call('del', KEYS[1])
            else
                return 0
            end";

        public RedisCacheService(IDistributedCache distributedCache, IConnectionMultiplexer connectionMultiplexer, ILogger<RedisCacheService> logger)
        {
            _distributedCache = distributedCache;
            _connectionMultiplexer = connectionMultiplexer;
            _logger = logger;
            _database = _connectionMultiplexer.GetDatabase();
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T">缓存对象类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <returns>缓存值</returns>
        public async Task<T> GetAsync<T>(string key)
        {
            var value = await _distributedCache.GetStringAsync(key);
            if (string.IsNullOrEmpty(value))
            {
                return default;
            }

            return JsonConvert.DeserializeObject<T>(value);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T">缓存对象类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="expireTime">过期时间（秒）</param>
        public async Task SetAsync<T>(string key, T value, int expireTime = 3600)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expireTime)
            };

            var jsonValue = JsonConvert.SerializeObject(value);
            await _distributedCache.SetStringAsync(key, jsonValue, options);
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        public async Task RemoveAsync(string key)
        {
            await _distributedCache.RemoveAsync(key);
        }

        /// <summary>
        /// 判断缓存是否存在
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>是否存在</returns>
        public async Task<bool> ExistsAsync(string key)
        {
            var value = await _distributedCache.GetStringAsync(key);
            return !string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// 获取或添加缓存
        /// </summary>
        /// <typeparam name="T">缓存对象类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="factory">缓存值工厂</param>
        /// <param name="expireTime">过期时间（秒）</param>
        /// <returns>缓存值</returns>
        public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> factory, int expireTime = 3600)
        {
            var value = await GetAsync<T>(key);
            if (value != null)
            {
                return value;
            }

            value = await factory();
            if (value != null)
            {
                await SetAsync(key, value, expireTime);
            }

            return value;
        }

        /// <summary>
        /// 尝试获取分布式锁
        /// </summary>
        public async Task<bool> TryLockAsync(string lockKey, string lockValue, int expireTimeInSeconds = 30)
        {
            try
            {
                var key = GetLockKey(lockKey);
                var result = await _database.StringSetAsync(key, lockValue, TimeSpan.FromSeconds(expireTimeInSeconds), When.NotExists);

                if (result)
                {
                    _logger.LogDebug("Successfully acquired lock for key: {LockKey}", lockKey);
                }
                else
                {
                    _logger.LogDebug("Failed to acquire lock for key: {LockKey}", lockKey);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while trying to acquire lock for key: {LockKey}", lockKey);
                return false;
            }
        }

        /// <summary>
        /// 释放分布式锁
        /// </summary>
        public async Task<bool> ReleaseLockAsync(string lockKey, string lockValue)
        {
            try
            {
                var key = GetLockKey(lockKey);
                var result = await _database.ScriptEvaluateAsync(ReleaseLockScript, new RedisKey[] { key }, new RedisValue[] { lockValue });

                var success = result.ToString() == "1";
                if (success)
                {
                    _logger.LogDebug("Successfully released lock for key: {LockKey}", lockKey);
                }
                else
                {
                    _logger.LogDebug("Failed to release lock for key: {LockKey} (lock may have expired or been held by another process)", lockKey);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while trying to release lock for key: {LockKey}", lockKey);
                return false;
            }
        }

        /// <summary>
        /// 执行带锁的缓存操作
        /// </summary>
        public async Task<T> GetOrAddWithLockAsync<T>(string key, Func<Task<T>> factory, int expireTime = 3600, int lockExpireTime = 30)
        {
            // 先尝试从缓存获取
            var value = await GetAsync<T>(key);
            if (value != null)
            {
                return value;
            }

            // 使用分布式锁来防止缓存穿透
            var lockKey = $"cache_lock:{key}";
            var lockValue = Guid.NewGuid().ToString();

            if (await TryLockAsync(lockKey, lockValue, lockExpireTime))
            {
                try
                {
                    // 再次检查缓存（双重检查锁定模式）
                    value = await GetAsync<T>(key);
                    if (value != null)
                    {
                        return value;
                    }

                    // 执行工厂方法获取数据
                    value = await factory();
                    if (value != null)
                    {
                        await SetAsync(key, value, expireTime);
                    }

                    return value;
                }
                finally
                {
                    await ReleaseLockAsync(lockKey, lockValue);
                }
            }
            else
            {
                // 如果获取锁失败，等待一小段时间后再次尝试从缓存获取
                await Task.Delay(50);
                return await GetAsync<T>(key);
            }
        }

        /// <summary>
        /// 获取锁的完整键名
        /// </summary>
        private string GetLockKey(string lockKey)
        {
            return $"lock:{lockKey}";
        }
    }
}