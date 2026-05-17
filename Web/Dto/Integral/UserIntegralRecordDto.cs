
using Microsoft.CodeAnalysis;
using Web.Dto.AppUsers;
using Web.Dto.Plans;
using Web.Dto.Warehouses;

namespace Web
{
    public class UserIntegralRecordDto
    {


        [Description("标题")]
        public string Title { get; set; }

        [Description("用户")]
        public Guid UserId { get; set; }



        [Description("积分")]
        public int Points { get; set; }

        [Description("类型")]
        public IntegralRecordType Type { get; set; }

        public String TypeFormat => Type.ToDescription();


        [Description("备注")]
        public string Remark { get; set; }

        [Description("来源单号")]
        public string SourceOrderNo { get; set; }



    }

}
