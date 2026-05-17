using Microsoft.CodeAnalysis;
using Web.Dto.Plans;
using Web.Dto.Warehouses;

namespace Web
{
    public class UserProjectHighlightTypeDto 
    {
        [Description("类型名称")]
        public string Name { get; set; }

        [Description("类型Id")]
        public Guid Id { get; set; }
      

    }

}
