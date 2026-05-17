using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace YouJu.Infrastructure.DbSqlScripts
{
    public class TableDefinition
    {
        [Description("表名")]
        public string TableName { get; set; }

        [Description("表编码")]
        public string TableCode { get; set; }

        [Description("列定义")]
        public List<ColumnDefinition> Columns { get; set; }

        [Description("表关系")]
        public List<TableRelation> Relations { get; set; } = new List<TableRelation>();
    }
}
