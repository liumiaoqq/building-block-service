namespace Web.Dto.TableEntitys
{
    /// <summary>
    /// 功能清单
    /// </summary>
    public class FunctionDto
    {   
        public TableSettingDto TableSettingDto { get; set; }
    

        public FunctionDto() {
            TableSettingDto = new TableSettingDto();
        }
      
    }
}
