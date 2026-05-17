
using Microsoft.CodeAnalysis;
using Web.Dto.AppUsers;
using Web.Dto.Plans;
using Web.Dto.Warehouses;

namespace Web
{
    public class IntegralRecordDto : FullBaseDto
    {
     
        
        [Description("标题")]
        public string Title { get; set; }

        [Description("用户")]
        public Guid UserId { get; set; }

        public AppUserDto UserDto { get; set; }


        [Description("积分")]
        public int Points { get; set; }

        [Description("类型")]
        public IntegralRecordType Type { get; set; }

        [Description("备注")]
        public string Remark { get; set; }

        [Description("来源单号")]
        public string SourceOrderNo { get; set; }

        [Description("剩余积分")]
        public int RemainingPoints { get; set; }

    }

}
