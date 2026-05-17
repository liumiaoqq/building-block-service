namespace Web
{
    public class SqlParseRecordPagedInput : PagedBaseInput
    {

        public string InputSql { get; set; }


        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }





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
