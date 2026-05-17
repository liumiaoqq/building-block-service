namespace Web.Dto.TableFunctionList
{
    public class ExportFunctionTableEntityDto
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
        /// 是否外键表
        /// </summary>
        public bool IsForeignKey { get; set; }

        /// <summary>
        /// 对应外键的id
        /// </summary>
        public Guid? RelativeColumnPropId { get; set; }

        public ColumnPropDto RelativeColumnPropDto { get; set; }



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




    }
}
