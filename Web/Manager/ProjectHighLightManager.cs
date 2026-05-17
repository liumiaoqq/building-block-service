using System.Text;

namespace Web.Manager
{
    public class ProjectHighLightManager
    {
        protected ISqlSugarClient _sqlSugarClient;

        public ProjectHighLightManager(ISqlSugarClient sqlSugarClient)
        {
            _sqlSugarClient = sqlSugarClient;
        }


        public async Task<ProjectHighlight> CreateAsync(ProjectHighlight projectHighlight)
        {
            var result = await _sqlSugarClient.Insertable(projectHighlight).ExecuteReturnEntityAsync();
            return result;
        }

        public async Task<PagedReuslt<ProjectHighlightDto>> ListAsync(ProjectHighlightPagedInput input)
        {
            RefAsync<int> totalCount = 0;
            var result = await _sqlSugarClient.Queryable<ProjectHighlight>()
            .WhereIF(input.Name.IsNotNullOrNotWhiteSpace(), x => x.Name.Contains(input.Name))
            .WhereIF(input.TypeId.HasValue, x => x.TypeId == input.TypeId)
            .WhereIF(input.Sort.HasValue, x => x.Sort == input.Sort)
            .OrderByDescending(x => x.Sort)
            .Select<ProjectHighlightDto>()
            .ToPageListAsync(input.Page, input.Size, totalCount);

            //得到类型的id
            var typeIds = result.Select(x => x.TypeId).Distinct().ToList();
            //根据类型的id得到类型的名称
            var typeList = await _sqlSugarClient.Queryable<ProjectHighlightType>()
            .Where(x => typeIds.Contains(x.Id)).Select<ProjectHighlightTypeDto>().ToListAsync();

            //遍历结果，把类型的名称赋值给类型的名称
            foreach (var item in result)
            {
                item.TypeDto = typeList.FirstOrDefault(x => x.Id == item.TypeId);

            }



            return new PagedReuslt<ProjectHighlightDto>(result, totalCount.Value);
        }

        /// <summary>
        /// 用户端获取项目亮点列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public async Task<PagedReuslt<UserProjectHighLightDto>> UserListAsync(ProjectHighlightPagedInput input)
        {
            var result = await _sqlSugarClient.Queryable<ProjectHighlight>()
            .WhereIF(input.Name.IsNotNullOrNotWhiteSpace(), x => x.Name.Contains(input.Name))
            .WhereIF(input.TypeId.HasValue, x => x.TypeId == input.TypeId)
            .WhereIF(input.Sort.HasValue, x => x.Sort == input.Sort)
            .OrderByDescending(x => x.Sort)
            .Select<UserProjectHighLightDto>()
            .ToPageListAsync(input.Page, input.Size);

            //得到类型的id
            var typeIds = result.Select(x => x.TypeId).Distinct().ToList();
            //根据类型的id得到类型的名称
            var typeList = await _sqlSugarClient.Queryable<ProjectHighlightType>()
            .Where(x => typeIds.Contains(x.Id)).Select<ProjectHighlightTypeDto>().ToListAsync();

            //遍历结果，把类型的名称赋值给类型的名称
            foreach (var item in result)
            {
                item.TypeName = typeList.FirstOrDefault(x => x.Id == item.TypeId).Name;

            }
            return new PagedReuslt<UserProjectHighLightDto>(result, result.Count);
        }
    }
}
