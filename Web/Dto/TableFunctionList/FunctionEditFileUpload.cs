namespace Web.Dto.TableFunctionList
{
    /// <summary>
    /// 编辑文件上传
    /// </summary>
    public class FunctionEditFileUpload: FunctionEditBase
    {



        public int Limit { get; set; }
 

        public static FunctionEditFileUpload NormalBuilder()
        {

            return new FunctionEditFileUpload
            {
                EditFormType = EditFormType.FileUpload,
                SpanValue = 24,
                Limit=8,
                IsRequired = true,
                IsClearable=true,
                EditDisabled = false,
                ValidRules = Enumerable.Range(0, 3).Select(x => new FunctionValidRule()).ToList(),
            };

        }
    }
}
