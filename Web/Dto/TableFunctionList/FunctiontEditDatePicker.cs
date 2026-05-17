namespace Web.Dto.TableFunctionList
{
    public class FunctiontEditDatePicker : FunctionEditBase
    {
    

        public string DefaultTime1 { get; set; }

        public string DefaultTime2 { get; set; }

     

        public static FunctiontEditDatePicker NormalBuilder()
        {
            return new FunctiontEditDatePicker
            {
                EditFormType = EditFormType.DatePicker,
                SpanValue = 24,
                DefaultTime1 = "08:00:00",
                DefaultTime2 = "20:00:00",
                IsRequired = true,
                EditDisabled = false,
                IsClearable = true,
                ValidRules = Enumerable.Range(0, 3).Select(x => new FunctionValidRule()).ToList(),
            };
        }
    }
}
