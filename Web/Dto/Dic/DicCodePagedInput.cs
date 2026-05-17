namespace Web
{
    public class DicCodePagedInput : PagedBaseInput
    {
        public Guid? DicTypeId { get; set; }

        public string DicTypeCode { get; set; }

        public List<Guid> DicTypeIds { get; set; }

        
    }
}
