using Newtonsoft.Json;

namespace Web.Dto.Rules
{
    public class EditRuleDetDto : FullBaseDto
    {


        public Guid EditRuleId { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// 执行顺序(从大到小)
        /// </summary>
        public int Sort { get; set; }


        /// <summary>
        /// 匹配正则表达式
        /// </summary>
        public string MatchRegular { get; set; }


        /// <summary>
        /// 是否匹配为外键
        /// </summary>
        public bool IsMatchForeignKey { get; set; }


        /// <summary>
        /// 匹配属性规则
        /// </summary>
        public EditRuleMatchType EditRuleMatchType { get; set; }

        public string EditRuleMatchTypeFormat => EditRuleMatchType.ToDescription();

        /// <summary>
        /// 匹配后处理
        /// </summary>
        public EditFormType EditFormType { get; set; }



        public string EditFormTypeFormat => EditFormType.ToDescription();


        /// <summary>
        /// 正则表达式
        /// </summary>
    
        public string RegularExpressions { get; set; }

        public List<RegularExpressionDto> RegularExpressionDtos { get; set; }=new List<RegularExpressionDto>();



        
    }
}
