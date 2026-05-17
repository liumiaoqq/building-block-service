namespace Web
{
    public class DicTypeDto:FullBaseDto
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public int Sort { get; set; }

        public string Remark { get; set; }

        public bool IsSystem { get; set; }

        public List<DicCodeDto> DicCodeDtos { get; set; }

        public DicTypeDto()
        {
            DicCodeDtos = new List<DicCodeDto>();
        }
    }
}
