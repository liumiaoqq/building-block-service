namespace Web.Dto.TableFunctionList
{
    /// <summary>
    /// 编辑框
    /// </summary>
    public class FunctionEditInput : FunctionEditBase
    {

   
        public int MaxLength { get; set; }

        public int MinLength { get; set; }

        public string ShowTooltip { get; set; }

  

      

        public static FunctionEditInput NormalBuilder()
        {

            return new FunctionEditInput
            {
                EditFormType = EditFormType.Input,
                SpanValue = 24,
                IsRequired = true,

                IsClearable =true,
                EditDisabled = false,
                ValidRules = Enumerable.Range(0, 3).Select(x => new FunctionValidRule()).ToList(),
            };

        }
    }
}
