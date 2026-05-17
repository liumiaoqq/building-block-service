using Web.Dto.Plans;
using Web.Manager;
using System.Data;
using MediatR;
using Web.Events;

namespace Web.Service
{
    public class ProjectCaseService
    {
        private readonly PlanManager _planManager;

        private ISqlSugarClient _sqlSugarClient;

        private ICurrentUser _currentUser;

        private readonly IMediator _mediator;

        private readonly ProjectCaseManager _projectCaseManager;

        public ProjectCaseService(ISqlSugarClient sqlSugarClient, ICurrentUser currentUser, ProjectCaseManager projectCaseManager, IMediator mediator, PlanManager planManager = null)
        {
            _sqlSugarClient = sqlSugarClient;
            _currentUser = currentUser;
            _projectCaseManager = projectCaseManager;
            _mediator = mediator;
            _planManager = planManager;
        }

        public async Task<PagedReuslt<ProjectCaseDto>> ListAsync(ProjectCasePagedInput input)
        {

            return await _projectCaseManager.ListAsync(input);
        }

        public async Task<UserProjectCaseDto> UserGetAsync(ProjectCasePagedInput input)
        {
            return await _projectCaseManager.UserGetAsync(input);
        }

        public async Task CreateOrEditAsync(ProjectCaseDto input)
        {
            await _projectCaseManager.CreateOrEditAsync(input);
        }

        public async Task DeleteAsync(IdInput<Guid> input)
        {
            await _projectCaseManager.DeleteAsync(input);
        }


        public async Task<PagedReuslt<UserProjectCaseDto>> UserListAsync(ProjectCasePagedInput input)
        {
            return await _projectCaseManager.UserListAsync(input);
        }

        /// <summary>
        /// 完成选中模板的拷贝
        /// </summary>
        public async Task UserProjectCaseSubmitAsync(UserProjectCaseSubmitInput input)
        {
            if (input.SelectProjectCaseId == Guid.Empty)
            {
                throw new YouJuException("案例不存在");
            }
            //去数据库查询案例
            var projectCase = await _sqlSugarClient.Queryable<ProjectCase>().FirstAsync(x => x.Id == input.SelectProjectCaseId);
            if (projectCase == null)
            {
                throw new YouJuException("案例不存在");
            }

            if (projectCase.IsPublic == false)
            {
                throw new YouJuException("案例模板暂未公开");
            }
            if (projectCase.CaseType == ProjectCaseType.基础脚手架)
            {
                await UserProjectCaseTemplateSubmitAsync(input, projectCase);
            }
            else if (projectCase.CaseType == ProjectCaseType.完整脚手架)
            {
                await UserProjectCaseTeachingSystemSubmitAsync(input, projectCase);
            }
            else
            {
                throw new YouJuException("案例类型暂未开发");
            }


        }


