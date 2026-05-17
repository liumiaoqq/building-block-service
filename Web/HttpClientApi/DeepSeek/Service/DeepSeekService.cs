using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Web.Dto;
using Newtonsoft.Json.Linq;
using Web.HttpClient;
using Polly;
using System;
using System.Threading.Tasks;

namespace Web.HttpClientApi.DeepSeek.Service

{
    /// <summary>
    /// DeepSeek API 服务
    /// </summary>
    public class DeepSeekService : IDeepSeekService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<DeepSeekService> _logger;
        public DeepSeekService(IHttpClientFactory httpClientFactory, ILogger<DeepSeekService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <summary>
        /// 获取系统提示
        /// </summary>
        public string GetSystemPrompt(DeepSeekSystemPrompt deepSeekSystemPrompt)
        {
            var content = "";
            if (deepSeekSystemPrompt == DeepSeekSystemPrompt.TableStructureNameCamelCase)
            {
                content = $@"您是一个专业的表设计专家，请处理用户输入的表结构，将表结构中的字段名称转换为驼峰命名，并输出为JSON格式.\n\n
                               示例JSON输出格式：
                               {{
                               ""Tables"": 
                               [
                               {{
                                ""TableName"": ""activityrecord"",
                                ""NewTableName"": ""ActivityRecord"",
                                }},
                                {{
                                ""TableName"": ""appointmentdet"",
                                ""NewTableName"": ""AppointmentDet"",
                                }},
                                {{
                                ""TableName"": ""buy_card"",
                                ""NewTableName"": ""BuyCard"",
                                }},
                                {{
                                ""TableName"": ""order"",
                                ""NewTableName"": ""Order"",
                                }},
                                {{
                                ""TableName"": ""goodProp"",
                                ""NewTableName"": ""GoodProp"",
                                }},
                                {{
                                ""TableName"": ""Messagenotice"",
                                ""NewTableName"": ""MessageNotice"",
                                }}
                               ]
                               }}";
                return content;
            }
            else if (deepSeekSystemPrompt == DeepSeekSystemPrompt.TableStructureLengthAdjustment)
            {
                content = $@"您是一个专业的数据库表结构优化专家，请分析用户提供的表结构信息，并根据实际数据内容推荐合适的列长度调整方案，输出为JSON格式。
                    请确保您的分析考虑以下因素：
                    1. 当前列的实际数据长度分布
                    2. 数据类型的最佳实践
                    3. 存储空间优化与数据完整性的平衡

                    示例JSON输出格式：
                    {{
                    ""Columns"": [
                    {{
                        ""TableCode"": ""ActivityRecord"",
                        ""TableName"": ""活动记录"",
                        ""ColumnCode"": ""UserName"",
                        ""ColumnName"": ""用户名"",
                        ""NewLength"": ""20"",
                        ""OldLength"": ""100""
                    }},
                    {{
                        ""TableCode"": ""ActivityRecord"",
                        ""TableName"": ""活动记录"",
                        ""ColumnCode"": ""Description"",
                        ""ColumnName"": ""描述"",
                        ""NewLength"": ""200"",
                        ""OldLength"": ""500""
                    }},
                    {{
                        ""TableCode"": ""ActivityRecord"",
                        ""TableName"": ""活动记录"",
                        ""ColumnCode"": ""Email"",
                        ""ColumnName"": ""邮箱"",
                        ""NewLength"": ""80"",
                        ""OldLength"": ""50""
                    }},
                    {{
                        ""TableCode"": ""ActivityRecord"",
                        ""TableName"": ""活动记录"",
                        ""ColumnCode"": ""Phone"",
                        ""ColumnName"": ""手机号"",
                        ""NewLength"": ""11"",
                        ""OldLength"": ""100""
                    }}
                    ]
                    }}

                    请根据用户提供的表结构信息，分析并推荐适当的列长度调整方案。";
                return content;
            }
            else if (deepSeekSystemPrompt == DeepSeekSystemPrompt.TableStructureRelationShip)
            {
                content = $@"您是一个专业的数据库表结构优化专家，请分析用户提供的表结构信息，并根据实际数据内容推荐合适的表关系调整方案，输出为JSON格式。

                    示例JSON输出格式：
                    {{
                    ""Tables"": [
                     {{
                    ""TableCode"": ""ActivityRecord"",
                    ""TableName"": ""活动记录表"",
                    ""RefTableCode"": ""User"",
                    ""RefTableName"": ""用户表"",
                    ""RefColumnCode"": ""UserId"",
                    ""RefColumnName"": ""用户名"",
                    }},
                    {{
                    ""TableCode"": ""ActivityRecord"",
                    ""TableName"": ""活动记录表"",
                    ""RefTableCode"": ""Activity"",
                    ""RefTableName"": ""活动表"",
                    ""RefColumnCode"": ""ActivityId"",
                    ""RefColumnName"": ""所属活动"",
                    }}
                    ]
                    }}

                    请根据用户提供的表结构信息，分析并推荐适当的表关系调整方案。";
                return content;
            }
            else if (deepSeekSystemPrompt == DeepSeekSystemPrompt.EnumParse)
            {
                content = @"您是一个专业的程序员，请分析用户提供的代码或则SQL脚本，并根据实际数据内容提取出枚举的值，输出为JSON格式。
                示例JSON输出格式：
                {
                   ""Enums"": [
                    {
                    ""Name"": ""状态"",
                    ""Code"": ""Status"",
                    ""EnumPropsList"": [
                        {
                            ""Name"": ""未开始"",
                            ""Value"": ""0""
                        },
                        {
                            ""Name"": ""进行中"",
                            ""Value"": ""1""
                        },
                        {
                            ""Name"": ""已完成"",
                            ""Value"": ""2""
                        }
                    ]
                    }
                    ]
                }
                ";

                return content;
            }
            else if (deepSeekSystemPrompt == DeepSeekSystemPrompt.TableColumnSort)
            {
                content = @"您是一个专业的数据库表结构优化专家，请分析用户提供的表结构信息，并根据用户使用行为和最佳实践，为表格列提供合理的显示顺序建议，输出为JSON格式。
                
                请遵循以下优化原则：
                1. 重要且常用的字段应该排在前面（如：名称、编号、状态）
                2. 关联字段应该靠近相关字段（如：用户ID和用户名）
                3. 时间字段通常放在后面（如：创建时间、更新时间）
                4. 描述性字段放在中间（如：描述、备注）
                5. 统计类字段可以靠后（如：数量、金额）
                
                示例JSON输出格式：
                {
                    ""Tables"": [
                        {
                            ""TableName"": ""活动记录"",
                            ""Columns"": [
                                {
                                    ""ColumnCode"": ""Name"",
                                    ""ColumnName"": ""活动名称"",
                                    ""NewSort"": 1,
                                    ""Reason"": ""名称字段应该放在第一位，方便用户快速识别记录""
                                },
                                {
                                    ""ColumnCode"": ""Status"",
                                    ""ColumnName"": ""状态"",
                                    ""NewSort"": 2,
                                    ""Reason"": ""状态是常用的筛选和查看字段，应该靠前显示""
                                },
                                {
                                    ""ColumnCode"": ""UserId"",
                                    ""ColumnName"": ""用户ID"",
                                    ""NewSort"": 3,
                                    ""Reason"": ""关联字段，用于标识所属用户""
                                },
                                {
                                    ""ColumnCode"": ""Description"",
                                    ""ColumnName"": ""描述"",
                                    ""NewSort"": 4,
                                    ""Reason"": ""描述性字段，放在基本信息之后""
                                },
                                {
                                    ""ColumnCode"": ""CreatedAt"",
                                    ""ColumnName"": ""创建时间"",
                                    ""NewSort"": 5,
                                    ""Reason"": ""时间字段通常放在后面""
                                }
                            ]
                        }
                    ]
                }
                
                请根据用户提供的表结构信息，分析并推荐最佳的列显示顺序，并为每个调整提供合理的理由。";

                return content;
            }
            else if (deepSeekSystemPrompt == DeepSeekSystemPrompt.ProjectStructureGeneration)
            {
                content = @"您是一个专业的项目架构师，请根据用户输入的需求描述，生成一个合理的项目结构树，输出为JSON格式。

                请遵循以下原则：
                1. 项目结构应该清晰明了，层次分明
                2. 根据项目类型（如：Web系统、移动应用、后台管理等）组织合理的模块
                3. 常见的模块包括：用户管理、角色管理、权限管理、系统设置等
                4. 每个模块下可以包含具体的功能点
                5. ID必须是唯一的整数，从1开始递增
                6. Pid表示父节点ID，根节点的Pid为0
                7. IsLeaf表示是否为叶子节点，如果有子节点则为false，否则为true
                
                示例JSON输出格式：
                {
                    ""ProjectTree"": {
                        ""Name"": ""在线教育管理系统"",
                        ""Id"": 1,
                        ""Pid"": 0,
                        ""IsLeaf"": false,
                        ""Children"": [
                            {
                                ""Name"": ""学生端"",
                                ""Id"": 2,
                                ""Pid"": 1,
                                ""IsLeaf"": false,
                                ""Children"": [
                                    {
                                        ""Name"": ""课程浏览"",
                                        ""Id"": 3,
                                        ""Pid"": 2,
                                        ""IsLeaf"": true,
                                        ""Children"": []
                                    },
                                    {
                                        ""Name"": ""在线学习"",
                                        ""Id"": 4,
                                        ""Pid"": 2,
                                        ""IsLeaf"": true,
                                        ""Children"": []
                                    },
                                    {
                                        ""Name"": ""作业提交"",
                                        ""Id"": 5,
                                        ""Pid"": 2,
                                        ""IsLeaf"": true,
                                        ""Children"": []
                                    }
                                ]
                            },
                            {
                                ""Name"": ""教师端"",
                                ""Id"": 6,
                                ""Pid"": 1,
                                ""IsLeaf"": false,
                                ""Children"": [
                                    {
                                        ""Name"": ""课程管理"",
                                        ""Id"": 7,
                                        ""Pid"": 6,
                                        ""IsLeaf"": true,
                                        ""Children"": []
                                    },
                                    {
                                        ""Name"": ""作业批改"",
                                        ""Id"": 8,
                                        ""Pid"": 6,
                                        ""IsLeaf"": true,
                                        ""Children"": []
                                    }
                                ]
                            },
                            {
                                ""Name"": ""管理员"",
                                ""Id"": 9,
                                ""Pid"": 1,
                                ""IsLeaf"": false,
                                ""Children"": [
                                    {
                                        ""Name"": ""用户管理"",
                                        ""Id"": 10,
                                        ""Pid"": 9,
                                        ""IsLeaf"": true,
                                        ""Children"": []
                                    },
                                    {
                                        ""Name"": ""系统设置"",
                                        ""Id"": 11,
                                        ""Pid"": 9,
                                        ""IsLeaf"": true,
                                        ""Children"": []
                                    }
                                ]
                            }
                        ]
                    }
                }
                
                请根据用户的需求描述，生成一个合理的项目结构树。";

                return content;
            }
            else if (deepSeekSystemPrompt == DeepSeekSystemPrompt.PlantUMLGeneration)
            {
                content = @"您是一个专业的软件工程师和流程分析专家，请根据用户提供的代码项目结构和具体需求，分析代码逻辑并生成PlantUML活动图语法，输出为JSON格式。

                请遵循以下原则：
                1. 仔细分析代码中的业务流程和逻辑
                2. 识别关键的流程步骤、条件判断和循环
                3. 生成清晰、准确的PlantUML活动图语法
                4. 使用PlantUML标准语法：@startuml开始，@enduml结束
                5. 合理使用:活动;、if/then/else、while等语法
                6. 为复杂流程提供简要说明
                
                示例JSON输出格式：
                {
                    ""PlantUMLCode"": ""@startuml\nstart\n:用户登录;\n:验证用户名和密码;\nif (验证是否成功?) then (是)\n  :加载用户信息;\nelse (否)\n  :提示错误信息;\nendif\n:跳转到首页;\nstop\n@enduml"",
                    ""Description"": ""这是一个用户登录的基本流程图，包含了验证逻辑和条件判断。""
                }
                
                请根据用户提供的代码项目信息和需求，分析并生成对应的PlantUML流程图。";

                return content;
            }
            else if (deepSeekSystemPrompt == DeepSeekSystemPrompt.CodeModuleClassification)
            {
                content = @"您是一个专业的软件架构师和代码分析专家，请根据用户提供的代码项目文件列表（包含文件名、扩展名和ID），分析代码结构并按业务功能模块进行智能分类，输出为JSON格式。

                重要说明：
                - 文件列表已经过滤掉了工具类、配置类、样式文件等非业务代码
                - 请专注于识别业务逻辑模块，不要创建工具类、配置类等通用模块
                - 文件列表只提供文件名和扩展名，请根据这些信息进行智能分析

                请遵循以下原则：
                1. 根据文件名、文件扩展名识别业务功能模块
                2. 模块分类应该按照业务领域划分，如：用户管理、订单管理、商品管理、支付管理、库存管理等
                3. 同一业务领域相关的文件应归类到同一模块（包括前端和后端）
                4. 为每个模块提供清晰的业务名称和描述
                5. 每个模块包含该模块下的文件ID列表（使用提供的数字ID，如""1"", ""2"", ""3""等）
                6. 模块路径应该反映业务领域的层级结构
                7. 每个模块应该是一个完整的业务领域，可能包含Controller、Service、Manager、Model、前端页面（Vue/React组件）等多个层次的文件
                8. 前端和后端属于同一业务的文件应归入同一模块，例如UserController.cs和user/index.vue应归入""用户管理""模块


                示例JSON输出格式：
                {
                    ""Modules"": [
                        {
                        ""ModuleName"": ""用户管理"",
                            ""ModulePath"": ""business / user"",
                            ""Description"": ""用户注册、登录、认证授权、个人信息管理等相关业务功能，包含前端页面和后端接口"",
                            ""FileIds"": [""1"", ""2"", ""3"", ""4"", ""15"", ""16""]
                        },
                        {
                        ""ModuleName"": ""订单管理"",
                            ""ModulePath"": ""business / order"",
                            ""Description"": ""订单创建、查询、状态流转、订单评价等相关业务功能，包含前端页面和后端接口"",
                            ""FileIds"": [""5"", ""6"", ""7"", ""17"", ""18""]
                        },
                        {
                        ""ModuleName"": ""商品管理"",
                            ""ModulePath"": ""business / product"",
                            ""Description"": ""商品信息管理、分类、SKU、库存等相关业务功能，包含前端页面和后端接口"",
                            ""FileIds"": [""8"", ""9"", ""10"", ""19""]
                        },
                        {
                        ""ModuleName"": ""支付管理"",
                            ""ModulePath"": ""business / payment"",
                            ""Description"": ""订单支付、退款、支付回调等相关业务功能，包含前端页面和后端接口"",
                            ""FileIds"": [""11"", ""12"", ""20""]
                        }
                    ]
                }

                注意事项：
                1.请直接使用用户提供的文件列表中的数字ID，不要修改或生成新的ID
                2.专注于业务模块识别，避免创建技术性模块（如工具类、通用组件等）
                3.模块名称应该体现业务含义，而非技术实现
                4.每个模块应该是一个完整的业务领域，包含Controller、Service、Manager、前端页面等多个层次
                5.前端和后端属于同一业务的文件必须归入同一模块，形成完整的业务功能闭环

                请根据用户提供的文件列表，智能分析并生成合理的业务模块分类方案。";

                return content;
            }
            else if (deepSeekSystemPrompt == DeepSeekSystemPrompt.PlantUMLSequenceDiagramGeneration)
            {
                content = @"您是一个专业的软件工程师和时序分析专家，请根据用户提供的代码项目结构和具体需求，分析代码交互逻辑并生成PlantUML时序图语法，输出为JSON格式。

                请遵循以下原则：
                1. 仔细分析代码中各组件、类、服务之间的交互和调用关系
                2. 识别关键的参与者（Actor）、对象、服务等
                3. 生成清晰、准确的PlantUML时序图语法
                4. 使用PlantUML标准语法：@startuml开始，@enduml结束
                5. 合理使用participant、actor、->、-->、activate、deactivate等语法
                6. 体现完整的调用链路和交互时序
                7. 为复杂交互提供简要说明
                
                示例JSON输出格式：
                {
                    ""PlantUMLCode"": ""@startuml\nactor 用户\nparticipant 前端页面\nparticipant Controller\nparticipant Service\nparticipant Database\n\n用户 -> 前端页面: 点击登录按钮\nactivate 前端页面\n前端页面 -> Controller: POST /login\nactivate Controller\nController -> Service: 验证用户信息\nactivate Service\nService -> Database: 查询用户\nactivate Database\nDatabase --> Service: 返回用户数据\ndeactivate Database\nService --> Controller: 验证结果\ndeactivate Service\nController --> 前端页面: 返回Token\ndeactivate Controller\n前端页面 --> 用户: 登录成功\ndeactivate 前端页面\n@enduml"",
                    ""Description"": ""这是一个用户登录的时序图，展示了从前端到后端再到数据库的完整调用链路。""
                }
                
                请根据用户提供的代码项目信息和需求，分析并生成对应的PlantUML时序图。";

                return content;
            }
            else if (deepSeekSystemPrompt == DeepSeekSystemPrompt.EnumBinding)
            {
                content = @"您是一个专业的数据库设计专家，请分析用户提供的表结构和枚举列表，智能匹配哪些列应该使用枚举类型，输出为JSON格式。

                重要说明：
                - 提供的列都是整型类型
                - 这些列都不是外键（不以Id结尾）
                - 这些列可能是枚举值字段

                请遵循以下原则：
                1. 分析列的名称和含义，判断该列是否适合使用枚举
                2. 常见的枚举场景：状态(Status)、类型(Type)、级别(Level)、性别(Gender)、分类(Category)等固定选项的字段
                3. 根据列名和枚举名进行语义匹配
                4. 只匹配确定性高的列，不确定的不要匹配
                5. 一个列只能匹配一个枚举
                6. 优先匹配名称相似度高的
                
                示例JSON输出格式：
                {
                    ""Bindings"": [
                        {
                            ""TableCode"": ""Order"",
                            ""ColumnCode"": ""Status"",
                            ""MatchedEnumCode"": ""OrderStatusEnum""
                        },
                        {
                            ""TableCode"": ""User"",
                            ""ColumnCode"": ""Gender"",
                            ""MatchedEnumCode"": ""GenderEnum""
                        },
                        {
                            ""TableCode"": ""Product"",
                            ""ColumnCode"": ""Category"",
                            ""MatchedEnumCode"": ""ProductCategoryEnum""
                        }
                    ]
                }
                
                请根据用户提供的表结构和枚举信息，分析并返回合适的列与枚举的绑定关系。只返回确定性高的匹配结果，不确定的不要返回。";

                return content;
            }
            else if (deepSeekSystemPrompt == DeepSeekSystemPrompt.ColumnPropTypeBinding)
            {
                content = @"您是一个专业的数据库设计专家，请分析用户提供的表结构中的字符串类型列，智能判断这些列是否应该使用更具体的类型，输出为JSON格式。

                重要说明：
                - 提供的列都是字符串(String)类型
                - 需要判断这些列是否应该改为更具体的类型
                - 可用的类型包括：Image(图片)、Video(视频)、Audio(音频)、File(文件)、MultiText(多行文本)、Text(长文本)

                请遵循以下原则：
                1. 根据列名称判断其应该使用的类型
                2. 常见的类型映射：
                   - Image/图片：头像(Avatar)、图片(Image/Photo/Pic)、二维码(QrCode)、封面(Cover)、Logo等
                   - Video/视频：视频(Video/VideoUrl/VideoUrls)、视频链接等
                   - Audio/音频：音频(Audio/AudioUrl/AudioUrls)、语音等
                   - File/文件：文件(File/FileUrl/FileUrls)、附件(Attachment)等
                   - MultiText/多行文本：简介(Introduction)、摘要(Summary)、备注(Remark)等
                   - Text/长文本：详情(Detail/Details)、内容(Content)、描述(Description)等
                3. 只匹配确定性高的列，不确定的不要匹配
                4. 如果列名很通用（如Name、Title、Code等），保持字符串类型，不要匹配

                可用类型列表：
                - Image: 图片类型
                - Video: 视频类型
                - Audio: 音频类型
                - File: 文件类型
                - MultiText: 多行文本类型
                - Text: 长文本类型
                
                示例JSON输出格式：
                {
                    ""Bindings"": [
                        {
                            ""TableCode"": ""User"",
                            ""ColumnCode"": ""Avatar"",
                            ""SuggestedType"": ""Image""
                        },
                        {
                            ""TableCode"": ""Product"",
                            ""ColumnCode"": ""VideoUrl"",
                            ""SuggestedType"": ""Video""
                        },
                        {
                            ""TableCode"": ""Article"",
                            ""ColumnCode"": ""Content"",
                            ""SuggestedType"": ""Text""
                        },
                        {
                            ""TableCode"": ""Order"",
                            ""ColumnCode"": ""Remark"",
                            ""SuggestedType"": ""MultiText""
                        }
                    ]
                }
                
                请根据用户提供的表结构信息，分析并返回合适的列类型建议。只返回确定性高的匹配结果，不确定的不要返回。";

                return content;
            }
            else if (deepSeekSystemPrompt == DeepSeekSystemPrompt.PrimaryDisplayColumn)
            {
                content = @"您是一个专业的数据库设计专家，请分析用户提供的表结构，为每个表从字符串类型的列中选择一个最适合作为""主显示列""的字段，输出为JSON格式。

                重要说明：
                - 主显示列用于在其他表关联时显示该表的代表性信息
                - 输入格式：TableCode为表编码，Columns为逗号分隔的字符串列编码
                - 每个表必须且只能选择一个主显示列

                请遵循以下优先级原则（从高到低）：
                1. 名称类字段优先级最高：Name、Title、Label、DisplayName、Caption等
                2. 标题类字段次之：Subject、Headline等
                3. 编号类字段再次：Code、No、Number、SerialNumber等
                4. 如果以上都没有，选择最能代表该表记录的字符串字段
                5. 避免选择：备注(Remark)、描述(Description)、内容(Content)、路径(Path/Url)等辅助性字段

                示例输入：
                [
                    {""TableCode"": ""User"", ""Columns"": ""Name,Remark,Avatar""},
                    {""TableCode"": ""Product"", ""Columns"": ""Title,Description,Code""},
                    {""TableCode"": ""Order"", ""Columns"": ""OrderNo,Remark""}
                ]

                示例JSON输出格式：
                {
                    ""Tables"": [
                        {""TableCode"": ""User"", ""PrimaryColumnCode"": ""Name""},
                        {""TableCode"": ""Product"", ""PrimaryColumnCode"": ""Title""},
                        {""TableCode"": ""Order"", ""PrimaryColumnCode"": ""OrderNo""}
                    ]
                }
                
                请根据用户提供的表结构信息，为每个表选择最合适的主显示列。";

                return content;
            }
            return "";
        }



        /// <summary>
        /// 发送聊天请求
        /// </summary>
        public async Task<TResult> ChatCompletionAsync<TResult>(string systemPrompt, string requestData, int max_tokens = 2000)
        {
            // 定义重试策略
            var retryPolicy = Policy
                .Handle<JsonException>()
                .WaitAndRetryAsync(
                    3, // 最大重试3次
                    retryAttempt => TimeSpan.FromMilliseconds(1), // 直接请求
                    (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning($"DeepSeek API返回数据解析失败，正在进行第{retryCount}次重试。异常信息: {exception.Message}");
                    }
                );

            return await retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var client = _httpClientFactory.CreateClient("DeepSeek");

                    // 构建请求体
                    var requestBody = new
                    {
                        model = "deepseek-chat",
                        response_format = new
                        {
                            type = "json_object"
                        },
                        messages = new[] {
                            new {
                                role = "system",
                                content=systemPrompt,
                            },
                            new {
                                role = "user",
                                content=requestData,
                            }
                        },
                        max_tokens = max_tokens
                    };

                    var jsonContent = JsonSerializer.Serialize(requestBody);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("/v1/chat/completions", content);
                    response.EnsureSuccessStatusCode();

                    var responseBody = await response.Content.ReadAsStringAsync();
                    var jObject = JObject.Parse(responseBody);

                    // 修正JSON解析逻辑
                    var choices = jObject["choices"] as JArray;
                    if (choices == null || choices.Count == 0)
                    {
                        throw new YouJuException("DeepSeek API返回数据格式错误");
                    }

                    var message = choices[0]["message"];
                    var contentText = message["content"].ToString();

                    return JsonSerializer.Deserialize<TResult>(contentText);
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "DeepSeek API请求失败");
                    throw new YouJuException("DeepSeek API请求失败,请稍后再试");
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "DeepSeek API返回数据解析失败");
                    // 此处只记录异常，让策略决定是否重试
                    throw; // 重新抛出异常，由重试策略捕获
                }
            });
        }



        /// <summary>
        /// 发送聊天请求并返回字符串结果
        /// </summary>
        public async Task<string> ChatAsync(string userContent, string systemPrompt, int max_tokens = 4000)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("DeepSeek");

                // 构建请求体
                var requestBody = new
                {
                    model = "deepseek-chat",
                    response_format = new
                    {
                        type = "json_object"
                    },
                    messages = new[] {
                        new {
                            role = "system",
                            content = systemPrompt,
                        },
                        new {
                            role = "user",
                            content = userContent,
                        }
                    },
                    max_tokens = max_tokens
                };

                var jsonContent = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("/v1/chat/completions", content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseBody);

                // 解析返回结果
                var choices = jObject["choices"] as JArray;
                if (choices == null || choices.Count == 0)
                {
                    throw new YouJuException("DeepSeek API返回数据格式错误");
                }

                var message = choices[0]["message"];
                var contentText = message["content"].ToString();

                return contentText;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "DeepSeek API请求失败");
                throw new YouJuException("DeepSeek API请求失败,请稍后再试");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "DeepSeek API返回数据解析失败");
                throw new YouJuException("DeepSeek API返回数据解析失败");
            }
        }

    }
}