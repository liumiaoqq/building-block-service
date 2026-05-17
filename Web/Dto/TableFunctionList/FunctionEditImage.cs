namespace Web.Dto.TableFunctionList
{
    public class FunctionEditImage: FunctionEditBase
    {
    

        public int Limit { get; set; }

  


     

        public static FunctionEditImage NormalBuilder()
        {

            return new FunctionEditImage
            {
                EditFormType = EditFormType.Image,
                SpanValue = 24,
                Limit=1,
                IsRequired = true,
                IsClearable=true,
                EditDisabled = false,
                ValidRules = Enumerable.Range(0, 3).Select(x => new FunctionValidRule()).ToList(),

            };

        }
    }
}
