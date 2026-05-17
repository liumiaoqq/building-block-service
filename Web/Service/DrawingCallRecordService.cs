using Web.Manager;

namespace Web.Service
{
    public class DrawingCallRecordService
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        private readonly ICurrentUser _currentUser;

        private readonly DrawingCallRecordManager _drawingCallRecordManager;

        public DrawingCallRecordService(ISqlSugarClient sqlSugarClient, ICurrentUser currentUser, DrawingCallRecordManager drawingCallRecordManager)
        {
            _sqlSugarClient = sqlSugarClient;
            _currentUser = currentUser;
            _drawingCallRecordManager = drawingCallRecordManager;
        }

        public async Task<PagedReuslt<DrawingCallRecordDto>> ListAsync(DrawingCallRecordPagedInput input)
        {
            return await _drawingCallRecordManager.ListAsync(input);
        }

        /// <summary>
        /// 获取用户自己的调用记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedReuslt<DrawingCallRecordDto>> UserListAsync(DrawingCallRecordPagedInput input)
        {
            var userId = _currentUser.UserId;

            if (userId == null)
            {
                throw new Exception("用户未登录");
            }

            input.UserId = userId;
            return await _drawingCallRecordManager.ListAsync(input);
        }
    }
}

