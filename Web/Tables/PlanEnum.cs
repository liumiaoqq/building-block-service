using Org.BouncyCastle.Bcpg.OpenPgp;

namespace Web.Tables
{
    [YoungTable("PlanEnum")]
    public class PlanEnum : CreationAuditedAggregateRoot
    {
        public Guid PlanId { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }


        public string EnumProps { get; set; }

        [SugarColumn(IsIgnore = true)]
        public List<EnumInfo> EnumPropsList => EnumProps.ToList<EnumInfo>();
    }

    public class EnumInfo
    {

        public string Name { get; set; }

        public string Code { get; set; }

        public string Value { get; set; }
    }
}
