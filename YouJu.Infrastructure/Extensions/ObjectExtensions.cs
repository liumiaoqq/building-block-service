using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YouJu.Infrastructure.Extensions
{
    public static class ObjectExtensions
    {
        public static TOut Clone<TIn, TOut>(this TIn value)
        { 
           return JsonConvert.DeserializeObject<TOut>(JsonConvert.SerializeObject(value));
        }
        public static string ToJson<T>(this T value)
        {
            
            if (value == null) {
                return JsonConvert.SerializeObject(Activator.CreateInstance<T>());
            }
            var jsonSerializerSettings= new JsonSerializerSettings();
            jsonSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            jsonSerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            jsonSerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
            jsonSerializerSettings.Formatting = Formatting.None;

            return UpperFirst(JsonConvert.SerializeObject(value, jsonSerializerSettings));
        }
        /// <summary>
        /// Json字符串首字母转大写
        /// </summary>
        /// <param name="strJsonData">json字符串</param>
        /// <returns></returns>
        public static string UpperFirst(string strJsonData)
        {
            MatchCollection matchCollection = Regex.Matches(strJsonData, "\\\"[a-zA-Z0-9]+\\\"\\s*:");
            foreach (Match item in matchCollection)
            {
                string res = Regex.Replace(item.Value, @"\b[a-z]\w+", delegate (Match match)
                {
                    string val = match.ToString();
                    return char.ToUpper(val[0]) + val.Substring(1);
                });
                strJsonData = strJsonData.Replace(item.Value, res);
            }
            return strJsonData;
        }
    }
}
