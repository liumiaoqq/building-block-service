
namespace Web.Dto.TableFunctionList
{
    /// <summary>
    /// 编辑数量范围
    /// </summary>
    public class FunctionEditQtyRange : FunctionEditBase
    {

    

        public int Maxlength { get; set; }

        public int Minlength { get; set; }

     

     
        public static FunctionEditQtyRange NormalBuilder()
        {

            return new FunctionEditQtyRange
            {
                EditFormType = EditFormType.QtyRange,
                SpanValue = 24,
                Maxlength = 1000000,
                Minlength=0,
                IsClearable=true,
                IsRequired = true,
                EditDisabled = false,
                ValidRules = Enumerable.Range(0, 3).Select(x => new FunctionValidRule()).ToList(),


            };

        }

    }
}
