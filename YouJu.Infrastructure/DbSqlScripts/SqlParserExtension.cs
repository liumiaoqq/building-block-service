using gudusoft.gsqlparser.stmt;
using gudusoft.gsqlparser;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using gudusoft.gsqlparser.nodes;
using MailKit;
using System.Text.RegularExpressions;
using YouJu.Infrastructure.Dto;

namespace YouJu.Infrastructure.DbSqlScripts
{
    public static class SqlParserExtension
    {
        /// <summary>
        /// 解析 SQL 脚本并返回表定义。
        /// </summary>
        /// <param name="sqlScript">SQL 脚本</param>
        /// <returns>表定义列表</returns>
        public static List<TableDefinition> ParseMysqlCreateTableStatements(string sqlScript)
        {
            // 替换ON UPDATE CURRENT_TIMESTAMP(0)为空字符串 因为gsqlparser解析不了
            sqlScript = sqlScript.Replace("ON UPDATE CURRENT_TIMESTAMP(0)", " ");
            var tables = new List<TableDefinition>();


            TGSqlParser parser = new TGSqlParser(EDbVendor.dbvmysql);
            parser.sqltext = sqlScript;

            if (parser.parse() == 0)
            {

                foreach (var sqlstatement in parser.sqlstatements)
                {
                    TableDefinition table = null;
                    // 添加表关系列表

                    if (sqlstatement.GetType().IsAssignableFrom(typeof(TDropTableSqlStatement)))
                    {
                        continue;
                    }
                    if (sqlstatement.GetType().IsAssignableFrom(typeof(TCreateTableSqlStatement)))
                    {
                        var createTableSqlStatement = sqlstatement as TCreateTableSqlStatement;

                        table = new TableDefinition()
                        {
                            TableCode = createTableSqlStatement.TableName.TableString.ReplaceSpecialCharacters(),
                            TableName = createTableSqlStatement.MySQLTableOptionList.FirstOrDefault(x => x.tableOption == ETableOption.EO_COMMENT)?.OptionValue,
                            Columns = new List<ColumnDefinition>(),
                            Relations = new List<TableRelation>()
                        };
                        table.TableName = table.TableName ?? table.TableCode;
                        table.TableName = table.TableName.ReplaceAll(new List<string> { "'", "`" }, "");
                        //如果结尾是表或则表表或则表表表或则表表表表都替换成""
                        if (table.TableName.Contains("表"))
                        {
                            table.TableName = table.TableName.Replace("表", "");
                        }
                        foreach (var columnSqlStatement in createTableSqlStatement.ColumnList)
                        {
                            var tColumnDefinition = columnSqlStatement as TColumnDefinition;
                            var column = new ColumnDefinition()
                            {
                                Name = tColumnDefinition.columnComment?.String?.ReplaceSpecialCharacters(),
                                Code = tColumnDefinition.ColumnName.ColumnNameOnly?.ReplaceSpecialCharacters(),
                                Content = tColumnDefinition.columnComment?.String?.ReplaceSpecialCharacters(),
                                Length = tColumnDefinition.Datatype.Length?.String?.ReplaceSpecialCharacters(),
                                Type = tColumnDefinition.Datatype.DataTypeName?.ReplaceSpecialCharacters(),
                                IsNull = tColumnDefinition.Null,
                            };
                            table.Columns.Add(column);
                        }


                        // 解析外键约束
                        if (createTableSqlStatement.TableConstraints != null)
                        {
                            foreach (var constraint in createTableSqlStatement.TableConstraints)
                            {
                                var tConstraint = constraint as TConstraint;

                                if (tConstraint.Constraint_type == EConstraintType.primary_key)
                                {
                                    table.Columns.FirstOrDefault(x => x.Code == tConstraint.IndexCols[0].String.ReplaceSpecialCharacters()).IsPrimaryKey = true;
                                }

                                if (tConstraint.Constraint_type == EConstraintType.foreign_key)
                                {
                                    foreach (var referencedColumnItem in tConstraint.ReferencedColumnList)
                                    {

                                        var referencedColumn = referencedColumnItem as TObjectName;

                                        table.Relations.Add(new TableRelation()
                                        {
                                            TableCode = table.TableName.ReplaceSpecialCharacters(),
                                            ColumnCode = tConstraint.IndexCols[0].String.ReplaceSpecialCharacters(),
                                            RefTableCode = tConstraint.ReferencedObject.TableString.ReplaceSpecialCharacters(),
                                            RefColumnCode = referencedColumn.String.ReplaceSpecialCharacters()
                                        });

                                    }

                                }



                            }
                        }

                        tables.Add(table);
                    }
                }
            }



            return tables;
        }

        public static string ReplaceSpecialCharacters(this string str)
        {
            return str.ReplaceAll(new List<string> { "'", "`" }, "");
        }


        /// <summary>
        /// 替换字符串列表中的字符串
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="oldValues">要替换的旧字符串列表</param>
        /// <param name="newValue">替换后的新字符串</param>
        public static string ReplaceAll(this string str, List<string> oldValues, string newValue)
        {
            foreach (var oldValue in oldValues)
            {
                str = str.Replace(oldValue, newValue);
            }
            return str;
        }

        public static List<TableDefinition> ParseMysqlCreateTableBySigleBatchStatements(string sqlScript)
        {
            // 将SQL脚本按表分割处理
            var sqlBlocks = SplitSqlByTables(sqlScript);

            List<TableDefinition> tables = new List<TableDefinition>();
            foreach (var sqlBlock in sqlBlocks)
            {
                // 对每个表的SQL块单独解析
                tables.AddRange(SqlParserExtension.ParseMysqlCreateTableStatements(sqlBlock));

            }
            return tables;
        }




        /// <summary>
        /// 将SQL脚本按表分割成多个块
        /// </summary>
        private static List<string> SplitSqlByTables(string sqlContent)
        {
            var result = new List<string>();

            // 使用正则表达式匹配每个CREATE TABLE语句块
            var pattern = @"DROP\s+TABLE\s+IF\s+EXISTS\s+[^;]+;\s*CREATE\s+TABLE\s+[^;]+;";
            var matches = Regex.Matches(sqlContent, pattern, RegexOptions.Singleline);

            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    result.Add(match.Value);
                }
            }

            // 如果没有匹配到DROP TABLE语句，尝试直接匹配CREATE TABLE语句
            if (result.Count == 0)
            {
                pattern = @"CREATE\s+TABLE\s+[^;]+;";
                matches = Regex.Matches(sqlContent, pattern, RegexOptions.Singleline);

                foreach (Match match in matches)
                {
                    if (match.Success)
                    {
                        result.Add(match.Value);
                    }
                }
            }

            return result;
        }

    }
}
