using Web.Dto.Gengerations;
using Web.GenerationCode.Dto;

namespace Web.GenerationCode.Manager
{
    public interface IGenerationCodeManager
    {

        /// <summary>
        /// 绑定数据
        /// </summary>
        public IGenerationCodeManager LoadModule(PlanGenerationWriteInput writeInput, List<GengerationComponentTreeData> templteTree);


        /// <summary>
        /// 加载模板
        /// </summary>
        /// <returns></returns>
        public IGenerationCodeManager TempleteConvert();

        /// <summary>
        /// 得到结果
        /// </summary>
        /// <returns></returns>
        public List<GengerationTranferTree> GetConvertResult();

        /// <summary>
        /// 文件词过滤
        /// </summary>
        /// <returns></returns>
        public IGenerationCodeManager PostFilter(List<string> postFilterKeyWords);
    }
}
