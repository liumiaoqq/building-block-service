using Scriban;

using System.Runtime.CompilerServices;
using Web.Dto.Gengerations;
using Web.GenerationCode.Dto;
using Web.GenerationCode.Provider;

namespace Web.GenerationCode.Manager
{
    public class GenerationCodeManager : IGenerationCodeManager
    {

        private readonly IConfiguration _configuration;

        private readonly IWebHostEnvironment _environment;

        private readonly IEnumerable<IGengerationTranslateProvider> _gengerationTranslateProviders;

        private PlanGenerationWriteInput _planGenerationWriteInput;

        private List<GengerationComponentTreeData> _templteTree;

        private List<GengerationTranferTree> _tranferTrees;

        private Dictionary<LanguageWay, Type> _keyValuePairs = new Dictionary<LanguageWay, Type>() {
            {LanguageWay.NetCore,typeof(NetCoreProvider) },
            {LanguageWay.UniApp,typeof(UniappProvider) },
            {LanguageWay.ElementUI,typeof(ElementUIProvider) },
            {LanguageWay.SpringBoot,typeof(SpringBootProvider) },
            {LanguageWay.Flask,typeof(SpringBootProvider) },
        };

        public GenerationCodeManager(IConfiguration configuration, IWebHostEnvironment environment, IEnumerable<IGengerationTranslateProvider> gengerationTranslateProviders)
        {
            _configuration = configuration;
            _environment = environment;
            _gengerationTranslateProviders = gengerationTranslateProviders;
        }


        /// <summary>
        /// 绑定数据
        /// </summary>

        public IGenerationCodeManager LoadModule(PlanGenerationWriteInput writeInput, List<GengerationComponentTreeData> templteTree)
        {
            _planGenerationWriteInput = writeInput;
            _templteTree = templteTree;
            return this;

        }

        /// <summary>
        /// 加载模板
        /// </summary>
        /// <returns></returns>
        public IGenerationCodeManager TempleteConvert()
        {


            List<GengerationTranferTree> tranferTrees = new List<GengerationTranferTree>();
            foreach (var project in _templteTree.Where(x => x.IsTemplete == false))
            {
                var tranferTree = new GengerationTranferTree()
                {
                    FileType = project.FileType,
                    FileName = $"{_planGenerationWriteInput.FileName}.{project.LanguageWay.ToString().ToLower()}",
                    IsTemplete = false,
                    IsFolder = project.IsFolder,
                };

                var implementationProvider = _gengerationTranslateProviders.FirstOrDefault(x => x.GetType() == _keyValuePairs[project.LanguageWay]);

                tranferTree.Children.AddRange(implementationProvider.GenerateResult(_planGenerationWriteInput, project.Children));
                tranferTrees.Add(tranferTree);

            }
            _tranferTrees = tranferTrees;
            return this;

        }
        /// <summary>
        /// 后置过滤
        /// </summary>
        /// <returns></returns>
        public IGenerationCodeManager PostFilter(List<string> postFilterKeyWords)
        {
            if (postFilterKeyWords == null || postFilterKeyWords.Count == 0)
            {
                return this;
            }

            var filteredTrees = new List<GengerationTranferTree>();

            foreach (var tree in _tranferTrees)
            {
                var filteredTree = FilterTreeNode(tree, postFilterKeyWords);
                if (filteredTree != null)
                {
                    filteredTrees.Add(filteredTree);
                }
            }

            _tranferTrees = filteredTrees;
            return this;
        }

        /// <summary>
        /// 递归过滤树节点
        /// </summary>
        /// <param name="node">当前节点</param>
        /// <param name="keywords">关键词列表</param>
        /// <returns>过滤后的节点，如果不匹配则返回null</returns>
        private GengerationTranferTree FilterTreeNode(GengerationTranferTree node, List<string> keywords)
        {
            if (node == null) return null;

            // 检查当前节点是否匹配关键词
            bool currentNodeMatches = IsNodeMatched(node, keywords);

            // 递归过滤子节点
            var filteredChildren = new List<GengerationTranferTree>();
            if (node.Children != null)
            {
                foreach (var child in node.Children)
                {
                    var filteredChild = FilterTreeNode(child, keywords);
                    if (filteredChild != null)
                    {
                        filteredChildren.Add(filteredChild);
                    }
                }
            }

            // 如果当前节点匹配或者有匹配的子节点，则保留当前节点
            if (currentNodeMatches || filteredChildren.Count > 0)
            {
                var filteredNode = new GengerationTranferTree
                {
                    FileType = node.FileType,
                    FileName = node.FileName,
                    IsTemplete = node.IsTemplete,
                    IsFolder = node.IsFolder,
                    Content = node.Content,
                    NetworkAddress = node.NetworkAddress,

                    Children = filteredChildren
                };
                return filteredNode;
            }

            return null;
        }

        /// <summary>
        /// 检查节点是否匹配关键词
        /// </summary>
        /// <param name="node">节点</param>
        /// <param name="keywords">关键词列表</param>
        /// <returns>是否匹配</returns>
        private bool IsNodeMatched(GengerationTranferTree node, List<string> keywords)
        {
            if (string.IsNullOrEmpty(node.FileName)) return false;

            var fileName = node.FileName.ToLower();

            // 检查是否包含任意一个关键词 （模糊匹配）
            return keywords.Any(keyword =>
                !string.IsNullOrEmpty(keyword) &&
                fileName.Contains(keyword.ToLower())
            );
        }
        /// <summary>
        /// 得到转换的结果
        /// </summary>
        /// <returns></returns>
        public List<GengerationTranferTree> GetConvertResult()
        {

            return _tranferTrees;
        }


    }
}
