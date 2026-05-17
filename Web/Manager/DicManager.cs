using Org.BouncyCastle.Utilities;

namespace Web.Manager
{
    public class DicManager
    {
        protected ISqlSugarClient _sqlSugarClient;

        private readonly IWebHostEnvironment _webHostEnvironment;



        public DicManager(ISqlSugarClient sqlSugarClient, IWebHostEnvironment webHostEnvironment)
        {
            _sqlSugarClient = sqlSugarClient;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<List<DicTypeDto>> GetDicTypeListByDicType(DicCodePagedInput input)
        {

            var dicTypes = await _sqlSugarClient.Queryable<DicType>()
                   .WhereIF(input.DicTypeId.HasValue, x => x.Id == input.DicTypeId.Value)
                    .WhereIF(input.DicTypeIds.HasItem(), x => input.DicTypeIds.Contains(x.Id))
                   .WhereIF(input.DicTypeCode.IsNotNullOrNotWhiteSpace(), x => x.Code == input.DicTypeCode)
                   .Select<DicTypeDto>()
                   .OrderByDescending(x => x.Sort)
                   .ToListAsync();

            input.DicTypeIds = dicTypes.Select(x => x.Id.Value).Distinct().ToList();
            var dicCodes = await _sqlSugarClient.Queryable<DicCode>()
                    .WhereIF(input.DicTypeIds.HasItem(), x => input.DicTypeIds.Contains(x.DicTypeId))
                    .OrderByDescending(x => x.Sort)
                    .Select<DicCodeDto>()
                    .ToListAsync();
            foreach (var item in dicTypes)
            {
                item.DicCodeDtos = dicCodes.Where(x => x.DicTypeId == item.Id).ToList();
            }
            return dicTypes;

        }
        public async Task<List<DicCodeDto>> GetDicCodeList(DicCodePagedInput input)
        {
            var rs = await GetDicTypeListByDicType(input);
          
            return rs.FirstOrDefault()?.DicCodeDtos;
        }
      

    }
}
