namespace Web.Dto.TableFunctionList
{
    public class FunctionSearchSelect
    {
        public SelectLabelType SelectLabelType { get; set; }

        public string Url { get; set; }

        public string ColumnName { get; set; }

        public string ColumnValue { get; set; }

        public string ColumnLabel { get; set; }


        public List<FuncitonSelecetOption> Options { get; set; }

        public static FunctionSearchSelect NormalBuilder()
        {

            return new FunctionSearchSelect
            {
                SelectLabelType =  SelectLabelType.动态,
                Options = Enumerable.Range(0, 3).Select(x => new FuncitonSelecetOption()).ToList()
            };

        }
    }
}
