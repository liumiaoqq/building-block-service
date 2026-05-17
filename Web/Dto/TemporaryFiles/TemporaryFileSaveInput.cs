namespace Web.Dto.TemporaryFiles
{
    public class TemporaryFileSaveInput
    {
        public Guid ComponentId { get; set; }

        public  Guid? ComponentTempleteId { get; set; }
        /// <summary>
        /// 被选中的树
        /// </summary>
        public List<CombineTemporaryFileTree> CheckedCombineTemporaryFileTrees { get; set; }
    
    }
}
