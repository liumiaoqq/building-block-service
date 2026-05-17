using System;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using Web.HttpClientApi.DeepSeek.Service;
using Web.HttpClient;

namespace Web.Manager
{
    public class UseCaseDiagramSessionManager
    {
        private readonly ISqlSugarClient _sqlSugarClient;
        private readonly ICurrentUser _currentUser;
        private readonly IDeepSeekService _deepSeekService;

        public UseCaseDiagramSessionManager(
            ISqlSugarClient sqlSugarClient,
            ICurrentUser currentUser,
            IDeepSeekService deepSeekService)
        {
            _sqlSugarClient = sqlSugarClient;
            _currentUser = currentUser;
            _deepSeekService = deepSeekService;
        }

        /// <summary>
        /// 获取用例图会话分页列表
        /// </summary>
        public async Task<PagedReuslt<UseCaseDiagramSessionDto>> ListAsync(GetUseCaseDiagramSessionPagedInput input)
        {
            var userId = _currentUser.GetUserId();
            var result = await _sqlSugarClient.Queryable<UseCaseDiagramSession>()
                .Where(x => x.CreatorId == userId)
                .WhereIF(input.Name.IsNotNullOrNotWhiteSpace(), x => x.Name.Contains(input.Name))
                .WhereIF(input.IsActive.HasValue, x => x.IsActive == input.IsActive)
                .OrderByDescending(x => x.IsActive)
                .OrderByDescending(x => x.Id)
                .Select<UseCaseDiagramSessionDto>()
                .ToPageListAsync(input.Page, input.Size);

            return new PagedReuslt<UseCaseDiagramSessionDto>(result, result.Count);
        }

        /// <summary>
        /// 根据ID获取用例图会话
        /// </summary>
        public async Task<UseCaseDiagramSessionDto> GetByIdAsync(Guid id)
        {
            var userId = _currentUser.GetUserId();
            return await _sqlSugarClient.Queryable<UseCaseDiagramSession>()
                .Where(x => x.Id == id)
                .Where(x => x.CreatorId == userId)
                .Select<UseCaseDiagramSessionDto>()
                .FirstAsync();
        }

        /// <summary>
        /// 获取当前激活的用例图会话
        /// </summary>
        public async Task<UseCaseDiagramSessionDto> GetActiveUseCaseDiagramSessionAsync()
        {
            var userId = _currentUser.GetUserId();
            return await _sqlSugarClient.Queryable<UseCaseDiagramSession>()
                .Where(x => x.CreatorId == userId)
                .Where(x => x.IsActive == true)
                .OrderByDescending(x => x.Id)
                .Select<UseCaseDiagramSessionDto>()
                .FirstAsync();
        }

        /// <summary>
        /// 获取或创建激活的用例图会话
        /// </summary>
        public async Task<UseCaseDiagramSessionDto> GetOrCreateActiveUseCaseDiagramSessionAsync(GetOrCreateActiveUseCaseDiagramSessionInput input)
        {
            var userId = _currentUser.GetUserId();

            // 先查询是否存在激活的用例图会话
            var activeUseCaseDiagram = await _sqlSugarClient.Queryable<UseCaseDiagramSession>()
                .Where(x => x.CreatorId == userId)
                .Where(x => x.IsActive == true)
                .OrderByDescending(x => x.Id)
                .Select<UseCaseDiagramSessionDto>()
                .FirstAsync();

            // 如果存在，直接返回
            if (activeUseCaseDiagram != null)
            {
                return activeUseCaseDiagram;
            }

            // 如果不存在，创建一个新的激活用例图会话
            var entity = new UseCaseDiagramSession
            {
                Name = "未命名用例图",
                IsActive = true,
                ActorsJson = "[]",
                UseCasesJson = "[]",
                RelationshipsJson = "[]"
            };

            entity = await _sqlSugarClient.Insertable(entity).ExecuteReturnEntityAsync();

            return await GetByIdAsync(entity.Id);
        }

