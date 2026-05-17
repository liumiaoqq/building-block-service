using Microsoft.CodeAnalysis;
using Web.Dto.Plans;
using Web.Dto.Warehouses;

namespace Web
{
    public class UserProjectServiceBusinessDto 
    {
      
        [Description("项目服务业务ID")]
        public Guid Id { get; set; }
     
        [Description("标题")]
        public string Title { get; set; }

        [Description("封面")]
        public string Cover { get; set; }

        // [Description("内容")]
        // public string Content { get; set; }


      
        // [Description("价格")]
        // public decimal Price { get; set; }

        // [Description("积分")]
        // public int Points { get; set; }

   

        // [Description("提示")]
        // public string Tip { get; set; }


    }

}
