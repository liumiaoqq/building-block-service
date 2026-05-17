using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouJu.Infrastructure.DbSqlScripts
{
    public class TableRelation
    {
        [Description("表名")]
        public string TableCode { get; set; }


        [Description("列名")]
        public string ColumnCode { get; set; }


        [Description("引用表名")]
        public string RefTableCode { get; set; }
        [Description("引用列名")]
        public string RefColumnCode { get; set; }


    }
}
