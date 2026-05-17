namespace Web.Extensions
{
    public class CustomAuthorizationAttribute:Attribute
    {
        public List<RoleType> RoleTypes { get; set; }

        public CustomAuthorizationAttribute(params RoleType[] values) {
            RoleTypes = values.ToList();
        }
    }
}
