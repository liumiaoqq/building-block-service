using Web.Dto.Gengerations;

namespace Web.Provider
{
    /// <summary>
    /// 代码生成代理
    /// </summary>
    public interface IGengerationTranslateProvider
    {
        /// <summary>
        /// 初始数据上下文
        /// </summary>
        /// <param name="context"></param>
        public abstract void InitContext(PlanGenerationWriteInput context);

        /// <summary>
        /// 加载导入的模板
        /// </summary>
        public abstract void LoadModule();



        /// <summary>
        /// 生成结果
        /// </summary>
        public abstract void GenerateResult();
    }
}
