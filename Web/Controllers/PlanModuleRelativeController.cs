using Web.Dto.PlanModuleRelatives;
using Web.Dto.Plans;
using Web.Extensions;
using Web.Manager;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlanModuleRelativeController : YouJuController<PlanModuleRelative, PlanModuleRelativeDto, PlanModuleRelativePagedInput>
    {
        private readonly PlanModuleRelativeManager _planModuleRelativeManager;
        public PlanModuleRelativeController(IServiceProvider serviceProvider, PlanModuleRelativeManager planModuleRelativeManager) : base(serviceProvider)
        {
            _planModuleRelativeManager = planModuleRelativeManager;
        }

        #region 选择对应的模块
        [HttpPost("ListAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public override async Task<PagedReuslt<PlanModuleRelativeDto>> ListAsync(PlanModuleRelativePagedInput input)
        {

            RefAsync<int> totalCount = 0;

            var items = await SqlSugarClient.Queryable<PlanModuleRelative>()
               .WhereIF(input.PlanId.HasValue, x => x.PlanId == input.PlanId)
               .Select<PlanModuleRelativeDto>()
               .ToPageListAsync(input.Page, input.Size, totalCount);

            var moduleIds = items.Select(x => x.ModuleId).ToList();
            var planIds = items.Select(x => x.PlanId).ToList();


            var planDtos = await SqlSugarClient.Queryable<Plan>().Where(x => planIds.Contains(x.Id)).Select<PlanDto>().ToListAsync();
            var systemModuleDto = await SqlSugarClient.Queryable<SystemModule>().Where(x => moduleIds.Contains(x.Id)).Select<SystemModuleDto>().ToListAsync();

            foreach (var item in items)
            {
                item.PlanDto = planDtos.FirstOrDefault(x => x.Id == item.PlanId);
                item.SystemModuleDto = systemModuleDto.FirstOrDefault(x => x.Id == item.ModuleId);
            }
            return new PagedReuslt<PlanModuleRelativeDto>(items, totalCount.Value);
        }

        [HttpPost("BatchCreateOrEdit")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async Task BatchCreateOrEdit(BatchCreateOrEditPlanModuleRelativesInput input)
        {
            await _planModuleRelativeManager.BatchCreateOrEdit(input);
        }
        #endregion

        #region 预览

        [HttpPost("GetPlanRelativeModuleList")]
        public async Task<List<SystemModuleDto>> GetPlanRelativeModuleList(PlanModuleRelativePagedInput input)
        {
            return await _planModuleRelativeManager.GetPlanRelativeModuleList(input);
        }

        #endregion



    }
}
