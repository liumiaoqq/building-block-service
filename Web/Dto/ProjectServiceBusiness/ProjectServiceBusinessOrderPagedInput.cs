namespace Web
{
    public class ProjectServiceBusinessOrderPagedInput : PagedBaseInput
    {
        [Description("订单编号")]
        public string No { get; set; }

        [Description("支付编号")]
        public string PayNo { get; set; }

        [Description("项目服务业务ID")]
        public Guid? ProjectServiceBusinessId { get; set; }

        [Description("项目服务业务订单状态")]
        public ProjectServiceBusinessOrderStatus? OrderStatus { get; set; }

    }
}
