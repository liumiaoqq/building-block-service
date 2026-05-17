using System.Collections.Generic;

namespace Web.HttpClientApi.DeepSeek.Dto
{
    /// <summary>
    /// ER图生成结果DTO
    /// </summary>
    public class ErDiagramGenerationResultDto
    {
        /// <summary>
        /// 实体列表
        /// </summary>
        public List<EntityDto> Entities { get; set; }

        /// <summary>
        /// 关系列表
        /// </summary>
        public List<RelationshipDto> Relationships { get; set; }

        /// <summary>
        /// 分析说明
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// 实体DTO
    /// </summary>
    public class EntityDto
    {
        /// <summary>
        /// 实体编码
        /// </summary>
        public string EntityCode { get; set; }

        /// <summary>
        /// 实体名称
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// 实体描述
        /// </summary>
        public string Description { get; set; }

     
    }


    /// <summary>
    /// 关系DTO
    /// </summary>
    public class RelationshipDto
    {
        /// <summary>
        /// 关系编码
        /// </summary>
        public string RelationshipCode { get; set; }

        /// <summary>
        /// 关系名称
        /// </summary>
        public string RelationshipName { get; set; }


        /// <summary>
        /// 第一个实体编码
        /// </summary>
        public string FromEntityCode { get; set; }

        /// <summary>
        /// 第二个实体编码
        /// </summary>
        public string ToEntityCode { get; set; }

        /// <summary>
        /// 基数（1:1, 1:N, N:M等）
        /// </summary>
        public string Cardinality { get; set; }
    }
}