        /// <summary>
        /// 用户选择基础架构进行
        /// </summary>
        /// <returns></returns>
        private async Task UserProjectCaseTemplateSubmitAsync(UserProjectCaseSubmitInput input, ProjectCase projectCase)
        {
            var userId = _currentUser.GetUserId();



            var count = await _planManager.GetPlanCountAsync(new PlanPagedInput() { UserId = userId });
            if (count > 20)
            {
                throw new YouJuException("每个用户只支持创建20个计划,如有需求可以联系字母哥!");
            }

            var plans = await _planManager.GetListAsync(new PlanPagedInput() { PlanId = projectCase.PlanId });
            var plan = plans.Items.FirstOrDefault();




            PlanManager.ValidFiledPlanName(input.PlanName);

            PlanManager.ValidFiledFileName(input.FileName);



            Plan clonePlan = null;
            try
            {
                _sqlSugarClient.Ado.BeginTran(IsolationLevel.ReadCommitted);

                //声明一个计划
                clonePlan = new Plan()
                {
                    Id = Guid.NewGuid(),
                    PlanName = input.PlanName,
                    FileName = input.FileName,
                    IsTemplete = false,
                    PlanType = plan.PlanType,
                    IsMiniProgram = plan.IsMiniProgram,
                    BackPort = plan.BackPort,
                    DatabaseConnection = plan.DatabaseConnection.Replace("数据库名称", input.FileName),
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
        /// 用户选择教学系统进行
        /// </summary>
        /// <returns></returns>
        private async Task UserProjectCaseTeachingSystemSubmitAsync(UserProjectCaseSubmitInput input, ProjectCase projectCase)
        {
            var userId = _currentUser.GetUserId();



            var count = await _planManager.GetPlanCountAsync(new PlanPagedInput() { UserId = userId });
            if (count > 20)
            {
                throw new YouJuException("每个用户只支持创建20个计划,如有需求可以联系字母哥!");
            }

            var plans = await _planManager.GetListAsync(new PlanPagedInput() { PlanId = projectCase.PlanId });
            var plan = plans.Items.FirstOrDefault();




            Plan clonePlan = null;
            try
            {
                _sqlSugarClient.Ado.BeginTran(IsolationLevel.ReadCommitted);

                //声明一个计划
                clonePlan = new Plan()
                {
                    Id = Guid.NewGuid(),
                    PlanName = plan.PlanName,
                    FileName = plan.FileName,
                    IsTemplete = false,
                    PlanType = plan.PlanType,
                    IsMiniProgram = plan.IsMiniProgram,
                    BackPort = plan.BackPort,
                    DatabaseConnection = plan.DatabaseConnection,
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
                    var index = 0;
                    foreach (var column in tableEntity.Columns.OrderBy(x => x.CreationTime))
                    {
                        index++;
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
                            CreationTime = DateTime.Now.AddSeconds(index),

                        };
                        if (column.ColumnPropType == ColumnPropType.枚举型)
                        {
                            var enumCode = plan.PlanEnums.FirstOrDefault(x => x.Id == column.PlanEnumId).Code;
                            cloneColumn.PlanEnumId = clonePlanEnums.First(x => x.Code == enumCode).Id;
                        }
                        cloneColumnProps.Add(cloneColumn);
                    }

                }



                List<TableNavigateRelative> cloneTableNavigateRelatives = new List<TableNavigateRelative>();

                foreach (var tableEntity in plan.TableEntitys)
                {
                    foreach (var tableNavigateRelative in tableEntity.TableNavigateRelatives)
                    {
                        var relativeTableCode = tableNavigateRelative.RelativeTable.Code;

                        //被关联表Code
                        var associationATableCode = tableNavigateRelative.AssociationATableEntity.Code;
                        var associationAColumnCode = tableNavigateRelative.AssociationAColumnPropDto.Code;


                        //得到克隆对应的表
                        var cloneTableEntity = cloneTableEntitys.FirstOrDefault(x => x.Code == relativeTableCode).Id;
                        var associationATableId = cloneTableEntitys.FirstOrDefault(x => x.Code == associationATableCode).Id;
                        var associationAColumnId = cloneColumnProps.FirstOrDefault(x => x.TableEntityId == cloneTableEntity
                        && x.Code == associationAColumnCode).Id;


                        //得到被关联的表
                        var cloneTableNavigateRelative = new TableNavigateRelative()
                        {
                            Id = Guid.NewGuid(),
                            TableNavigateType = tableNavigateRelative.TableNavigateType,
                            RelativeTableId = cloneTableEntity,
                            AssociationATableId = associationATableId,
                            AssociationAColumnId = associationAColumnId,
                            AssociationBTableId = null,
                            AssociationBColumnId = null,
                        };
                        cloneTableNavigateRelatives.Add(cloneTableNavigateRelative);
                    }
                }




                await _sqlSugarClient.Insertable(clonePlan).ExecuteCommandAsync();
                await _sqlSugarClient.Insertable(planModuleRelatives).ExecuteCommandAsync();
                await _sqlSugarClient.Insertable(clonePlanEnums).ExecuteCommandAsync();
                await _sqlSugarClient.Insertable(cloneTableEntitys).ExecuteCommandAsync();
                await _sqlSugarClient.Insertable(cloneColumnProps).ExecuteCommandAsync();
                //插入关系
                await _sqlSugarClient.Insertable(cloneTableNavigateRelatives).ExecuteCommandAsync();


                //在这里发送事件去调用RuleCenterService里面的逻辑
                _sqlSugarClient.Ado.CommitTran();

                // 在事务完成后发布通知
                await _mediator.Publish(new PlanCreatedNotification
                {
                    PlanId = clonePlan.Id,
                    CaseType = (ProjectCaseType)projectCase.CaseType
                });
            }
            catch (Exception ex)
            {
                _sqlSugarClient.Ado.RollbackTran();
                throw ex;
            }
        }
        public async Task AddViewCount(IdInput<Guid> input)
        {
            var projectCase = await _sqlSugarClient.Queryable<ProjectCase>().FirstAsync(x => x.Id == input.Id);
            if (projectCase != null)
            {
                projectCase.ViewCount++;
                await _sqlSugarClient.Updateable(projectCase).ExecuteCommandAsync();

            }

        }

    }



}
