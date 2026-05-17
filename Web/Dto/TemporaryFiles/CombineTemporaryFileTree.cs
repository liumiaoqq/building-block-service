namespace Web.Dto.TemporaryFiles
{

    public class CombineTemporaryFileTree : CombineTemporaryFileNode
    {

        public bool? IsAdd { get; set; }

        public bool? IsEdit { get; set; }

        public List<CombineTemporaryFileTree> Chidren { get; set; } = new List<CombineTemporaryFileTree>();

        public TemporaryFileRecordNode TemporaryFileRecordNode { get; set; }

        public ComponentTempleteNode ComponentTempleteNode { get; set; }

    }
}
