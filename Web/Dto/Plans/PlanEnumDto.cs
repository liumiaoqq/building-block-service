namespace Web.Dto.Plans
{
    public class PlanEnumDto : FullBaseDto
    {
        public Guid PlanId { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string EnumProps { get; set; }

        public List<EnumInfo> EnumPropsList;


    }

}
