
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace YouJu.Infrastructure
{
   public static  class RegularHelper
    {
        public const string Mobile_Regex = "^1[0-9]{10}$";
        public static string IsMobile(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "手机号码不能为空";
            
            value = value.Trim();
            if (value.Length < 11)
                return "手机号码长度不能小于11位";          
           return Regex.IsMatch(value,Mobile_Regex)?"":"手机格式有误";

        }

        /// <summary>
        /// 检测是否符合email格式
        /// </summary>
        /// <param name="strEmail">要判断的email字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsValidEmail(string strEmail)
        {
            return Regex.IsMatch(strEmail, @"^[\w\.]+([-]\w+)*@[A-Za-z0-9-_]+[\.][A-Za-z0-9-_]");
        }

        /// <summary>
        /// 检查是否是数字
        /// </summary>
        /// <returns></returns>
        public static bool IsNum(string strNum)
        {
            return Regex.IsMatch(strNum, @"^[0-9]");
        }

    }
}
