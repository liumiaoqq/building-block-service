using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouJu.Infrastructure.DbSqlScripts
{
    public class ColumnDefinition
    {
        [Description("列名")]
        public string Name { get; set; }

        [Description("列代码")]
        public string Code { get; set; }

        [Description("是否为空")]
        public bool IsNull { get; set; }

        [Description("类型")]
        public string Type { get; set; }

        [Description("长度")]
        public string Length { get; set; }

        [Description("内容")]
        public string Content { get; set; }


        [Description("是否为主键")]
        public bool IsPrimaryKey { get; set; }





    }
}
