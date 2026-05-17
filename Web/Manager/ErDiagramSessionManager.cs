using System;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using Web.HttpClientApi.DeepSeek.Service;
using Web.HttpClient;

namespace Web.Manager
{
    public class ErDiagramSessionManager
    {
        private readonly ISqlSugarClient _sqlSugarClient;
        private readonly ICurrentUser _currentUser;
        private readonly IDeepSeekService _deepSeekService;

        public ErDiagramSessionManager(
            ISqlSugarClient sqlSugarClient,
            ICurrentUser currentUser,
            IDeepSeekService deepSeekService)
        {
            _sqlSugarClient = sqlSugarClient;
            _currentUser = currentUser;
            _deepSeekService = deepSeekService;
        }

        /// <summary>
        /// 获取ER图会话分页列表
        /// </summary>
        public async Task<PagedReuslt<ErDiagramSessionDto>> ListAsync(GetErDiagramSessionPagedInput input)
        {
            var userId = _currentUser.GetUserId();
            var result = await _sqlSugarClient.Queryable<ErDiagramSession>()
                .Where(x => x.CreatorId == userId)
                .WhereIF(input.Name.IsNotNullOrNotWhiteSpace(), x => x.Name.Contains(input.Name))
                .WhereIF(input.CodeManagementId.HasValue, x => x.CodeManagementId == input.CodeManagementId)
                .WhereIF(input.IsActive.HasValue, x => x.IsActive == input.IsActive)
                .OrderByDescending(x => x.IsActive)
                .OrderByDescending(x => x.Id)
                .Select<ErDiagramSessionDto>()
                .ToPageListAsync(input.Page, input.Size);

            return new PagedReuslt<ErDiagramSessionDto>(result, result.Count);
        }

        /// <summary>
        /// 根据ID获取ER图会话
        /// </summary>
        public async Task<ErDiagramSessionDto> GetByIdAsync(Guid id)
        {
            var userId = _currentUser.GetUserId();
            return await _sqlSugarClient.Queryable<ErDiagramSession>()
                .Where(x => x.Id == id)
                .Where(x => x.CreatorId == userId)
                .Select<ErDiagramSessionDto>()
                .FirstAsync();
        }

        /// <summary>
        /// 获取当前激活的ER图会话
        /// </summary>
        public async Task<ErDiagramSessionDto> GetActiveErDiagramSessionAsync(Guid? codeManagementId = null)
        {
            var userId = _currentUser.GetUserId();
            return await _sqlSugarClient.Queryable<ErDiagramSession>()
                .Where(x => x.CreatorId == userId)
                .Where(x => x.IsActive == true)
                .WhereIF(codeManagementId.HasValue, x => x.CodeManagementId == codeManagementId)
                .OrderByDescending(x => x.Id)
                .Select<ErDiagramSessionDto>()
                .FirstAsync();
        }

        /// <summary>
        /// 获取或创建激活的ER图会话
        /// </summary>
        public async Task<ErDiagramSessionDto> GetOrCreateActiveErDiagramSessionAsync(GetOrCreateActiveErDiagramSessionInput input)
        {
            var userId = _currentUser.GetUserId();

            // 先查询是否存在激活的ER图会话
            var activeErDiagram = await _sqlSugarClient.Queryable<ErDiagramSession>()
                .Where(x => x.CreatorId == userId)
                .Where(x => x.IsActive == true)
                .WhereIF(input.CodeManagementId.HasValue, x => x.CodeManagementId == input.CodeManagementId)
                .OrderByDescending(x => x.Id)
                .Select<ErDiagramSessionDto>()
                .FirstAsync();

            // 如果存在，直接返回
            if (activeErDiagram != null)
            {
                return activeErDiagram;
            }

            // 如果不存在，创建一个新的激活ER图会话
            var entity = new ErDiagramSession
            {
                Name = "未命名ER图",
                CodeManagementId = input.CodeManagementId,
                IsActive = true,
                CompleteSql = null,
                TableRelationJson = null,
                UserPrompt = null,
                AiInputContent = null,
                AiOutputResult = null,
                Remark = null
            };

            await _sqlSugarClient.Insertable(entity).ExecuteCommandAsync();

            return await GetByIdAsync(entity.Id);
        }

