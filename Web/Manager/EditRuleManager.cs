using Org.BouncyCastle.Utilities;
using Web.Dto.Rules;

namespace Web.Manager
{
    public class EditRuleManager
    {
        protected ISqlSugarClient _sqlSugarClient;

        private readonly IWebHostEnvironment _webHostEnvironment;
        public EditRuleManager(ISqlSugarClient sqlSugarClient, IWebHostEnvironment webHostEnvironment)
        {
            _sqlSugarClient = sqlSugarClient;
            _webHostEnvironment = webHostEnvironment;
        }




        /// <summary>
        /// 查询
        /// </summary>
        public async Task<List<EditRuleDto>> ListAsync(EditRulePagedInput input)
        {

            var rules = await _sqlSugarClient.Queryable<EditRule>()
                .WhereIF(input.Id.HasValue, x => x.Id == input.Id.Value)
                .Select<EditRuleDto>().ToListAsync();
            var ruleIds = rules.Select(x => x.Id).ToList();
            var dets = await _sqlSugarClient.Queryable<EditRuleDet>().Where(x => ruleIds.Contains(x.EditRuleId)).Select<EditRuleDetDto>().ToListAsync();

            foreach (var item in rules)
            {
                item.RuleDets = dets.Where(x => x.EditRuleId == item.Id).ToList();
                foreach (var ruleDet in item.RuleDets)
                {
                    ruleDet.RegularExpressionDtos = ruleDet.RegularExpressions.ToList<RegularExpressionDto>();               
                }
            }
            return rules;
        }

    }
}
