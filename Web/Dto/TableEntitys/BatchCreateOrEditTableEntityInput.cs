namespace Web.Dto.TableEntitys
{
    public class BatchCreateOrEditTableEntityInput
    {

        public Guid PlanId { get; set; }


        public List<TableEntityDto> TableEntities { get; set; }

    }
}
