using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Web.Caches
{
    /// <summary>
    /// Redis分布式锁服务实现
    /// </summary>
    public class RedisLockService : IRedisLockService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly ILogger<RedisLockService> _logger;
        private readonly IDatabase _database;

        // Lua脚本用于原子性地释放锁
        private const string ReleaseLockScript = @"
            if redis.call('get', KEYS[1]) == ARGV[1] then
                return redis.call('del', KEYS[1])
            else
                return 0
            end";

        // Lua脚本用于原子性地续期锁
        private const string RenewLockScript = @"
            if redis.call('get', KEYS[1]) == ARGV[1] then
                return redis.call('expire', KEYS[1], ARGV[2])
            else
                return 0
            end";

        public RedisLockService(IDistributedCache distributedCache, IConnectionMultiplexer connectionMultiplexer, ILogger<RedisLockService> logger)
        {
            _distributedCache = distributedCache;
            _connectionMultiplexer = connectionMultiplexer;
            _logger = logger;
            _database = _connectionMultiplexer.GetDatabase();
        }

        /// <summary>
        /// 尝试获取锁
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
        /// 释放锁
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
        /// 续期锁
        /// </summary>
        public async Task<bool> RenewLockAsync(string lockKey, string lockValue, int expireTimeInSeconds = 30)
        {
            try
            {
                var key = GetLockKey(lockKey);
                var result = await _database.ScriptEvaluateAsync(RenewLockScript, new RedisKey[] { key }, new RedisValue[] { lockValue, expireTimeInSeconds });

                var success = result.ToString() == "1";
                if (success)
                {
                    _logger.LogDebug("Successfully renewed lock for key: {LockKey}", lockKey);
                }
                else
                {
                    _logger.LogDebug("Failed to renew lock for key: {LockKey} (lock may have expired or been held by another process)", lockKey);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while trying to renew lock for key: {LockKey}", lockKey);
                return false;
            }
        }

        /// <summary>
        /// 执行带锁的操作
        /// </summary>
        public async Task<bool> ExecuteWithLockAsync(string lockKey, Func<Task> action, int expireTimeInSeconds = 30, int retryCount = 3, int retryDelayMs = 100)
        {
            var lockValue = Guid.NewGuid().ToString();

            for (int i = 0; i < retryCount; i++)
            {
                if (await TryLockAsync(lockKey, lockValue, expireTimeInSeconds))
                {
                    try
                    {
                        await action();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error occurred while executing action with lock: {LockKey}", lockKey);
                        throw;
                    }
                    finally
                    {
                        await ReleaseLockAsync(lockKey, lockValue);
                    }
                }

                if (i < retryCount - 1)
                {
                    await Task.Delay(retryDelayMs);
                }
            }

            _logger.LogWarning("Failed to acquire lock for key: {LockKey} after {RetryCount} attempts", lockKey, retryCount);
            return false;
        }

        /// <summary>
        /// 执行带锁的操作并返回结果
        /// </summary>
        public async Task<T> ExecuteWithLockAsync<T>(string lockKey, Func<Task<T>> func, int expireTimeInSeconds = 30, int retryCount = 3, int retryDelayMs = 100)
        {
            var lockValue = Guid.NewGuid().ToString();

            for (int i = 0; i < retryCount; i++)
            {
                if (await TryLockAsync(lockKey, lockValue, expireTimeInSeconds))
                {
                    try
                    {
                        return await func();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error occurred while executing function with lock: {LockKey}", lockKey);
                        throw;
                    }
                    finally
                    {
                        await ReleaseLockAsync(lockKey, lockValue);
                    }
                }

                if (i < retryCount - 1)
                {
                    await Task.Delay(retryDelayMs);
                }
            }

            _logger.LogWarning("Failed to acquire lock for key: {LockKey} after {RetryCount} attempts", lockKey, retryCount);
            throw new InvalidOperationException($"Unable to acquire lock for key: {lockKey} after {retryCount} attempts");
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