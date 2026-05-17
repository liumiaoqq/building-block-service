using Org.BouncyCastle.Utilities;
using Web.Dto.Plans;
using Web.Dto.Rules;
using Web.Dto.Warehouses;
using Web.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;

namespace Web.Manager
{
    public class ProjectCaseManager
    {
        protected ISqlSugarClient _sqlSugarClient;

        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProjectCaseManager(ISqlSugarClient sqlSugarClient, IWebHostEnvironment webHostEnvironment)
        {
            _sqlSugarClient = sqlSugarClient;
            _webHostEnvironment = webHostEnvironment;
        }



        /// <summary>
        /// 用户查询
        /// </summary>
        public async Task<PagedReuslt<UserProjectCaseDto>> UserListAsync(ProjectCasePagedInput input)
        {
            RefAsync<int> totalCount = 0;
            var items = await _sqlSugarClient.Queryable<ProjectCase>()
                .WhereIF(input.CaseName.IsNotNullOrNotWhiteSpace(), x => x.CaseName.Contains(input.CaseName))
                .WhereIF(input.Id.HasValue, x => x.Id == input.Id.Value)
                .Where(x => x.IsPublic == true)
                .WhereIF(input.CaseType.HasValue, x => x.CaseType == input.CaseType.Value)
                .WhereIF(input.KeyWord.IsNotNullOrNotWhiteSpace(), x => x.CaseName.Contains(input.KeyWord))
                   .OrderByDescending(x => x.Sort)
                .Select<UserProjectCaseDto>()
                .ToPageListAsync(input.Page, input.Size, totalCount);

            return new PagedReuslt<UserProjectCaseDto>(items, totalCount);
        }


        /// <summary>
        /// 用户单个查询
        /// </summary>
        public async Task<UserProjectCaseDto> UserGetAsync(ProjectCasePagedInput input)
        {
            var item = await _sqlSugarClient.Queryable<ProjectCase>()
                .WhereIF(input.CaseName.IsNotNullOrNotWhiteSpace(), x => x.CaseName.Contains(input.CaseName))
                .WhereIF(input.Id.HasValue, x => x.Id == input.Id.Value)
                .Where(x => x.IsPublic == true)
                .WhereIF(input.CaseType.HasValue, x => x.CaseType == input.CaseType.Value)
                .WhereIF(input.KeyWord.IsNotNullOrNotWhiteSpace(), x => x.CaseName.Contains(input.KeyWord))
                   .OrderByDescending(x => x.CreationTime)
                .Select<UserProjectCaseDto>()
                .FirstAsync();

            return item;
        }

        /// <summary>
        /// 查询
        /// </summary>
        public async Task<PagedReuslt<ProjectCaseDto>> ListAsync(ProjectCasePagedInput input)
        {
            RefAsync<int> totalCount = 0;
            var items = await _sqlSugarClient.Queryable<ProjectCase>()
                .WhereIF(input.CaseName.IsNotNullOrNotWhiteSpace(), x => x.CaseName.Contains(input.CaseName))
                .WhereIF(input.Id.HasValue, x => x.Id == input.Id.Value)
                .WhereIF(input.CaseType.HasValue, x => x.CaseType == input.CaseType.Value)
                .WhereIF(input.PlanId.HasValue, x => x.PlanId == input.PlanId.Value)
                .WhereIF(input.KeyWord.IsNotNullOrNotWhiteSpace(), x => x.CaseName.Contains(input.KeyWord) || x.CaseDescription.Contains(input.KeyWord) || x.Content.Contains(input.KeyWord))
                  .OrderByDescending(x => x.Sort)
                  .OrderByDescending(x => x.CreationTime)
                .Select<ProjectCaseDto>()
                .ToPageListAsync(input.Page, input.Size, totalCount);
            //得到仓库ids
            var warehouseIds = items.Where(x => x.WarehouseId.HasValue).Select(x => x.WarehouseId.Value).Distinct().ToList();
            //查询出对应的仓库集合
            var warehouseDtos = await _sqlSugarClient.Queryable<Warehouse>().Where(x => warehouseIds.Contains(x.Id)).Select<WarehouseDto>().ToListAsync();
            var planIds = items.Where(x => x.PlanId.HasValue).Select(x => x.PlanId.Value).Distinct().ToList();
            var planDtos = await _sqlSugarClient.Queryable<Plan>().Where(x => planIds.Contains(x.Id)).Select<PlanDto>().ToListAsync();
            foreach (var item in items)
            {
                item.WarehouseDto = warehouseDtos.FirstOrDefault(x => x.Id == item.WarehouseId);
                item.PlanDto = planDtos.FirstOrDefault(x => x.Id == item.PlanId);
            }

            // 获取案例模块数据
            var caseIds = items.Select(x => x.Id).ToList();


            return new PagedReuslt<ProjectCaseDto>(items, totalCount);
        }

        public virtual async Task DeleteAsync(IdInput<Guid> input)
        {
            await _sqlSugarClient.Updateable<ProjectCase>().Where(it => it.Id == input.Id).SetColumns(x => x.IsDeleted == true).ExecuteCommandAsync();
            // 同步删除案例相关的模块数据
            await _sqlSugarClient.Updateable<ProjectCaseModule>().Where(it => it.ProjectCaseId == input.Id).SetColumns(x => x.IsDeleted == true).ExecuteCommandAsync();
        }


        public virtual async Task CreateOrEditAsync(ProjectCaseDto input)
        {


            if (input.Id.HasValue == false)
            {
                input.Id = Guid.NewGuid();
                var entity = input.Clone<ProjectCaseDto, ProjectCase>();
                await _sqlSugarClient.Insertable(entity).ExecuteCommandAsync();
                input.Id = entity.Id;
            }
            else
            {
                var entity = input.Clone<ProjectCaseDto, ProjectCase>();
                await _sqlSugarClient.Updateable(entity).ExecuteCommandAsync();

            }

        }
    }
}
