namespace Web.Dto.TableEntitys
{
    public class TableEntitySqlScriptSubmitDto
    {
        public Guid? PlanId { get; set; }

        public List<TableEntityDto> TableEntityDtos { get; set; }
    }
}
