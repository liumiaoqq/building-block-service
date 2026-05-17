using Web.Caches;
using System;
using System.Threading.Tasks;

namespace Web.Extensions
{
    /// <summary>
    /// Redis分布式锁扩展方法
    /// </summary>
    public static class RedisLockExtensions
    {
        /// <summary>
        /// 使用Redis锁执行操作的扩展方法
        /// </summary>
        /// <param name="lockService">Redis锁服务</param>
        /// <param name="lockKey">锁的键名</param>
        /// <param name="action">要执行的操作</param>
        /// <param name="expireTimeInSeconds">锁的过期时间（秒）</param>
        /// <param name="timeoutMs">获取锁的超时时间（毫秒）</param>
        /// <returns>操作是否成功执行</returns>
        public static async Task<bool> WithLockAsync(this IRedisLockService lockService, string lockKey, Func<Task> action, int expireTimeInSeconds = 30, int timeoutMs = 5000)
        {
            var lockValue = Guid.NewGuid().ToString();
            var endTime = DateTime.UtcNow.AddMilliseconds(timeoutMs);

            // 尝试获取锁，直到超时
            while (DateTime.UtcNow < endTime)
            {
                if (await lockService.TryLockAsync(lockKey, lockValue, expireTimeInSeconds))
                {
                    try
                    {
                        await action();
                        return true;
                    }
                    finally
                    {
                        await lockService.ReleaseLockAsync(lockKey, lockValue);
                    }
                }

                await Task.Delay(50); // 等待50ms后重试
            }

            return false;
        }

        /// <summary>
        /// 使用Redis锁执行操作并返回结果的扩展方法
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="lockService">Redis锁服务</param>
        /// <param name="lockKey">锁的键名</param>
        /// <param name="func">要执行的函数</param>
        /// <param name="expireTimeInSeconds">锁的过期时间（秒）</param>
        /// <param name="timeoutMs">获取锁的超时时间（毫秒）</param>
        /// <returns>函数执行结果</returns>
        /// <exception cref="TimeoutException">获取锁超时</exception>
        public static async Task<T> WithLockAsync<T>(this IRedisLockService lockService, string lockKey, Func<Task<T>> func, int expireTimeInSeconds = 30, int timeoutMs = 5000)
        {
            var lockValue = Guid.NewGuid().ToString();
            var endTime = DateTime.UtcNow.AddMilliseconds(timeoutMs);

            // 尝试获取锁，直到超时
            while (DateTime.UtcNow < endTime)
            {
                if (await lockService.TryLockAsync(lockKey, lockValue, expireTimeInSeconds))
                {
                    try
                    {
                        return await func();
                    }
                    finally
                    {
                        await lockService.ReleaseLockAsync(lockKey, lockValue);
                    }
                }

                await Task.Delay(50); // 等待50ms后重试
            }

            throw new TimeoutException($"Unable to acquire lock for key: {lockKey} within {timeoutMs}ms");
        }

        /// <summary>
        /// 安全地释放锁的扩展方法
        /// </summary>
        /// <param name="lockService">Redis锁服务</param>
        /// <param name="lockKey">锁的键名</param>
        /// <param name="lockValue">锁的值</param>
        /// <returns>释放结果</returns>
        public static async Task<bool> SafeReleaseLockAsync(this IRedisLockService lockService, string lockKey, string lockValue)
        {
            try
            {
                return await lockService.ReleaseLockAsync(lockKey, lockValue);
            }
            catch
            {
                // 忽略释放锁时的异常，避免影响主要业务逻辑
                return false;
            }
        }
    }
}