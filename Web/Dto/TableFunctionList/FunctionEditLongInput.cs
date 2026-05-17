namespace Web.Dto.TableFunctionList
{
    public class FunctionEditLongInput : FunctionEditBase
    {
        public int Rows { get; set; }

        public int MaxLength { get; set; }

        public int MinLength { get; set; }

        public string ShowTooltip { get; set; }
        public static FunctionEditLongInput NormalBuilder()
        {

            return new FunctionEditLongInput
            {
                EditFormType = EditFormType.LongText,
                SpanValue = 24,
                IsRequired = true,
                IsClearable = true,
                EditDisabled = false,
                ValidRules = Enumerable.Range(0, 3).Select(x => new FunctionValidRule()).ToList(),

            };

        }
    }
}
