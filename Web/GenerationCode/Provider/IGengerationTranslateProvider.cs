using Web.Dto.Gengerations;
using Web.GenerationCode.Dto;

namespace Web.GenerationCode.Provider
{
    /// <summary>
    /// 代码生成代理
    /// </summary>
    public interface IGengerationTranslateProvider
    {
        

        /// <summary>
        /// 生成结果
        /// </summary>
        public abstract List<GengerationTranferTree> GenerateResult(PlanGenerationWriteInput context,List<GengerationComponentTreeData> treeDatas);



    }
}
