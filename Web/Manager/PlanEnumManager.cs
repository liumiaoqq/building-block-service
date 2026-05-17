using MathNet.Numerics.Statistics.Mcmc;
using Web.Dto.Plans;
using System.Text.RegularExpressions;

namespace Web.Manager
{
    public class PlanEnumManager
    {
        protected ISqlSugarClient _sqlSugarClient;

        public PlanEnumManager(ISqlSugarClient sqlSugarClient)
        {
            _sqlSugarClient = sqlSugarClient;
        }




        public async Task<PagedReuslt<PlanEnumDto>> ListAsync(PlanEnumPagedInput input)
        {
            RefAsync<int> totalCount = 0;
            var items = await _sqlSugarClient.Queryable<PlanEnum>()
                .WhereIF(input.Id.HasValue, x => x.Id == input.Id)
                .WhereIF(input.UserId.HasValue, x => x.CreatorId == input.UserId)
                .WhereIF(input.PlanId.HasValue, x => x.PlanId == input.PlanId)
                .Select<PlanEnumDto>()
                 .ToPageListAsync(input.Page, input.Size, totalCount);
            foreach (var item in items)
            {
                item.EnumPropsList = item.EnumProps.ToList<EnumInfo>();
            }
            return new PagedReuslt<PlanEnumDto>(items, totalCount.Value);
        }

        public async Task<PlanEnumDto> GetAsync(IdInput<Guid> input)
        {
            var item = await _sqlSugarClient.Queryable<PlanEnum>().Where(x => x.Id == input.Id).Select<PlanEnumDto>().FirstAsync();
            return item ?? new PlanEnumDto();
        }


        public async Task<List<PlanEnumDto>> GetPlanEnumByIdsAsync(List<Guid> ids)
        {

            if (ids.Count == 0) return new List<PlanEnumDto>();
            var items = await _sqlSugarClient.Queryable<PlanEnum>()
                            .Where(x => ids.Contains(x.Id))
                            .Select<PlanEnumDto>()
                            .ToListAsync();
            return items;
        }
        /// <summary>
        /// 根据计划获取枚举个数
        /// </summary>
        public async Task<long> GetPlanEnumByPlanIdCountAsync(Guid planId)
        {
            var count = await _sqlSugarClient.Queryable<PlanEnum>()
                            .Where(x => x.PlanId == planId)
                            .CountAsync();
            return count;
        }

        public async Task<PlanEnumDto> CreateOrEditAsync(PlanEnumDto input)
        {
            //判断输入的枚举是否是Enum结尾
            if (!input.Code.EndsWith("Enum"))
            {
                throw new YouJuException("枚举名称必须以Enum结尾");
            }
            if ("Enum".Equals(input.Code))
            {
                throw new YouJuException("枚举编码不能为Enum");
            }
            //枚举只能是字母
            if (!Regex.IsMatch(input.Code, "^[a-zA-Z]+$"))
            {
                throw new YouJuException("枚举值只能是字母,请勿输入数字或则其他特殊符号");
            }

            input.GuidNullToEmpty();
            var entity = await _sqlSugarClient.Queryable<PlanEnum>().FirstAsync(x => x.Id == input.Id);
            if (entity is null)
            {
                input.Id = Guid.Empty;
                entity = input.Clone<PlanEnumDto, PlanEnum>();


                entity = await _sqlSugarClient.Insertable(entity).ExecuteReturnEntityAsync();
            }
            else
            {

                entity.Name = input.Name;
                entity.Code = input.Code;


                await _sqlSugarClient.Updateable(entity).ExecuteCommandAsync();
            }
            return entity.Clone<PlanEnum, PlanEnumDto>();
        }

        /// <summary>
        /// AI自动解析枚举保存
        /// </summary>
        public async Task AiFixEnumSaveAsync(AiFixEnumSaveDto input)
        {
            var planEnumList = await _sqlSugarClient.Queryable<PlanEnum>().Where(x => x.PlanId == input.PlanId).ToListAsync();

            //需要修改的planEnumList
            var updatePlanEnumList = new List<PlanEnum>();

            var newPlanEnumList = new List<PlanEnum>();


            foreach (var item in input.AiFixEnumResultDtos)
            {
                if (!item.Code.EndsWith("Enum"))
                {
                    item.Code = item.Code + "Enum";
                }
                //判断系统是否存在了
                var planEnum = planEnumList.Where(x => x.Name == item.Name && x.Code == item.Code).FirstOrDefault();
                if (planEnum != null)
                {

                    planEnum.EnumProps = item.EnumPropsList.Select(x => new EnumInfo()
                    {
                        Name = x.Name,
                        Code = x.Name,
                        Value = x.Value
                    }).ToList().ToJson();

                    updatePlanEnumList.Add(planEnum);
                }
                else
                {
                    //新添加的
                    var newPlanEnum = new PlanEnum()
                    {
                        PlanId = input.PlanId,
                        Name = item.Name,
                        Code = item.Code,
                        EnumProps = item.EnumPropsList.Select(x => new EnumInfo()
                        {
                            Name = x.Name,
                            Code = x.Name,
                            Value = x.Value
                        }).ToList().ToJson(),
                    };
                    newPlanEnumList.Add(newPlanEnum);
                }
            }

            if (newPlanEnumList.Count > 0)
            {
                await _sqlSugarClient.Insertable(newPlanEnumList).ExecuteCommandAsync();
            }

            if (updatePlanEnumList.Count > 0)
            {
                await _sqlSugarClient.Updateable(updatePlanEnumList).ExecuteCommandAsync();
            }

        }

        public async Task DeleteAsync(IdInput<Guid> input)
        {
            await _sqlSugarClient.Updateable<PlanEnum>().Where(it => it.Id == input.Id).SetColumns(x => x.IsDeleted == true).ExecuteCommandAsync();

        }

        public static void ValidFiledName(string value)
        {
            if (value.IsNullOrWhiteSpace())
            {
                throw new YouJuException("枚举名称不能为空");
            }
            if (value.Length > 40)
            {
                throw new YouJuException("枚举名称长度不能超过40个字符");
            }

        }

        public static void ValidFiledCode(string value)
        {
            if (value.IsNullOrWhiteSpace())
            {
                throw new YouJuException("枚举编码不能为空");
            }
            if (value.Length > 40)
            {
                throw new YouJuException("枚举编码长度不能超过40个字符");
            }


        }

        public static void CheckEnumInfoList(List<EnumInfo> list)
        {
            if (list.Count > 20)
            {
                throw new YouJuException("枚举项不要超过20个数量");
            }
            foreach (var item in list)
            {
                if (item.Name.IsNullOrWhiteSpace())
                {
                    throw new YouJuException("枚举项Name不能为空");
                }
                if (item.Name.Length > 20)
                {
                    throw new YouJuException("枚举项Name长度不能超过20个字符");
                }
                if (item.Code.IsNullOrWhiteSpace())
                {
                    throw new YouJuException("枚举项Code不能为空");
                }
                if (item.Code.Length > 40)
                {
                    throw new YouJuException("枚举项Code长度不能超过40个字符");
                }
                if (item.Value.IsNullOrWhiteSpace())
                {
                    throw new YouJuException("枚举项Value不能为空");
                }
                if (item.Value.Length > 40)
                {
                    throw new YouJuException("枚举项Value长度不能超过40个字符");
                }
            }

        }
    }

}
