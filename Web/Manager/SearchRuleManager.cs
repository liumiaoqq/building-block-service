using Org.BouncyCastle.Utilities;
using Web.Dto.Rules;

namespace Web.Manager
{
    public class SearchRuleManager
    {
        protected ISqlSugarClient _sqlSugarClient;

        private readonly IWebHostEnvironment _webHostEnvironment;
        public SearchRuleManager(ISqlSugarClient sqlSugarClient, IWebHostEnvironment webHostEnvironment)
        {
            _sqlSugarClient = sqlSugarClient;
            _webHostEnvironment = webHostEnvironment;
        }




        /// <summary>
        /// 查询
        /// </summary>
        public async Task<List<SearchRuleDto>> ListAsync(SearchRulePagedInput input)
        {

            var rules = await _sqlSugarClient.Queryable<SearchRule>()
                .WhereIF(input.Id.HasValue, x => x.Id == input.Id.Value)
                .Select<SearchRuleDto>().ToListAsync();
            var ruleIds = rules.Select(x => x.Id).ToList();
            var dets = await _sqlSugarClient.Queryable<SearchRuleDet>().Where(x => ruleIds.Contains(x.SearchRuleId)).Select<SearchRuleDetDto>().ToListAsync();

            foreach (var item in rules)
            {
                item.RuleDets = dets.Where(x => x.SearchRuleId == item.Id).ToList();
            }
            return rules;
        }

    }
}
