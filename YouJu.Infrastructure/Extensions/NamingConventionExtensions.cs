using System;
using System.Collections.Generic;
using System.Text;

namespace YouJu.Infrastructure
{
    /// <summary>
    /// 命名转换扩展类
    /// 提供各种命名风格之间的转换方法
    /// </summary>
    public static class NamingConventionExtensions
    {
        #region PascalCase 大写驼峰命名

        /// <summary>
        /// 将字符串转换为大写驼峰命名（PascalCase）
        /// 支持蛇形命名(user_code)、小写驼峰(userCode)等格式
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns>大写驼峰格式字符串</returns>
        /// <example>
        /// "userCode".ToPascalCase() => "UserCode"
        /// "user_code".ToPascalCase() => "UserCode"
        /// "user_Code".ToPascalCase() => "UserCode"
        /// "USER_CODE".ToPascalCase() => "UserCode"
        /// </example>
        public static string ToPascalCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // 先按下划线分割
            var parts = input.Split('_', StringSplitOptions.RemoveEmptyEntries);

            var result = new StringBuilder();

            foreach (var part in parts)
            {
                // 对每个部分进行处理：按大写字母分割（处理驼峰命名）
                var words = SplitByCamelCase(part);

                foreach (var word in words)
                {
                    if (!string.IsNullOrEmpty(word))
                    {
                        // 首字母大写，其余小写
                        result.Append(char.ToUpper(word[0]));
                        if (word.Length > 1)
                        {
                            result.Append(word.Substring(1).ToLower());
                        }
                    }
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// 按驼峰命名分割字符串
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns>分割后的单词列表</returns>
        /// <example>
        /// "userCode" => ["user", "Code"]
        /// "UserCode" => ["User", "Code"]
        /// </example>
        private static List<string> SplitByCamelCase(string input)
        {
            var words = new List<string>();
            if (string.IsNullOrEmpty(input))
                return words;

            var currentWord = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                if (char.IsUpper(c) && currentWord.Length > 0)
                {
                    // 遇到大写字母且当前词不为空，保存当前词并开始新词
                    words.Add(currentWord.ToString());
                    currentWord.Clear();
                }

                currentWord.Append(c);
            }

            // 添加最后一个词
            if (currentWord.Length > 0)
            {
                words.Add(currentWord.ToString());
            }

            return words;
        }

        #endregion

        #region 括号内容处理

        /// <summary>
        /// 移除字符串中的括号及其内容
        /// 支持中英文括号
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns>移除括号后的字符串</returns>
        /// <example>
        /// "语气类型(1:温柔,2:活泼)".RemoveBracketContent() => "语气类型"
        /// "状态（启用/禁用）".RemoveBracketContent() => "状态"
        /// </example>
        public static string RemoveBracketContent(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // 查找括号位置
            var bracketIndex = input.IndexOf('(');
            if (bracketIndex == -1)
            {
                bracketIndex = input.IndexOf('（'); // 中文括号
            }

            if (bracketIndex > 0)
            {
                return input.Substring(0, bracketIndex).Trim();
            }

            return input;
        }

        /// <summary>
        /// 提取字符串中括号内的内容
        /// 支持中英文括号
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns>括号内的内容，如果没有括号则返回空字符串</returns>
        /// <example>
        /// "语气类型(1:温柔,2:活泼)".ExtractBracketContent() => "1:温柔,2:活泼"
        /// </example>
        public static string ExtractBracketContent(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // 查找开始括号
            var startIndex = input.IndexOf('(');
            var endChar = ')';
            if (startIndex == -1)
            {
                startIndex = input.IndexOf('（');
                endChar = '）';
            }

            if (startIndex == -1)
                return string.Empty;

            // 查找结束括号
            var endIndex = input.IndexOf(endChar, startIndex);
            if (endIndex == -1)
                return string.Empty;

            return input.Substring(startIndex + 1, endIndex - startIndex - 1);
        }

        /// <summary>
        /// 判断字符串是否包含括号
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns>是否包含括号</returns>
        public static bool HasBracketContent(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            return input.Contains('(') || input.Contains('（');
        }

        #endregion

        #region camelCase 小写驼峰命名

        /// <summary>
        /// 将字符串转换为小写驼峰命名（camelCase）
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns>小写驼峰格式字符串</returns>
        /// <example>
        /// "UserCode".ToCamelCase() => "userCode"
        /// "user_code".ToCamelCase() => "userCode"
        /// </example>
        public static string ToCamelCase(this string input)
        {
            var pascalCase = input.ToPascalCase();
            if (string.IsNullOrEmpty(pascalCase))
                return pascalCase;

            return char.ToLower(pascalCase[0]) + pascalCase.Substring(1);
        }

        #endregion

        #region snake_case 蛇形命名

        /// <summary>
        /// 将字符串转换为蛇形命名（snake_case）
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns>蛇形格式字符串</returns>
        /// <example>
        /// "UserCode".ToSnakeCase() => "user_code"
        /// "userCode".ToSnakeCase() => "user_code"
        /// </example>
        public static string ToSnakeCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var result = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (char.IsUpper(c))
                {
                    if (i > 0)
                        result.Append('_');
                    result.Append(char.ToLower(c));
                }
                else
                {
                    result.Append(c);
                }
            }
            return result.ToString();
        }

        #endregion
    }
}
