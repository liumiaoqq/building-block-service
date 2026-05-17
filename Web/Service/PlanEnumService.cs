using Web.Dto.Plans;
using Web.Manager;

namespace Web.Service
{
    public class PlanEnumService
    {

        private readonly ISqlSugarClient _sqlSugarClient;

        private readonly ICurrentUser _currentUser;

        public readonly PlanManager _planManager;

        private readonly PlanEnumManager _planEnumManager;

        public PlanEnumService(ISqlSugarClient sqlSugarClient, ICurrentUser currentUser, PlanManager planManager, PlanEnumManager planEnumManager)
        {
            _sqlSugarClient = sqlSugarClient;
            _currentUser = currentUser;
            _planManager = planManager;
            _planEnumManager = planEnumManager;
        }

        public async Task<PagedReuslt<PlanEnumDto>> ListAsync(PlanEnumPagedInput input)
        {

            if (_currentUser.GetRoleType() == RoleType.用户)
            {
                input.UserId = _currentUser.GetUserId();
            }
            return await _planEnumManager.ListAsync(input);
        }

        public async Task<PlanEnumDto> GetAsync(IdInput<Guid> input)
        {
            return await _planEnumManager.GetAsync(input);
        }

        public async Task DeleteAsync(IdInput<Guid> input)
        {
            var userId = _currentUser.GetUserId();
            if (_currentUser.IsUser())
            {
                var count = await _sqlSugarClient.Queryable<PlanEnum>().CountAsync(x => x.CreatorId == userId && x.Id == input.Id);
                if (count == 0)
                {
                    throw new YouJuException("删除失败,该数据不属于你或者不存在");
                }
            }

            await _planEnumManager.DeleteAsync(input);
        }

        public async Task<PlanEnumDto> CreateOrEditAsync(PlanEnumDto input)
        {
            PlanEnumManager.ValidFiledCode(input.Code);
            PlanEnumManager.ValidFiledName(input.Name);


            var userId = _currentUser.GetUserId();
            if (_currentUser.IsUser())
            {
                await _planManager.CheckPlanIsExistAsync(input.PlanId, userId);
                var planEnumCount = await _planEnumManager.GetPlanEnumByPlanIdCountAsync(input.PlanId);
                if (planEnumCount > 20)
                {
                    throw new YouJuException("用户可设置的枚举类型不能超过15个,我相信你的项目不可能这么复杂的。");
                }
            }


            var count = await _sqlSugarClient.Queryable<PlanEnum>().Where(x => x.PlanId == input.PlanId && x.Code == input.Code)
                        .WhereIF(input.IsEidt, x => x.Id != input.Id)
                        .CountAsync();
            if (count > 0)
            {
                throw new YouJuException("请勿添加相同的配置");
            }
            if (input.EnumPropsList == null)
            {
                input.EnumPropsList = new List<EnumInfo>();
            }
            input.EnumProps = input.EnumPropsList.ToJson();

            PlanEnumManager.CheckEnumInfoList(input.EnumPropsList);
            return await _planEnumManager.CreateOrEditAsync(input);
        }

        /// <summary>
        /// 批量创建枚举项
        /// </summary>
        public async Task BatchSaveEnumProps(PlanEnumDto input)
        {
            var userId = _currentUser.GetUserId();

            var entity = await _sqlSugarClient.Queryable<PlanEnum>()
                .WhereIF(_currentUser.IsUser(), x => x.CreatorId == userId)
                .Where(x => x.Id == input.Id).FirstAsync();

            if (entity == null)
            {
                throw new YouJuException("没有找到枚举对象");
            }

            if (_currentUser.IsUser())
            {
                await _planManager.CheckPlanIsExistAsync(entity.PlanId, userId);
            }


            if (input.EnumPropsList == null)
            {
                input.EnumPropsList = new List<EnumInfo>();
            }
            entity.EnumProps = input.EnumPropsList.ToJson();

            PlanEnumManager.CheckEnumInfoList(input.EnumPropsList);

            await _sqlSugarClient.Updateable(entity).ExecuteCommandAsync();
        }

    }
}
