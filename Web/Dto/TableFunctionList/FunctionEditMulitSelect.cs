namespace Web.Dto.TableFunctionList
{
    public class FunctionEditMulitSelect : FunctionEditBase
    {
     

        public List<FuncitonSelecetOption> Options { get; set; }

        public static FunctionEditMulitSelect NormalBuilder()
        {

            return new FunctionEditMulitSelect
            {
                EditFormType = EditFormType.MulitSelect,
                SpanValue = 24,

                IsRequired = true,
                EditDisabled = false,
                IsClearable = true,
                ValidRules = Enumerable.Range(0, 3).Select(x => new FunctionValidRule()).ToList(),
                Options = Enumerable.Range(0, 3).Select(x => new FuncitonSelecetOption()).ToList()
            };

        }

    }
}
