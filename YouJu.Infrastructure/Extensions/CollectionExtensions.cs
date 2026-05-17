using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YouJu.Infrastructure
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// 如果条件成立，添加项
        /// </summary>
        public static void AddIf<T>(this ICollection<T> collection, T value, bool flag)
        {

            if (flag)
            {
                collection.Add(value);
            }
        }

        /// <summary>
        /// 如果条件成立，添加项
        /// </summary>
        public static void AddIf<T>(this ICollection<T> collection, T value, Func<bool> func)
        {

            if (func())
            {
                collection.Add(value);
            }
        }

        /// <summary>
        /// 如果不存在，添加项
        /// </summary>
        public static void AddIfNotExist<T>(this ICollection<T> collection, T value, Func<T, bool> existFunc = null)
        {
            bool exists = existFunc == null ? collection.Contains(value) : collection.Any(existFunc);
            if (!exists)
            {
                collection.Add(value);
            }
        }

        /// <summary>
        /// 如果不为空，添加项
        /// </summary>
        public static void AddIfNotNull<T>(this ICollection<T> collection, T value) where T : class
        {

            if (value != null)
            {
                collection.Add(value);
            }
        }

        /// <summary>
        /// 获取对象，不存在对使用委托添加对象
        /// </summary>
        public static T GetOrAdd<T>(this ICollection<T> collection, Func<T, bool> selector, Func<T> factory)
        {

            T item = collection.FirstOrDefault(selector);
            if (item == null)
            {
                item = factory();
                collection.Add(item);
            }

            return item;
        }

        /// <summary>
        /// 判断集合是否为null或空集合
        /// </summary>
        public static bool IsNullOrWhiteSpace<T>(this ICollection<T> collection)
        {
            return collection == null || collection.Count == 0;
        }

        /// <summary>
        /// 交换两项的位置
        /// </summary>
        public static void Swap<T>(this List<T> list, int index1, int index2)
        {


            if (index1 == index2)
            {
                return;
            }

            T tmp = list[index1];
            list[index1] = list[index2];
            list[index2] = tmp;
        }
        public static List<T> AddEmptyByLength<T>(this List<T> enumerable, int lenth)
        {
            for (int i = 0; i < lenth; i++)
            {
                enumerable.Add(Activator.CreateInstance<T>());
            }
            return enumerable;
        }


    }
}
