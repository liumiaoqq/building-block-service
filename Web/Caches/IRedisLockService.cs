using System;
using System.Threading.Tasks;

namespace Web.Caches
{
    /// <summary>
    /// Redis分布式锁服务接口
    /// </summary>
    public interface IRedisLockService
    {
        /// <summary>
        /// 尝试获取锁
        /// </summary>
        /// <param name="lockKey">锁的键名</param>
        /// <param name="lockValue">锁的值</param>
        /// <param name="expireTimeInSeconds">锁的过期时间（秒）</param>
        /// <returns>是否成功获取锁</returns>
        Task<bool> TryLockAsync(string lockKey, string lockValue, int expireTimeInSeconds = 30);

        /// <summary>
        /// 释放锁
        /// </summary>
        /// <param name="lockKey">锁的键名</param>
        /// <param name="lockValue">锁的值</param>
        /// <returns>是否成功释放锁</returns>
        Task<bool> ReleaseLockAsync(string lockKey, string lockValue);

        /// <summary>
        /// 续期锁
        /// </summary>
        /// <param name="lockKey">锁的键名</param>
        /// <param name="lockValue">锁的值</param>
        /// <param name="expireTimeInSeconds">延长的过期时间（秒）</param>
        /// <returns>是否成功续期</returns>
        Task<bool> RenewLockAsync(string lockKey, string lockValue, int expireTimeInSeconds = 30);

        /// <summary>
        /// 执行带锁的操作
        /// </summary>
        /// <param name="lockKey">锁的键名</param>
        /// <param name="action">要执行的操作</param>
        /// <param name="expireTimeInSeconds">锁的过期时间（秒）</param>
        /// <param name="retryCount">重试次数</param>
        /// <param name="retryDelayMs">重试间隔（毫秒）</param>
        /// <returns>操作是否成功执行</returns>
        Task<bool> ExecuteWithLockAsync(string lockKey, Func<Task> action, int expireTimeInSeconds = 30, int retryCount = 3, int retryDelayMs = 100);

        /// <summary>
        /// 执行带锁的操作并返回结果
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="lockKey">锁的键名</param>
        /// <param name="func">要执行的函数</param>
        /// <param name="expireTimeInSeconds">锁的过期时间（秒）</param>
        /// <param name="retryCount">重试次数</param>
        /// <param name="retryDelayMs">重试间隔（毫秒）</param>
        /// <returns>函数执行结果</returns>
        Task<T> ExecuteWithLockAsync<T>(string lockKey, Func<Task<T>> func, int expireTimeInSeconds = 30, int retryCount = 3, int retryDelayMs = 100);
    }
}