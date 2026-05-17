using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace YouJu.Infrastructure.Expection
{
    /// <summary>
    /// 异常传输对象
    /// </summary>
    public class YouJuException : Exception
    {
   
        public string Code { get; set; }

        /// <summary>
        /// 额外参数
        /// </summary>
        public Dictionary<string, object> Tag { get; set; }
        public YouJuException()
        {
        }
        public YouJuException(string message, string code = "500")
           : base(message)
        {
            Code = code;

        }
     


        public YouJuException(string message, Exception innerException,string code="500")
            : base(message, innerException)
        {
            Code = code;

        }


    }
}
