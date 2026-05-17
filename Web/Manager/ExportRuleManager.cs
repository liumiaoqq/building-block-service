using Web.Dto.Rules;

namespace Web.Manager
{
    public class ExportRuleManager
    {
        protected ISqlSugarClient _sqlSugarClient;

        private readonly IWebHostEnvironment _webHostEnvironment;
        public ExportRuleManager(ISqlSugarClient sqlSugarClient, IWebHostEnvironment webHostEnvironment)
        {
            _sqlSugarClient = sqlSugarClient;
            _webHostEnvironment = webHostEnvironment;
        }




        /// <summary>
        /// 查询
        /// </summary>
        public async Task<List<ExportRuleDto>> ListAsync(ExportRulePagedInput input)
        {

            var rules = await _sqlSugarClient.Queryable<ExportRule>()
                .WhereIF(input.Id.HasValue, x => x.Id == input.Id.Value)
                .Select<ExportRuleDto>().ToListAsync();
            var ruleIds = rules.Select(x => x.Id).ToList();
           var dets=await _sqlSugarClient.Queryable<ExportRuleDet>().Where(x => ruleIds.Contains(x.ExportRuleId)).Select<ExportRuleDetDto>().ToListAsync();

            foreach (var item in rules)
            {
                item.RuleDets=dets.Where(x=>x.ExportRuleId==item.Id).ToList();
            }
            return rules;
        }

    }
}
