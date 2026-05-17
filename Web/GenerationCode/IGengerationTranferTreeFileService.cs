using Web.GenerationCode.Dto;

namespace Web.GenerationCode
{
    public interface IGengerationTranferTreeFileService
    {
        public IGengerationTranferTreeFileService InitSettings(string rootDir,string zipFileName);
        public string GetTranferDownAddress(List<GengerationTranferTree> gengerationTranferTrees);
    }
}
