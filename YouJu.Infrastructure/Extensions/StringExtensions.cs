using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YouJu.Infrastructure
{
    public static class StringExtensions
    {
        #region 一些基本的符号常量

        /**/
        /// <summary>
        /// 点符号 .
        /// </summary>
        public const string Dot = ".";

        /**/
        /// <summary>
        /// 下划线 _
        /// </summary>
        public const string UnderScore = "_";

        /**/
        /// <summary>
        /// 逗号加空格 , 
        /// </summary>
        public const string CommaSpace = ", ";

        /**/
        /// <summary>
        /// 逗号 ,
        /// </summary>
        public const string Comma = ",";

        /**/
        /// <summary>
        /// 左括号 (
        /// </summary>
        public const string OpenParen = "(";

        /**/
        /// <summary>
        /// 右括号 )
        /// </summary>
        public const string ClosedParen = ")";

        /**/
        /// <summary>
        /// 单引号 '
        /// </summary>
        public const string SingleQuote = "\'";

        /**/
        /// <summary>
        /// 斜线 \
        /// </summary>
        public const string Slash = @"\";

        #endregion


        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
        public static bool IsNotNullOrNotWhiteSpace(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }


        /// <summary>
        /// 移除最后的字符
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string RemoveFinalChar(string s)
        {
            if (s.Length > 1)
            {
                s = s.Substring(0, s.Length - 1);
            }
            return s;
        }

        /// <summary>
        /// 移除最后的指定字符
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string RemoveFinalComma(string s, string character)
        {
            if (s.Trim().Length > 0)
            {
                int c = s.LastIndexOf(character);
                if (c > 0)
                {
                    s = s.Substring(0, s.Length - (s.Length - c));
                }
            }
            return s;
        }


        /// <summary>
        /// 移除字符中的空格
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string RemoveSpaces(string s)
        {
            s = s.Trim();
            s = s.Replace(" ", "");
            return s;
        }


        /// <summary>
        /// 去除字符串中的头尾空格
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string TrimSpace(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return source;
            return source.Trim(' ').Trim('　');
        }

        /// <summary>
        ///截取字符串前面固定长度的字符
        /// </summary>
        /// <param name="source"></param>
        /// <param name="length">截取的长度</param>
        /// <param name="padding">截取尾部添加字符</param>
        /// <returns></returns>
        public static string Left(this string source, int length, string padding = "...")
        {
            if (string.IsNullOrWhiteSpace(source))
                return "";
            if (source.Length <= length)
            {
                return source;
            }
            return source.Substring(0, length) + padding;
        }




        /// <summary>
        /// 删除不可见字符
        /// </summary>
        public static string RemoveEmpty(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "";
            }
            Regex reg = new Regex(@"[\f\n\r\t\v]*", RegexOptions.IgnoreCase);
            value = reg.Replace(value, "");
            reg = new Regex(@"[ ]+");//合并多个空格为一个
            return reg.Replace(value, " ");
        }

        /// <summary>
        /// 过滤文本中的空行
        /// </summary>
        public static string RemoveEmptyRow(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "";
            }
            Regex reg = new Regex(@"\n[\t|\s| ]*\r", RegexOptions.IgnoreCase);
            return reg.Replace(value, "");
        }

        /// <summary>
        /// 对比字符串是否相等,默认忽略大小写
        /// </summary>
        /// <returns></returns>
        public static bool IsEqualString(string value1, string value2, bool ignoreLower = true)
        {
            return string.Equals(value1, value2,
                ignoreLower ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }


        public static string GetLastSubString(this string value, char character)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;
            if (char.IsWhiteSpace(character))
                return value;
            int postion = value.LastIndexOf(character);
            if (postion == -1)
                return value;

            return value.Substring(value.LastIndexOf(character), value.Length - postion);


        }





        /// <summary>
        /// 单词变成单数形式
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static string ToSingular(this string word)
        {
            Regex plural1 = new Regex("(?<keep>[^aeiou])ies$");
            Regex plural2 = new Regex("(?<keep>[aeiou]y)s$");
            Regex plural3 = new Regex("(?<keep>[sxzh])es$");
            Regex plural4 = new Regex("(?<keep>[^sxzhyu])s$");

            if (plural1.IsMatch(word))
            {
                return plural1.Replace(word, "${keep}y");
            }
            if (plural2.IsMatch(word))
            {
                return plural2.Replace(word, "${keep}");
            }
            if (plural3.IsMatch(word))
            {
                return plural3.Replace(word, "${keep}");
            }
            if (plural4.IsMatch(word))
            {
                return plural4.Replace(word, "${keep}");
            }

            return word;
        }

        /// <summary>
        /// 单词变成复数形式
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static string ToPlural(this string word)
        {
            Regex plural1 = new Regex("(?<keep>[^aeiou])y$");
            Regex plural2 = new Regex("(?<keep>[aeiou]y)$");
            Regex plural3 = new Regex("(?<keep>[sxzh])$");
            Regex plural4 = new Regex("(?<keep>[^sxzhy])$");

            if (plural1.IsMatch(word))
            {
                return plural1.Replace(word, "${keep}ies");
            }
            if (plural2.IsMatch(word))
            {
                return plural2.Replace(word, "${keep}s");
            }
            if (plural3.IsMatch(word))
            {
                return plural3.Replace(word, "${keep}es");
            }
            if (plural4.IsMatch(word))
            {
                return plural4.Replace(word, "${keep}s");
            }

            return word;
        }




        /// <summary>
        /// 加密显示电话
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string EncryptPhone(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return "***";
            }
            if (source.Length > 8)
            {
                return source.Substring(0, 3) + "**" + source.Substring(source.Length - 4);
            }
            if (source.Length > 4)
            {
                return source.Substring(0, 2) + "**" + source.Substring(source.Length - 2);
            }
            return source.Substring(0, 1) + "**" + source.Substring(source.Length - 1);
        }




        /// <summary>
        /// 给URL添加查询参数
        /// </summary>
        /// <param name="url">URL字符串</param>
        /// <param name="queries">要添加的参数，形如："id=1,cid=2"</param>
        /// <returns></returns>
        public static string AddUrlQuery(this string url, params string[] queries)
        {
            foreach (string query in queries)
            {
                if (!url.Contains("?"))
                {
                    url += "?";
                }
                else if (!url.EndsWith("&"))
                {
                    url += "&";
                }

                url = url + query;
            }
            return url;
        }

        /// <summary>
        /// 获取URL中指定参数的值，不存在返回空字符串
        /// </summary>
        public static object GetUrlQuery(this string url, string key)
        {
            var dict = url.GetAllUrlQuery();
            if (dict.ContainsKey(key))
            {
                return dict[key];
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取URL中指定参数的值，不存在返回空字符串
        /// </summary>
        public static TOut GetUrlQuery<TOut>(this string url, string key)
        {
            var dict = url.GetAllUrlQuery();
            if (dict.ContainsKey(key))
            {
                return (TOut)dict[key];
            }
            return Activator.CreateInstance<TOut>();
        }


        public static Dictionary<string, object> GetAllUrlQuery(this string url)
        {
            var temp = new Dictionary<string, object>();
            Uri uri = new Uri(url);
            string query = uri.Query;
            if (string.IsNullOrWhiteSpace(query))
            {
                return temp;
            }
            query = query.TrimStart('?');
            var array = query.Split("&", StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in array)
            {
                var strs = item.Split("=");
                temp.Add(strs[0], strs[1]);
            }
            return temp;

        }


        /// <summary>
        /// 给URL添加 # 参数
        /// </summary>
        /// <param name="url">URL字符串</param>
        /// <param name="query">要添加的参数</param>
        /// <returns></returns>
        public static string AddHashFragment(this string url, string query)
        {
            if (!url.Contains("#"))
            {
                url += "#";
            }

            return url + query;
        }


        public static Guid ToGuid(this string source)
        {

            return Guid.Parse(source);
        }

        public static List<string> JoinAsList(this string source, string cut)
        {
            if (string.IsNullOrWhiteSpace(source))
                return new List<string>();
            return source.Split(cut).ToList();
        }
        public static List<long> JoinAsLong(this string source, string cut)
        {
            if (string.IsNullOrWhiteSpace(source))
                return new List<long>();
            return source.Split(cut).Select(x => long.Parse(x)).ToList();
        }

        public static string EnsureEndsWith(this string str, char character)
        {

            return str.Last() == character ? str : str + character;

        }

        public static bool IsLongValue(this string value)
        {

            return long.TryParse(value, out long result);


        }

        public static long? ToLong(this string value)
        {
            if (value.IsNullOrWhiteSpace()) return null;
            long.TryParse(value, out long result);
            return result;
        }


        public static bool IsFolder(this string value)
        {

            return "folder".Equals(value, StringComparison.OrdinalIgnoreCase);
        }


        public static List<string> GetFileTypes()
        {
            return new List<string>() {
                    ".txt", ".log", ".conf", ".ini", ".properties", ".xml", ".html", ".css", ".js", ".py", ".java", ".c", ".cpp", ".cxx", ".h", ".hpp", ".hxx", ".hh", ".cs", ".asm", ".bat", ".cmd", ".sh",".json",".user",".csproj",".md",".sln",".gitignore",
                 ".vue",".md",".development",".editorconfig",".production",".eslintignore",".gitignore",".env",".yml",".scss",".csproj",
                ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff",
                    ".mp4", ".avi", ".mov", ".wmv", ".mkv",
                    ".mp3", ".wav", ".ogg",
                    ".zip", ".rar", ".7z",
                    ".pdf",
                    ".docx", ".xlsx",
                    ".mdc",".hook"
                };
        }

        public static bool CheckIsFolder(this string value)
        {
            if (value.IsNullOrWhiteSpace()) return true;
            var fileTypes = GetFileTypes();
            int lstPostion = value.LastIndexOf(".");
            if (lstPostion != -1 && fileTypes.Any(x => x.Equals(value.Substring(lstPostion))))
            {
                return false;
            }
            if (lstPostion == -1) return true;
            return true;

        }

        public static string GetFileTypes(this string value)
        {
            var fileTypes = GetFileTypes();
            int lstPostion = value.LastIndexOf(".");
            if (lstPostion != -1 && fileTypes.Any(x => x.Equals(value.Substring(lstPostion))))
            {
                return value.Substring(lstPostion);
            }
            if (lstPostion == -1) return "folder";
            return "folder";

        }
        public static List<T> ToList<T>(this string value)
        {
            JsonSerializerSettings _jsonSetting = new JsonSerializerSettings { ContractResolver = new DefaultContractResolver(), NullValueHandling = NullValueHandling.Ignore };

            if (value.IsNullOrWhiteSpace())
            {
                return new List<T>();
            }
            else
            {
                return JsonConvert.DeserializeObject<List<T>>(value, _jsonSetting);
            }
        }

        /// <summary>
        /// 检测是否存在如下字符
        /// </summary>
        public static List<string> CheckExistChar(this string value, List<string> strings)
        {
            var values = new List<string>();

            foreach (string s in strings)
            {

                if (value.Contains(s))
                {
                    values.Add(s);

                }
            }
            return values;

        }


        public static T DeserializeObject<T>(this string value)
        {
            if (value.IsNullOrWhiteSpace())
            {

                return Activator.CreateInstance<T>();
            }
            //JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
            //{
            //    ContractResolver = new CamelCasePropertyNamesContractResolver(),
            //    Formatting = Formatting.None,
            //};

            return JsonConvert.DeserializeObject<T>(value);

        }




    }
}
