using System;
using System.Threading.Tasks;

namespace Web.Caches
{
    /// <summary>
    /// 缓存服务接口
    /// </summary>
    public interface IDistributedCacheService
    {
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T">缓存对象类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <returns>缓存值</returns>
        Task<T> GetAsync<T>(string key);

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T">缓存对象类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="expireTime">过期时间（秒）</param>
        Task SetAsync<T>(string key, T value, int expireTime = 3600);

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        Task RemoveAsync(string key);

        /// <summary>
        /// 判断缓存是否存在
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>是否存在</returns>
        Task<bool> ExistsAsync(string key);

        /// <summary>
        /// 获取或添加缓存
        /// </summary>
        /// <typeparam name="T">缓存对象类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="factory">缓存值工厂</param>
        /// <param name="expireTime">过期时间（秒）</param>
        /// <returns>缓存值</returns>
        Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> factory, int expireTime = 3600);

        /// <summary>
        /// 尝试获取分布式锁
        /// </summary>
        /// <param name="lockKey">锁的键名</param>
        /// <param name="lockValue">锁的值</param>
        /// <param name="expireTimeInSeconds">锁的过期时间（秒）</param>
        /// <returns>是否成功获取锁</returns>
        Task<bool> TryLockAsync(string lockKey, string lockValue, int expireTimeInSeconds = 30);

        /// <summary>
        /// 释放分布式锁
        /// </summary>
        /// <param name="lockKey">锁的键名</param>
        /// <param name="lockValue">锁的值</param>
        /// <returns>是否成功释放锁</returns>
        Task<bool> ReleaseLockAsync(string lockKey, string lockValue);

        /// <summary>
        /// 执行带锁的缓存操作
        /// </summary>
        /// <typeparam name="T">缓存对象类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="factory">缓存值工厂</param>
        /// <param name="expireTime">缓存过期时间（秒）</param>
        /// <param name="lockExpireTime">锁过期时间（秒）</param>
        /// <returns>缓存值</returns>
        Task<T> GetOrAddWithLockAsync<T>(string key, Func<Task<T>> factory, int expireTime = 3600, int lockExpireTime = 30);
    }
}