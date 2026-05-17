using Web.Dto.PlanModuleRelatives;
using Web.Dto.Plans;

namespace Web.Manager
{
    public class PlanModuleRelativeManager
    {
        protected ISqlSugarClient _sqlSugarClient;

        public PlanModuleRelativeManager(ISqlSugarClient sqlSugarClient)
        {
            _sqlSugarClient = sqlSugarClient;
        }


        /// <summary>
        /// 批量创建或者修改
        /// </summary>
        public async Task BatchCreateOrEdit(BatchCreateOrEditPlanModuleRelativesInput input)
        {
            var inputPlanIds = input.ModuleIds;

            //得到所有计划的关联
            var planModuleRelatives = await _sqlSugarClient.Queryable<PlanModuleRelative>().Where(x => x.PlanId == input.PlanId).ToListAsync();
            var moduleIds = planModuleRelatives.Select(x => x.ModuleId).Distinct().ToList();

            //得到2个的交际 不需要动
            var intersectPlanIds = inputPlanIds.Intersect(moduleIds);
            //传入的排除交际就是需要新增的
            var newAddPlanIds = inputPlanIds.Where(x => !intersectPlanIds.Contains(x)).ToList();
            //数据库原始的排除交集就是需要删除的
            var deletePlanIds = moduleIds.Where(x => !intersectPlanIds.Contains(x)).ToList();

            if (newAddPlanIds.Count > 0)
            {
                await _sqlSugarClient.Insertable(newAddPlanIds.Select(x => new PlanModuleRelative()
                {
                    ModuleId = x,
                    PlanId = input.PlanId,

                }).ToList()).ExecuteCommandAsync();

            }
            if (deletePlanIds.Count > 0)
            {
                await _sqlSugarClient.Deleteable<PlanModuleRelative>().Where(x => x.PlanId == input.PlanId && deletePlanIds.Contains(x.ModuleId)).ExecuteCommandAsync();
            }

        }


        #region 预览模块

        public async Task<List<SystemModuleDto>> GetPlanRelativeModuleList(PlanModuleRelativePagedInput input)
        {
            //得到我关联的所有模块
            var relativeModuleIds = await _sqlSugarClient.Queryable<PlanModuleRelative>().Where(x => x.PlanId == input.PlanId).Select(x => x.ModuleId).ToListAsync();
            var systemModuleDtos = await _sqlSugarClient.Queryable<SystemModule>().Where(x => relativeModuleIds.Contains(x.Id)).Select<SystemModuleDto>().ToListAsync();
            return systemModuleDtos;
        }

        #endregion


    }
}
