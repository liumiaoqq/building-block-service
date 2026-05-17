using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using YouJu.Infrastructure.Dto;

namespace YouJu.Infrastructure
{
    /// <summary>
    /// 枚举辅助类
    /// </summary>
    public static class EnumExtensions
    {
     
        public static string ToDescription(this Enum enumValue)
        {
            if (enumValue == null) return String.Empty;
            //获取指定枚举值的枚举类型。
            Type type = enumValue.GetType();

            FieldInfo fieldInfo = type.GetField(enumValue.ToString());
            if (fieldInfo != null)
            {
                //获得枚举中各个字段的定义数组
                var atts = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (atts.Length > 0)
                {
               
                    return atts[0].Description;
                }
            }
            return enumValue.ToString();
        }

        public static int ToInt(this Enum enumValue)
        {
            return Convert.ToInt32(enumValue);
        }
      


        /// <summary>
        /// 获取枚举集合列表
        /// </summary>
        /// <param name="enmType"></param>
        /// <returns></returns>
        public static Dictionary<string,string> GetEnumList(this Type enumType)
        {

            Dictionary<string, string> result = new Dictionary<string, string>();
            Array array = Enum.GetValues(enumType);
            foreach (var item in array)
            {
                DescriptionAttribute[] attributes = (DescriptionAttribute[])item.GetType().GetField(item.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
             
                if (attributes.Length > 0)
                {
                    result.Add(attributes[0].Description, Convert.ToInt32(item).ToString());
                   
                }
            }
            return result;
        }

        /// <summary>
        /// 获取枚举集合列表
        /// </summary>
        /// <param name="enmType"></param>
        /// <returns></returns>
        public static List<SelectResult> GetEnumSelect(this Type enumType)
        {

           var  result = new List<SelectResult>();
            Array array = Enum.GetValues(enumType);
            foreach (var item in array)
            {
                DescriptionAttribute[] attributes = (DescriptionAttribute[])item.GetType().GetField(item.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes.Length > 0)
                {
                   
                    result.Add(new SelectResult() { Name= item.ToString() ,Value= Convert.ToInt32(item).ToString(),Label= attributes[0].Description });

                }
            }
            return result;
        }
    }
}
