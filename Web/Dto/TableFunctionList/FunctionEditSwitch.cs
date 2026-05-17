namespace Web.Dto.TableFunctionList
{
    public class FunctionEditSwitch : FunctionEditBase
    {
     

        public static FunctionEditSwitch NormalBuilder()
        {

            return new FunctionEditSwitch
            {
                EditFormType = EditFormType.Switch,
                SpanValue = 24,
                IsRequired = true,
                IsClearable = true,
                EditDisabled = false,
                ValidRules = Enumerable.Range(0, 3).Select(x => new FunctionValidRule()).ToList(),

            };

        }
    }
}
