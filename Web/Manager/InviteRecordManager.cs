using System.Text;

namespace Web.Manager
{
    public class InviteRecordManager
    {
        protected ISqlSugarClient _sqlSugarClient;
        protected ICurrentUser CurrentUser;


        public InviteRecordManager(ISqlSugarClient sqlSugarClient, ICurrentUser currentUser)
        {
            _sqlSugarClient = sqlSugarClient;
            CurrentUser = currentUser;
        }


        public async Task<PagedReuslt<InviteRecordDto>> ListAsync(InviteRecordPagedInput input)
        {
            var result = await _sqlSugarClient.Queryable<InviteRecord>()
     .WhereIF(input.VisitCode.IsNotNullOrNotWhiteSpace(), x => x.VisitCode == input.VisitCode)
           .WhereIF(input.InviteUserName.IsNotNullOrNotWhiteSpace(), x => SqlFunc.Subqueryable<AppUser>().Where(s => s.Id == x.InviteUserId && (s.UserName.Contains(input.InviteUserName) || s.Email.Contains(input.InviteUserName))).Any())
           .WhereIF(input.UserName.IsNotNullOrNotWhiteSpace(), x => SqlFunc.Subqueryable<AppUser>().Where(s => s.Id == x.UserId && s.UserName.Contains(input.UserName) || s.Email.Contains(input.UserName)).Any())
            .OrderByDescending(x => x.CreationTime)
            .Select<InviteRecordDto>()
            .ToPageListAsync(input.Page, input.Size);
            //得到对应userId和InviteRecordDto的ids并集
            var ids = result.Select(x => x.InviteUserId).ToList();
            ids.AddRange(result.Select(x => x.UserId).ToList());
            //查询出所有的用户信息
            var userDtos = await _sqlSugarClient.Queryable<AppUser>()
            .Where(x => ids.Contains(x.Id)).ToListAsync();

            foreach (var item in result)
            {
                item.UserName = userDtos.FirstOrDefault(x => x.Id == item.UserId)?.UserName;
                item.Email = userDtos.FirstOrDefault(x => x.Id == item.UserId)?.Email;
                item.InviteUserName = userDtos.FirstOrDefault(x => x.Id == item.InviteUserId)?.UserName;
                item.InviteEmail = userDtos.FirstOrDefault(x => x.Id == item.InviteUserId)?.Email;
            }


            return new PagedReuslt<InviteRecordDto>(result, result.Count);
        }
        public async Task<PagedReuslt<UserInviteRecordDto>> UserListAsync(InviteRecordPagedInput input)
        {

            Guid userId = CurrentUser.UserId.Value;

            var result = await _sqlSugarClient.Queryable<InviteRecord>()
           .WhereIF(input.VisitCode.IsNotNullOrNotWhiteSpace(), x => x.VisitCode == input.VisitCode)

           .OrderByDescending(x => x.CreationTime)
           .Select<UserInviteRecordDto>()
           .ToPageListAsync(input.Page, input.Size);
            return new PagedReuslt<UserInviteRecordDto>(result, result.Count);
        }



        public async Task<InviteRecord> CreateAsync(InviteRecord inviteRecord)
        {
            var result = await _sqlSugarClient.Insertable(inviteRecord).ExecuteReturnEntityAsync();
            return result;
        }



    }
}
