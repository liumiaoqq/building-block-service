namespace Web.Dto.TableFunctionList
{
    public class FunctiontEditDateTimePicker : FunctionEditBase
    {
   

        public string DefaultTime1 { get; set; }

        public string DefaultTime2 { get; set; }

     


        public static FunctiontEditDateTimePicker NormalBuilder()
        {

            return new FunctiontEditDateTimePicker
            {
                EditFormType = EditFormType.DateTimePicker,
                SpanValue = 24,
                DefaultTime1= "08:00:00",
                DefaultTime2 = "20:00:00",
                IsRequired = true,
                EditDisabled = false,
                IsClearable = true,
                ValidRules = Enumerable.Range(0, 3).Select(x => new FunctionValidRule()).ToList(),
            };

        }
    }
}
