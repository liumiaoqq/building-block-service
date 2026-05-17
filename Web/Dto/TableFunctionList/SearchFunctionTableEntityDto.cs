namespace Web.Dto.TableFunctionList
{
    public class SearchFunctionTableEntityDto
    {
        /// <summary>
        /// 列Id
        /// </summary>
        public Guid ColumnPropId { get; set; }


        /// <summary>
        /// 列属性
        /// </summary>
        public ColumnPropDto ColumnPropDto { get; set; }


        /// <summary>
        /// 是否外键
        /// </summary>
        public bool IsForeignKey { get; set; }




        /// <summary>
        /// 列的名称
        /// </summary>
        public string ShowColumnName { get; set; }

        /// <summary>
        /// 显示列的编码
        /// </summary>
        public string ShowColumnCode { get; set; }


        /// <summary>
        /// 显示顺序
        /// </summary>
        public int ShowSort { get; set; }

        /// <summary>
        /// 对应的类型
        /// </summary>
        public SearchType SearchType { get; set; }


        public object Setting { get; set; }


        public string SearchTypeFormat => SearchType.ToDescription();
        public string SearchTypeValue => SearchType.ToString();
    }
}
