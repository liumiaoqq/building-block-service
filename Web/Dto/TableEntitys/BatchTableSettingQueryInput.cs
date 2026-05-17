
using Newtonsoft.Json;
using Web.Dto.TableFunctionList;
using Web.Tables;

namespace Web.Dto.TableEntitys
{
    public class BatchTableSettingQueryInput : FullBaseDto
    {
        public Guid PlanId { get; set; }
        public Guid TableEntityId { get; set; }


        public BatchTableSettingQueryInput()
        {


        }
    }
}
