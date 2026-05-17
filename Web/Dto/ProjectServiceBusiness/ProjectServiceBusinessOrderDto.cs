using Microsoft.CodeAnalysis;
using Web.Dto.Plans;
using Web.Dto.Warehouses;

namespace Web
{
    public class ProjectServiceBusinessOrderDto : FullBaseDto
    {

        [Description("订单编号")]
        public string No { get; set; }

        [Description("支付编号")]
        public string PayNo { get; set; }

        [Description("是否使用积分")]
        public bool IsUsePoints { get; set; }

        [Description("用户ID")]
        public Guid UserId { get; set; }

        [Description("项目服务业务ID")]
        public Guid ProjectServiceBusinessId { get; set; }


        [Description("项目服务业务")]
        public ProjectServiceBusinessDto ProjectServiceBusiness { get; set; }


        [Description("价格")]
        public decimal UsePrice { get; set; }

        [Description("积分")]
        public int UsePoints { get; set; }

        [Description("数量")]
        public int Qty { get; set; }

        [Description("项目服务业务订单状态")]
        public ProjectServiceBusinessOrderStatus OrderStatus { get; set; }



        [Description("项目服务业务订单状态")]
        public string OrderStatusFormat => OrderStatus.ToDescription();
        [Description("支付类型")]
        public PayType PayType { get; set; }

        [Description("支付类型")]
        public string PayTypeFormat => PayType.ToDescription();


    }

}
