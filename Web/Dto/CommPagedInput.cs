namespace Web.Dto
{
    public class CommPagedInput : PagedBaseInput
    {

    

        public string Keyword { get; set; }
        public string UserName { get; set; }
        public RoleType? RoleIds { get; set; }
        public string Email { get; set; }

        public string PhoneNumber { get; set; }


        public string Name { get; set; }


    }
}
