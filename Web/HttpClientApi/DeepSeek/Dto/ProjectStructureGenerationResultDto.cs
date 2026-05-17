namespace Web.HttpClientApi.DeepSeek.Dto
{
    /// <summary>
    /// 项目结构生成结果DTO
    /// </summary>
    public class ProjectStructureGenerationResultDto
    {
        /// <summary>
        /// 项目结构树
        /// </summary>
        public ProjectTreeNodeDto ProjectTree { get; set; }
    }

    /// <summary>
    /// 项目树节点DTO
    /// </summary>
    public class ProjectTreeNodeDto
    {
        /// <summary>
        /// 节点名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 节点ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 父节点ID
        /// </summary>
        public int Pid { get; set; }

        /// <summary>
        /// 是否叶子节点
        /// </summary>
        public bool IsLeaf { get; set; }

        /// <summary>
        /// 子节点列表
        /// </summary>
        public List<ProjectTreeNodeDto> Children { get; set; }
    }
}