        /// <summary>
        /// 创建ER图会话
        /// </summary>
        public async Task<Guid> CreateAsync(CreateErDiagramSessionInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Name))
            {
                throw new Exception("会话名称不能为空");
            }

            var userId = _currentUser.GetUserId();

            // 如果新建的ER图会话设置为激活，需要将其他激活的ER图会话设置为非激活
            if (input.IsActive)
            {
                var activeSessions = await _sqlSugarClient.Queryable<ErDiagramSession>()
                    .Where(x => x.CreatorId == userId)
                    .Where(x => x.IsActive == true)
                    .WhereIF(input.CodeManagementId.HasValue, x => x.CodeManagementId == input.CodeManagementId)
                    .ToListAsync();

                if (activeSessions != null && activeSessions.Count > 0)
                {
                    foreach (var session in activeSessions)
                    {
                        session.IsActive = false;
                    }
                    await _sqlSugarClient.Updateable(activeSessions)
                        .UpdateColumns(x => new { x.IsActive })
                        .ExecuteCommandAsync();
                }
            }

            var entity = new ErDiagramSession
            {
                Name = input.Name,
                CodeManagementId = input.CodeManagementId,
                IsActive = input.IsActive,
                CompleteSql = input.CompleteSql,
                TableRelationJson = input.TableRelationJson,
                UserPrompt = input.UserPrompt,
                AiInputContent = input.AiInputContent,
                AiOutputResult = input.AiOutputResult,
                Remark = input.Remark
            };

