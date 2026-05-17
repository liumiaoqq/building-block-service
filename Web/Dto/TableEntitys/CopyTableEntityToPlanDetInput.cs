using NPOI.HPSF;

namespace Web.Dto.TableEntitys
{
    public class CopyTableEntityToPlanDetInput
    {
        public Guid PlanId { get; set; }

        public List<Guid> CheckIds { get; set; }
    }
}
