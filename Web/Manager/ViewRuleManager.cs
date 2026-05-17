using Org.BouncyCastle.Utilities;
using Web.Dto.Rules;

namespace Web.Manager
{
    public class ViewRuleManager
    {
        protected ISqlSugarClient _sqlSugarClient;

        private readonly IWebHostEnvironment _webHostEnvironment;
        public ViewRuleManager(ISqlSugarClient sqlSugarClient, IWebHostEnvironment webHostEnvironment)
        {
            _sqlSugarClient = sqlSugarClient;
            _webHostEnvironment = webHostEnvironment;
        }


        /// <summary>
        /// 查询
        /// </summary>
        public async Task<List<ViewRuleDto>> ListAsync(ViewRulePagedInput input)
        {

            var rules = await _sqlSugarClient.Queryable<ViewRule>()
                .WhereIF(input.Id.HasValue, x => x.Id == input.Id.Value)
                .Select<ViewRuleDto>().ToListAsync();
            var ruleIds = rules.Select(x => x.Id).ToList();
            var dets = await _sqlSugarClient.Queryable<ViewRuleDet>().Where(x => ruleIds.Contains(x.ViewRuleId)).Select<ViewRuleDetDto>().ToListAsync();

            foreach (var item in rules)
            {
                item.RuleDets = dets.Where(x => x.ViewRuleId == item.Id).ToList();
            }
            return rules;
        }

    }
}
