using Web.Manager;

namespace Web.Service
{
    public class ColumnPropService
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        private readonly ICurrentUser _currentUser;


        private readonly ColumnPropManager _columnPropManager;

        public ColumnPropService(ISqlSugarClient sqlSugarClient, ICurrentUser currentUser, ColumnPropManager columnPropManager)
        {
            _sqlSugarClient = sqlSugarClient;
            _currentUser = currentUser;
            _columnPropManager = columnPropManager;
        }


        public async Task<List<ColumnPropDto>> GetColumnProps(ColumnPropPagedInput input)
        {
            if (_currentUser.IsUser())
            {

                input.UserId = _currentUser.UserId;
                if (input.TableEntityId.HasValue == false)
                {
                    throw new YouJuException("TableEntityId参数丢失");

                }
            }

            return await _columnPropManager.GetColumnProps(input);
        }


        public PagedReuslt<SelectResult> GetColumnPropTypes(ColumnPropPagedInput input)
        {
            if (_currentUser.IsUser())
            {

                input.UserId = _currentUser.UserId;
                if (input.PlanId.HasValue == false)
                {
                    throw new YouJuException("PlanId参数丢失");

                }
            }


            return _columnPropManager.GetColumnPropTypes(input);
        }


        public async Task BatchCreateOrEditColumnProp(List<ColumnPropDto> input)
        {
            await _columnPropManager.BatchCreateOrEditColumnProp(input);
        }
    }
}
