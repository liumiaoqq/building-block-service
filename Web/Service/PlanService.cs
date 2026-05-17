using System.Data;
using Web.Dto.Plans;
using Web.Manager;

namespace Web.Service
{
    public class PlanService
    {
        private readonly PlanManager _planManager;

        private ISqlSugarClient _sqlSugarClient;

        private ICurrentUser _currentUser;

        public PlanService(PlanManager planManager, ISqlSugarClient sqlSugarClient, ICurrentUser currentUser)
        {
            _planManager = planManager;
            _sqlSugarClient = sqlSugarClient;
            _currentUser = currentUser;
        }

        public async Task<PagedReuslt<PlanDto>> ListAsync(PlanPagedInput input)
        {
            var userId = _currentUser.GetUserId();
            if (_currentUser.IsUser())
            {
                input.UserId = userId;
            }
            return await _planManager.ListAsync(input);
        }


        /// <summary>
        /// 完成选中模板的拷贝
        /// </summary>
        public async Task UserPlanTempleteSubmitAsync(UserPlanTempleteSubmitInput input)
        {

            var userId = _currentUser.GetUserId();
            if (input.SelectPlanId == Guid.Empty)
            {
                throw new YouJuException("计划模板不存在");
            }
            PlanManager.ValidFiledPlanName(input.PlanName);

            PlanManager.ValidFiledFileName(input.FileName);


            try
            {
                _sqlSugarClient.Ado.BeginTran(IsolationLevel.ReadCommitted);

                var count = await _planManager.GetPlanCountAsync(new PlanPagedInput() { UserId = userId });
                if (count > 5)
                {
                    throw new YouJuException("每个用户只支持创建5个计划,如有商业计划可以联系字母哥!");
                }

                var plans = await _planManager.GetListAsync(new PlanPagedInput() { PlanId = input.SelectPlanId, IsTemplete = true });
                var plan = plans.Items.FirstOrDefault();
                //声明一个计划
                var clonePlan = new Plan()
                {
                    Id = Guid.NewGuid(),
                    PlanName = input.PlanName,
                    FileName = input.FileName,
                    IsTemplete = false,
                    PlanType = plan.PlanType,
                    IsMiniProgram = plan.IsMiniProgram,
                    BackPort = plan.BackPort,
                    DatabaseConnection = plan.DatabaseConnection.Replace("Templete", input.FileName),
                    SqlTempleteId = plan.SqlTempleteId,
                };


                var planModuleRelatives = new List<PlanModuleRelative>();
                //完成关联模块的设置
                foreach (var module in plan.SystemModules)
                {
                    planModuleRelatives.Add(new PlanModuleRelative()
                    {
                        Id = Guid.NewGuid(),
                        PlanId = clonePlan.Id,
                        ModuleId = module.Id.Value,
                    });
                }


                //循环好所有的枚举
                var clonePlanEnums = new List<PlanEnum>();
                foreach (var planEnum in plan.PlanEnums)
                {
                    clonePlanEnums.Add(new PlanEnum()
                    {
                        Id = Guid.NewGuid(),
                        Code = planEnum.Code,
                        EnumProps = planEnum.EnumProps,
                        Name = planEnum.Name,
                        PlanId = clonePlan.Id,
                    });
                }

                //循环表结构
                var cloneTableEntitys = new List<TableEntity>();
                var cloneColumnProps = new List<ColumnProp>();
                foreach (var tableEntity in plan.TableEntitys)
                {
                    var cloneTableEntity = new TableEntity()
                    {
                        Id = Guid.NewGuid(),
                        Code = tableEntity.Code,
                        IsExtra = tableEntity.IsExtra,

                        IsOpen = false,
                        PlanId = clonePlan.Id,
                        IsBuiltin = tableEntity.IsBuiltin,

                        Name = tableEntity.Name,
                    };
                    cloneTableEntitys.Add(cloneTableEntity);
                    foreach (var column in tableEntity.Columns)
                    {
                        var cloneColumn = new ColumnProp()
                        {
                            Id = Guid.NewGuid(),
                            TableEntityId = cloneTableEntity.Id,
                            Display = column.Name,
                            IsNull = column.IsNull,
                            Length = column.Length,
                            Name = column.Name,
                            ColumnPropType = column.ColumnPropType,
                            Code = column.Code,
                        };
                        if (column.ColumnPropType == ColumnPropType.枚举型)
                        {
                            var enumCode = plan.PlanEnums.FirstOrDefault(x => x.Id == column.PlanEnumId).Code;
                            cloneColumn.PlanEnumId = clonePlanEnums.First(x => x.Code == enumCode).Id;
                        }
                        cloneColumnProps.Add(cloneColumn);
                    }
                }


                await _sqlSugarClient.Insertable(clonePlan).ExecuteCommandAsync();
                await _sqlSugarClient.Insertable(planModuleRelatives).ExecuteCommandAsync();
                await _sqlSugarClient.Insertable(clonePlanEnums).ExecuteCommandAsync();
                await _sqlSugarClient.Insertable(cloneTableEntitys).ExecuteCommandAsync();
                await _sqlSugarClient.Insertable(cloneColumnProps).ExecuteCommandAsync();
                _sqlSugarClient.Ado.CommitTran();
            }
            catch (Exception ex)
            {
                _sqlSugarClient.Ado.RollbackTran();
                throw ex;
            }
        }
        /// <summary>
        /// 维护方法
        /// </summary>
        public async Task ResetUserPlanTempleteSubmitAsync()
        {

            var templetePlanId = Guid.Parse("D63D92B9-C6EB-4320-945F-2469EBE15AF4");
            var plans = await _planManager.GetListAsync(new PlanPagedInput() { PlanId = templetePlanId, IsTemplete = true });
            var plan = plans.Items.FirstOrDefault();

            var planIds = _sqlSugarClient.Queryable<Plan>().Where(x => x.WarehouseId == null).Select(x => x.Id).ToList();

            var deleteRelatives = _sqlSugarClient.Queryable<PlanModuleRelative>().Where(x => planIds.Contains(x.PlanId)).ToList();
            if (deleteRelatives.Count > 0)
            {
                foreach (var item in deleteRelatives)
                {
                    item.IsDeleted = true;

                }
                _sqlSugarClient.Updateable(deleteRelatives).ExecuteCommand();
            }

            var planModuleRelatives = new List<PlanModuleRelative>();
            foreach (var planId in planIds)
            {
                //完成关联模块的设置
                foreach (var module in plan.SystemModules)
                {
                    planModuleRelatives.Add(new PlanModuleRelative()
                    {
                        Id = Guid.NewGuid(),
                        PlanId = planId,
                        ModuleId = module.Id.Value,
                    });
                }
            }
            _sqlSugarClient.Insertable(planModuleRelatives).ExecuteCommand();


        }
    }
}
