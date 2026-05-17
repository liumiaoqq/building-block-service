namespace Web.Dto.TableFunctionList
{
    /// <summary>
    /// 编辑富文本
    /// </summary>
    public class FunctionEditRichText : FunctionEditBase
    {

  

        public static FunctionEditRichText NormalBuilder()
        {

            return new FunctionEditRichText
            {
                EditFormType = EditFormType.RichText,
                SpanValue = 24,
                IsRequired = true,
                IsClearable=true,
                EditDisabled = false,
                ValidRules = Enumerable.Range(0, 3).Select(x => new FunctionValidRule()).ToList(),

            };

        }
    }
}
