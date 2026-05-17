namespace Web.Dto.TableFunctionList
{

    /// <summary>
    /// 编辑下拉框
    /// </summary>
    public class FunctionEditSelect : FunctionEditBase
    {

     


        public SelectLabelType SelectLabelType { get; set; }

        public string Url { get; set; }

        public string ColumnName { get; set; }

        public string ColumnValue { get; set; }

        public string ColumnLabel { get; set; }


        public List<FunctionValidRule> ValidRules { get; set; } 

        public List<FuncitonSelecetOption> Options { get; set; } 

        public static FunctionEditSelect NormalBuilder()
        {

            return new FunctionEditSelect
            {
                EditFormType = EditFormType.SigleSelect,
                SpanValue = 24,

                IsRequired = true,
                EditDisabled = false,
                IsClearable=true,
                ValidRules = Enumerable.Range(0, 3).Select(x => new FunctionValidRule()).ToList(),
                Options = Enumerable.Range(0, 3).Select(x => new FuncitonSelecetOption()).ToList()
            };

        }


     
    }
}
