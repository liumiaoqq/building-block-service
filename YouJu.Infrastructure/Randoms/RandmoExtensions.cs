using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouJu.Infrastructure
{
    public static class RandmoExtensions
    {
        /// <summary>
        /// 返回不重复的随机数，start和end表示随机的范围，count表示返回的数量
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static int[] GenerateNumber(int start, int end, int count)
        {
            //用于存放start-end范围的数组
            int[] container = new int[end - start];
            //用于保存返回结果
            int[] result = new int[count];
            Random random = new Random();
            for (int i = start; i <= end; i++)
            {
                container[i - 1] = i;
            }
            int index = 0;
            int value = 0;
            for (int i = 0; i < count; i++)
            {
                //从[1,container.Count + 1)中取一个随机值，保证这个值不会超过container的元素个数
                index = random.Next(1, container.Length - 1 - i);
                //以随机生成的值作为索引取container中的值
                value = container[index];
                //将随机取得值的放到结果集合中
                result[i] = value;
                //将刚刚使用到的从容器集合中移到末尾去
                container[index] = container[container.Length - i - 1];
                //将刚使用过的随机数放到container数组末，因为下一次循环不可能用到，保证了取数的唯一性
                container[container.Length - i - 1] = value;
            }
            return result;
        }
        /// <summary>
        /// 返回四位随机数
        /// </summary>
        /// <returns></returns>
        public static string RandomCode_bak()
        {
            Random random = new Random();
            return random.Next(1000, 9999).ToString();
        }
    }
}
