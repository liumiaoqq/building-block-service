namespace Web.Dto.Gengerations
{
    public class PlanGengerationSqlDto
    {
        public string Name { get; set; }

        [Description("数据库类型")]
        public DataBaseType DataBaseType { get; set; }

        public string DataBaseTypeFormat => DataBaseType.ToDescription();

        /// <summary>
        /// 整个数据库创建模板
        /// </summary>
        public string CreateDbContentResult { get; set; }
  

    }

}
