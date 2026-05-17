namespace Web.Tables
{
    public class SqlParseRecordDto : FullBaseDto
    {
        public string InputSql { get; set; }

        public string Error { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }


        /// <summary>
        /// 解析后的结构返回
        /// </summary>
        public string ParseResultJson { get; set; }


        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DbType { get; set; }


        /// <summary>
        /// 转换类型
        /// </summary>
        public SqlParseType SqlParseType { get; set; }


    }
}
