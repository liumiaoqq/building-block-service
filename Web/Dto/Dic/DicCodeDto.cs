namespace Web
{
    public class DicCodeDto:FullBaseDto
    {
        public Guid? DicTypeId { get; set; }
        public DicTypeDto DicTypeDto { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public int Sort { get; set; }

        public string Remark { get; set; }

        public bool IsSystem { get; set; }
    }

}