            await _sqlSugarClient.Insertable(entity).ExecuteCommandAsync();
            return entity.Id;
        }

        /// <summary>
        /// 更新ER图会话
        /// </summary>
        public async Task UpdateAsync(UpdateErDiagramSessionInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Name))
            {
                throw new Exception("会话名称不能为空");
            }

            var userId = _currentUser.GetUserId();
            var entity = await _sqlSugarClient.Queryable<ErDiagramSession>()
                .Where(x => x.Id == input.Id)
                .Where(x => x.CreatorId == userId)
                .FirstAsync();

            if (entity == null)
            {
                throw new Exception("ER图会话不存在");
            }

            // 如果将此ER图会话设置为激活，需要将其他激活的ER图会话设置为非激活
            if (input.IsActive && !entity.IsActive)
            {
                var activeSessions = await _sqlSugarClient.Queryable<ErDiagramSession>()
                    .Where(x => x.CreatorId == userId)
                    .Where(x => x.Id != input.Id)
                    .Where(x => x.IsActive == true)
                    .ToListAsync();

                if (activeSessions != null && activeSessions.Count > 0)
                {
                    foreach (var session in activeSessions)
                    {
                        session.IsActive = false;
                    }
                    await _sqlSugarClient.Updateable(activeSessions)
                        .UpdateColumns(x => new { x.IsActive })
                        .ExecuteCommandAsync();
                }
            }

            entity.Name = input.Name;
            entity.CodeManagementId = input.CodeManagementId;
            entity.IsActive = input.IsActive;
            entity.CompleteSql = input.CompleteSql;
            entity.TableRelationJson = input.TableRelationJson;
            entity.UserPrompt = input.UserPrompt;
            entity.AiInputContent = input.AiInputContent;
            entity.AiOutputResult = input.AiOutputResult;
            entity.Remark = input.Remark;

            await _sqlSugarClient.Updateable(entity).ExecuteCommandAsync();
        }

        /// <summary>
        /// 设置激活的ER图会话
        /// </summary>
        public async Task SetActiveErDiagramSessionAsync(SetActiveErDiagramSessionInput input)
        {
            var userId = _currentUser.GetUserId();
            var entity = await _sqlSugarClient.Queryable<ErDiagramSession>()
                .Where(x => x.Id == input.Id)
                .Where(x => x.CreatorId == userId)
                .FirstAsync();

            if (entity == null)
            {
                throw new Exception("ER图会话不存在");
            }

            // 先查询出其他激活的ER图会话
            var activeSessions = await _sqlSugarClient.Queryable<ErDiagramSession>()
                .Where(x => x.CreatorId == userId)
                .Where(x => x.Id != input.Id)
                .Where(x => x.IsActive == true)
                .WhereIF(entity.CodeManagementId.HasValue, x => x.CodeManagementId == entity.CodeManagementId)
                .ToListAsync();

            // 修改这些会话的激活状态
            if (activeSessions != null && activeSessions.Count > 0)
            {
                foreach (var session in activeSessions)
                {
                    session.IsActive = false;
                }
                await _sqlSugarClient.Updateable(activeSessions)
                    .UpdateColumns(x => new { x.IsActive })
                    .ExecuteCommandAsync();
            }

            // 设置当前ER图会话为激活
            entity.IsActive = true;
            await _sqlSugarClient.Updateable(entity)
                .UpdateColumns(x => new { x.IsActive })
                .ExecuteCommandAsync();
        }

        /// <summary>
        /// 删除ER图会话
        /// </summary>
        public async Task DeleteAsync(Guid id)
        {
            var userId = _currentUser.GetUserId();
            await _sqlSugarClient.Updateable<ErDiagramSession>()
                .SetColumns(x => x.IsDeleted == true)
                .Where(x => x.Id == id)
                .Where(x => x.CreatorId == userId)
                .ExecuteCommandAsync();
        }

        /// <summary>
        /// AI生成ER图相关数据
        /// </summary>
        public async Task<ErDiagramSessionDto> GenerateErDiagramByAIAsync(GenerateErDiagramByAIInput input)
        {
            var userId = _currentUser.GetUserId();

         
            // 获取会话信息
            var session = await _sqlSugarClient.Queryable<ErDiagramSession>()
                .Where(x => x.Id == input.ErDiagramSessionId)
                .Where(x => x.CreatorId == userId)
                .FirstAsync();

            if (session == null)
            {
                throw new Exception("ER图会话不存在");
            }
               if (string.IsNullOrWhiteSpace(session.UserPrompt))
            {
                throw new Exception("功能提示词不能为空");
            }

            if (string.IsNullOrWhiteSpace(session.TableRelationJson))
            {
                throw new Exception("表关系JSON不能为空");
            }


            // 构建系统提示词（固定部分）
            var systemPrompt = @"你是专业的业务分析师和ER图设计师。根据功能提示词中的角色和业务需求，设计清晰合理的ER图。

【分析优先级】
1. 实体设计：从数据库表结构中提取业务表作为候选实体（用户表除外，用角色实体替代）
2. 关系设计：优先从功能提示词挖掘动作词，而不是依赖表的外键关系
3. 实体过滤：最终只保留有关系的实体，没有关系的实体直接去掉
4. 核心原则：表结构决定候选实体，功能提示词决定关系，有关系才保留实体

【设计原则】
1. 动作词深度挖掘（重要）：
   - 仔细阅读功能提示词，提取所有动作词（管理、审核、创建、发布、执行、领取、分配、查看、编辑、删除、批准、拒绝、提交、完成、取消等）
   - 每个动作词对应一个关系，动作的主体和客体成为实体
   - 分析业务流程中的动词和操作，不要遗漏任何业务动作
   - 优先使用功能描述中的原始动作词，保持业务语义准确
   
2. 实体来源（必须遵守）：
   - 将数据库表结构中的所有业务表作为候选实体
   - 用户表不作为实体，改用角色实体（如管理员、普通用户等）
   - 所有订单表、商品表、分类表、库存表等业务表都可作为候选实体
   
3. 关系来源（核心）：
   - 从功能提示词挖掘动作词，形成关系
   - 不要依赖表的外键关系自动生成关系
   - 每个动作词对应一个关系：角色→动作→业务实体
   - 例如：管理员审核订单→管理员→审核→订单（审核来自功能描述）
   - 例如：订单包含商品→订单→包含→商品（包含来自业务逻辑）
   
4. 实体过滤规则（重要）：
   - 最终输出的实体列表中，只保留在关系中出现过的实体
   - 如果某个候选实体（从表结构提取的）没有出现在任何关系中，直接去掉，不要输出
   - 例如：如果日志表、配置表等在功能描述中没有任何关系，则不要包含在实体列表中
   
5. 关系数量控制：
   - 任何实体的直接关联不超过5个
   - 被多个表引用的基础实体只保留核心关联（2-3个）
   - 超过5个关联时，将部分改为管理关系
   
6. 关系去重优化（重要）：
   - 两个实体之间最多只有1个关系
   - 如果有多个动作，智能选择合并动作词：
     * 管理员类角色（管理员、系统管理员、超级管理员等）：多个动作合并为管理
     * 普通用户类角色（用户、会员、客户等）：多个动作合并为创建或下单
   - 例如：管理员对商品有审核、编辑、删除三个动作，合并为管理员管理商品
   - 例如：用户对订单有下单、支付两个动作，合并为用户创建订单
   - 例如：用户对商品有浏览、收藏、评价三个动作，合并为用户管理商品
   
7. 命名规范：
   - EntityName用简洁中文（去掉表字）
   - RelationshipName必须用动作词，优先从功能提示词中提取
   - 常见动作词：管理、审核、包含、所属、创建、发布、执行、领取、分配、批准、提交、完成等

【示例分析】
功能提示词：电商系统，管理员可以审核订单和管理商品，用户可以下单、支付、评价商品
表结构：users表、orders表、products表、order_items表、reviews表、categories表、system_logs表、system_config表

分析流程：
第1步-从表结构提取候选实体（用户表除外）：
  * 业务表：orders、products、order_items、reviews、categories、system_logs、system_config
  * 转为候选实体：订单、商品、订单明细、评价、分类、系统日志、系统配置
  
第2步-从功能提示词识别角色（替代用户表）：
  * 角色：管理员、用户
  
第3步-从功能提示词提取动作词：
  * 审核、管理、下单、支付、评价
  
第4步-关系去重优化（两个实体间只保留1个关系）：
  * 用户到订单：有下单和支付两个动作，用户是普通用户角色，合并为创建
  * 用户到商品：只有评价1个动作，保留评价
  * 管理员到订单：只有审核1个动作，保留审核
  * 管理员到商品：只有管理1个动作，保留管理
  
第5步-设计最终关系：
  * 管理员→审核→订单（单一动作，保留审核）
  * 管理员→管理→商品（单一动作，保留管理）
  * 用户→创建→订单（多个动作，普通用户角色，合并为创建）
  * 用户→评价→商品（单一动作，保留评价）
  * 订单→包含→订单明细（包含来自业务逻辑）
  * 订单明细→关联→商品（关联来自业务逻辑）
  * 商品→所属→分类（所属来自业务逻辑）
  
第6步-实体过滤（重要）：
  * 在关系中出现的实体：管理员、用户、订单、商品、订单明细、分类、评价
  * 未在关系中出现的实体：系统日志、系统配置
  * 最终实体列表：只保留管理员、用户、订单、商品、订单明细、分类、评价
  * 去掉的实体：系统日志、系统配置（因为功能描述中没有涉及它们的任何关系）

关键：根据角色类型智能选择合并词，管理员用管理，普通用户用创建；没有关系的实体直接去掉不输出

输出JSON格式：
{
  'Entities': [
    {'EntityCode': 'admin', 'EntityName': '管理员'},
    {'EntityCode': 'user', 'EntityName': '用户'},
    {'EntityCode': 'orders', 'EntityName': '订单'},
    {'EntityCode': 'products', 'EntityName': '商品'},
    {'EntityCode': 'order_items', 'EntityName': '订单明细'},
    {'EntityCode': 'reviews', 'EntityName': '评价'},
    {'EntityCode': 'categories', 'EntityName': '分类'}
  ],
  'Relationships': [
    {'RelationshipCode': 'admin_audits_orders', 'RelationshipName': '审核', 'FromEntityCode': 'admin', 'ToEntityCode': 'orders', 'Cardinality': '1:N'},
    {'RelationshipCode': 'admin_manages_products', 'RelationshipName': '管理', 'FromEntityCode': 'admin', 'ToEntityCode': 'products', 'Cardinality': '1:N'},
    {'RelationshipCode': 'user_creates_orders', 'RelationshipName': '创建', 'FromEntityCode': 'user', 'ToEntityCode': 'orders', 'Cardinality': '1:N'},
    {'RelationshipCode': 'user_reviews_products', 'RelationshipName': '评价', 'FromEntityCode': 'user', 'ToEntityCode': 'products', 'Cardinality': 'N:N'},
    {'RelationshipCode': 'orders_contains_items', 'RelationshipName': '包含', 'FromEntityCode': 'orders', 'ToEntityCode': 'order_items', 'Cardinality': '1:N'},
    {'RelationshipCode': 'items_relates_products', 'RelationshipName': '关联', 'FromEntityCode': 'order_items', 'ToEntityCode': 'products', 'Cardinality': 'N:1'},
    {'RelationshipCode': 'products_belongs_categories', 'RelationshipName': '所属', 'FromEntityCode': 'products', 'ToEntityCode': 'categories', 'Cardinality': 'N:1'}
  ],
  'Description': '电商系统ER图，智能合并：用户创建订单（普通用户），管理员管理商品。注意：系统日志和系统配置因无关系被过滤'
}

请深入分析功能提示词，重点挖掘所有动作词，以角色和业务为核心，设计合理的ER图JSON。
关键规则：
1. 两个实体之间最多只有1个关系，多个动作智能合并：管理员类角色用管理，普通用户类角色用创建
2. 只输出有关系的实体，没有关系的实体直接去掉不要输出
3. 根据角色类型智能选择动作词：管理员→管理，用户→创建
4. 实体过滤是最后一步：先设计关系，再检查每个实体是否在关系中出现，未出现的直接去掉！";

            // 构建用户输入内容（变化部分）
            var userContentBuilder = new StringBuilder();
            userContentBuilder.AppendLine("=== 业务需求（功能提示词）===");
            userContentBuilder.AppendLine(session.UserPrompt);
            userContentBuilder.AppendLine();
            userContentBuilder.AppendLine("=== 数据库表结构信息 ===");
            userContentBuilder.AppendLine(session.TableRelationJson);

            var aiInputContent = userContentBuilder.ToString();

            var erDiagramResult = await _deepSeekService.ChatAsync(aiInputContent, systemPrompt);

            // 保存到数据库
            session.AiInputContent = aiInputContent;
            session.AiOutputResult = erDiagramResult;

            await _sqlSugarClient.Updateable(session)
                .UpdateColumns(x => new { x.AiInputContent, x.AiOutputResult })
                .ExecuteCommandAsync();

            return await GetByIdAsync(session.Id);
        }

        /// <summary>
        /// 压缩代码内容
        /// </summary>
        private string CompressCode(string content, string fileType)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return string.Empty;
            }

            var lines = content.Split('\n');
            var compressedLines = new StringBuilder();

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();

                if (string.IsNullOrWhiteSpace(trimmedLine))
                {
                    continue;
                }

                if (fileType == "JavaScript" || fileType == "Vue" || fileType == "TypeScript" || fileType == "C#")
                {
                    if (trimmedLine.StartsWith("//") || trimmedLine.StartsWith("///"))
                    {
                        continue;
                    }
                }

                compressedLines.AppendLine(trimmedLine);
            }

            return compressedLines.ToString();
        }
    }
}
