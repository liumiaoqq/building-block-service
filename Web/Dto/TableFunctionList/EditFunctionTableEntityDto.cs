namespace Web.Dto.TableFunctionList
{
    public class EditFunctionTableEntityDto
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
        /// 列的名称
        /// </summary>
        public string ShowColumnName { get; set; }

        /// <summary>
        /// 显示列的编码
        /// </summary>
        public string ShowColumnCode { get; set; }
            
        /// <summary>
        /// 对应的配置值
        /// </summary>
        public object Setting { get; set; }


        /// <summary>
        /// 对应的类型
        /// </summary>
        public EditFormType EditFormType { get; set; }


        public int ShowSort { get; set; }



        public string EditFormTypeFormat => EditFormType.ToDescription();
        public string EditFormTypeValue => EditFormType.ToString();
    }
}