        /// <summary>
        /// 创建用例图会话
        /// </summary>
        public async Task<Guid> CreateAsync(CreateUseCaseDiagramSessionInput input)
        {
            var userId = _currentUser.GetUserId();

            // 如果设置为激活，先取消其他激活的会话
            if (input.IsActive)
            {
                var activeSessions = await _sqlSugarClient.Queryable<UseCaseDiagramSession>()
                    .Where(x => x.CreatorId == userId)
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

            var entity = new UseCaseDiagramSession
            {
                Name = input.Name,
                IsActive = input.IsActive,
                ActorsJson = input.ActorsJson,
                UseCasesJson = input.UseCasesJson,
                RelationshipsJson = input.RelationshipsJson,
                UserPrompt = input.UserPrompt,
                AiInputContent = input.AiInputContent,
                AiOutputResult = input.AiOutputResult
            };

            entity = await _sqlSugarClient.Insertable(entity).ExecuteReturnEntityAsync();
            return entity.Id;
        }

        /// <summary>
        /// 更新用例图会话
        /// </summary>
        public async Task UpdateAsync(UpdateUseCaseDiagramSessionInput input)
        {
            var userId = _currentUser.GetUserId();
            var entity = await _sqlSugarClient.Queryable<UseCaseDiagramSession>()
                .Where(x => x.Id == input.Id)
                .Where(x => x.CreatorId == userId)
                .FirstAsync();

            if (entity == null)
            {
                throw new Exception("用例图会话不存在");
            }

            // 如果设置为激活，先取消其他激活的会话
            if (input.IsActive && !entity.IsActive)
            {
                var activeSessions = await _sqlSugarClient.Queryable<UseCaseDiagramSession>()
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
            entity.IsActive = input.IsActive;
            entity.ActorsJson = input.ActorsJson;
            entity.UseCasesJson = input.UseCasesJson;
            entity.RelationshipsJson = input.RelationshipsJson;
            entity.UserPrompt = input.UserPrompt;
            entity.AiInputContent = input.AiInputContent;
            entity.AiOutputResult = input.AiOutputResult;

            await _sqlSugarClient.Updateable(entity).ExecuteCommandAsync();
        }

        /// <summary>
        /// 设置激活的用例图会话
        /// </summary>
        public async Task SetActiveUseCaseDiagramSessionAsync(SetActiveUseCaseDiagramSessionInput input)
        {
            var userId = _currentUser.GetUserId();
            var entity = await _sqlSugarClient.Queryable<UseCaseDiagramSession>()
                .Where(x => x.Id == input.Id)
                .Where(x => x.CreatorId == userId)
                .FirstAsync();

            if (entity == null)
            {
                throw new Exception("用例图会话不存在");
            }

            // 先查询出其他激活的用例图会话
            var activeSessions = await _sqlSugarClient.Queryable<UseCaseDiagramSession>()
                .Where(x => x.CreatorId == userId)
                .Where(x => x.Id != input.Id)
                .Where(x => x.IsActive == true)
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

            // 设置当前用例图会话为激活
            entity.IsActive = true;
            await _sqlSugarClient.Updateable(entity)
                .UpdateColumns(x => new { x.IsActive })
                .ExecuteCommandAsync();
        }

        /// <summary>
        /// 删除用例图会话
        /// </summary>
        public async Task DeleteAsync(Guid id)
        {
            var userId = _currentUser.GetUserId();
            await _sqlSugarClient.Updateable<UseCaseDiagramSession>()
                .SetColumns(x => x.IsDeleted == true)
                .Where(x => x.Id == id)
                .Where(x => x.CreatorId == userId)
                .ExecuteCommandAsync();
        }

        /// <summary>
        /// AI生成用例图相关数据
        /// </summary>
        public async Task<UseCaseDiagramSessionDto> GenerateUseCaseDiagramByAIAsync(GenerateUseCaseDiagramByAIInput input)
        {
            var userId = _currentUser.GetUserId();

            // 获取会话信息
            var session = await _sqlSugarClient.Queryable<UseCaseDiagramSession>()
                .Where(x => x.Id == input.UseCaseDiagramSessionId)
                .Where(x => x.CreatorId == userId)
                .FirstAsync();

            if (session == null)
            {
                throw new Exception("用例图会话不存在");
            }

            if (string.IsNullOrWhiteSpace(input.UserPrompt))
            {
                throw new Exception("功能提示词不能为空");
            }

            // 构建系统提示词（固定部分）
            var systemPrompt = @"你是专业的业务分析师和用例图设计师。根据用户的业务需求，设计清晰合理的用例图。

【核心原则】
1. 参与者（Actor）：代表与系统交互的外部实体，通常是不同角色的用户
2. 用例（Use Case）：代表系统提供的功能或服务
3. 关系（Relationship）：包括关联关系（参与者与用例）、包含关系（include）、扩展关系（extend）

【设计规则】
1. 参与者设计：
   - 从需求中识别不同的角色（如管理员、普通用户、访客、系统等）
   - 参与者名称要清晰明确
   - 避免过于细分，合并相似角色

2. 用例设计：
   - 用例名称使用动词+名词形式（如创建订单、查看商品、审核申请）
   - 每个用例应该是一个完整的功能单元
   - 用例粒度适中，不要过于细碎
   - 从需求中提取所有业务功能作为用例

3. 关联关系设计：
   - 明确哪些参与者使用哪些用例
   - 一个用例可以被多个参与者使用
   - 一个参与者可以使用多个用例

4. Include关系设计（包含）：
   - 当一个用例的实现必然包含另一个用例时使用include
   - 例如：下订单 include 选择商品
   - 表示必须执行的子功能

5. Extend关系设计（扩展）：
   - 当一个用例在特定条件下可能触发另一个用例时使用extend
   - 例如：支付订单 extend 使用优惠券
   - 表示可选的额外功能

6. 数量控制：
   - 参与者数量：2-6个
   - 用例数量：5-15个
   - 每个参与者关联的用例：2-8个
   - Include/Extend关系：适度使用，不超过5个

【输出格式】
请严格按照以下JSON格式输出，不要添加任何markdown标记或代码块标记：
{
  ""actors"": [""参与者1"", ""参与者2""],
  ""useCases"": [
    {
      ""name"": ""用例名称"",
      ""actors"": [""参与该用例的参与者1"", ""参与者2""]
    }
  ],
  ""relationships"": [
    {
      ""from"": ""源用例名称"",
      ""to"": ""目标用例名称"",
      ""type"": ""include或extend""
    }
  ]
}

【示例】
需求：电商系统，管理员可以管理商品和审核订单，用户可以浏览商品、下单、支付，支付时可以使用优惠券

输出：
{
  ""actors"": [""管理员"", ""用户""],
  ""useCases"": [
    {""name"": ""管理商品"", ""actors"": [""管理员""]},
    {""name"": ""审核订单"", ""actors"": [""管理员""]},
    {""name"": ""浏览商品"", ""actors"": [""用户""]},
    {""name"": ""下订单"", ""actors"": [""用户""]},
    {""name"": ""选择商品"", ""actors"": [""用户""]},
    {""name"": ""支付订单"", ""actors"": [""用户""]},
    {""name"": ""使用优惠券"", ""actors"": [""用户""]}
  ],
  ""relationships"": [
    {""from"": ""下订单"", ""to"": ""选择商品"", ""type"": ""include""},
    {""from"": ""支付订单"", ""to"": ""使用优惠券"", ""type"": ""extend""}
  ]
}

请根据以上规则，深入分析业务需求，设计合理的用例图JSON。注意：只输出纯JSON，不要包含任何其他文字或标记。";

            // 构建用户输入内容（变化部分）
            var userContentBuilder = new StringBuilder();
            userContentBuilder.AppendLine("=== 业务需求 ===");
            userContentBuilder.AppendLine(input.UserPrompt);

            var aiInputContent = userContentBuilder.ToString();

            var useCaseDiagramResult = await _deepSeekService.ChatAsync(aiInputContent, systemPrompt);

            // 保存到数据库
            session.UserPrompt = input.UserPrompt;
            session.AiInputContent = aiInputContent;
            session.AiOutputResult = useCaseDiagramResult;

            await _sqlSugarClient.Updateable(session)
                .UpdateColumns(x => new { x.UserPrompt, x.AiInputContent, x.AiOutputResult })
                .ExecuteCommandAsync();

            return await GetByIdAsync(session.Id);
        }
    }
}
