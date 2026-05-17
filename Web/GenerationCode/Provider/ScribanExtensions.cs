using Scriban;
using Scriban.Runtime;

namespace Web.GenerationCode.Provider
{
    /// <summary>
    /// 自定义字符串函数
    /// </summary>
    public class CustomStringFunctions : ScriptObject
    {
        public static string lowercase_first(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            return char.ToLower(input[0]) + input.Substring(1);
        }

        /// <summary>
        /// 将 PascalCase 转换为 snake_case
        /// 例如: UserName -> user_name, CreatedAt -> created_at, IsDeleted -> is_deleted
        /// </summary>
        public static string to_snake_case(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var result = new System.Text.StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (char.IsUpper(c))
                {
                    if (i > 0)
                        result.Append('_');
                    result.Append(char.ToLower(c));
                }
                else
                {
                    result.Append(c);
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// 将 URL 路径每段首字母转小写，保留驼峰格式
        /// 例如: /User/LIst -> /user/lIst, UserRelative/List -> userRelative/list
        /// </summary>
        public static string to_lower_url(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var segments = input.Split('/');
            for (int i = 0; i < segments.Length; i++)
            {
                if (!string.IsNullOrEmpty(segments[i]))
                {
                    segments[i] = char.ToLower(segments[i][0]) + segments[i].Substring(1);
                }
            }
            return string.Join("/", segments);
        }

        /// <summary>
        /// 从 JDBC 连接字符串中提取数据库名并转换为蛇形命名，返回完整URL
        /// 例如: jdbc:mysql://localhost:3306/RuralDoctorDiagnosisTreatment?useUnicode=true&characterEncoding=UTF-8&useSSL=false&serverTimezone=Asia/Shanghai&allowPublicKeyRetrieval=true
        /// -> jdbc:mysql://localhost:3306/rural_doctor_diagnosis_treatment?useUnicode=true&characterEncoding=UTF-8&useSSL=false&serverTimezone=Asia/Shanghai&allowPublicKeyRetrieval=true 
        /// </summary>
        public static string extract_mysql_db_snake_case(string jdbcUrl)
        {
            if (string.IsNullOrEmpty(jdbcUrl))
                return string.Empty;

            // 先分离查询参数
            var questionIndex = jdbcUrl.IndexOf('?');
            var urlWithoutParams = questionIndex >= 0 ? jdbcUrl.Substring(0, questionIndex) : jdbcUrl;
            var queryParams = questionIndex >= 0 ? jdbcUrl.Substring(questionIndex) : string.Empty;

            // 匹配 jdbc:mysql://host:port/ 之后的数据库名
            // 查找 :// 后的第一个 / (跳过协议部分)
            var protocolEnd = urlWithoutParams.IndexOf("://");
            if (protocolEnd < 0)
                return jdbcUrl;

            // 从协议之后查找第一个 / (主机:端口之后)
            var dbSlashIndex = urlWithoutParams.IndexOf('/', protocolEnd + 3);
            if (dbSlashIndex < 0 || dbSlashIndex >= urlWithoutParams.Length - 1)
                return jdbcUrl;

            // 提取数据库名
            var dbName = urlWithoutParams.Substring(dbSlashIndex + 1);
            if (string.IsNullOrEmpty(dbName))
                return jdbcUrl;

            // 转换数据库名为蛇形命名，重新拼接完整URL
            var snakeDbName = to_snake_case(dbName);
            return urlWithoutParams.Substring(0, dbSlashIndex + 1) + snakeDbName + queryParams;
        }
        
    }

    /// <summary>
    /// Scriban 渲染帮助类
    /// </summary>
    public static class ScribanHelper
    {
        public static string Render(string templateContent, object model, Func<System.Reflection.MemberInfo, string> memberRenamer)
        {
            try
            {
                var template = Template.Parse(templateContent);
                var context = new TemplateContext();
                context.MemberRenamer = member => memberRenamer(member);

                // 注册自定义函数 - 使用静态方法
                var functions = new ScriptObject();
                functions.Import(typeof(CustomStringFunctions));
                context.PushGlobal(functions);

                // 注册模型数据
                var scriptObject = new ScriptObject();
                scriptObject.Import(model, renamer: member => memberRenamer(member));
                context.PushGlobal(scriptObject);

                return template.Render(context);
            }
            catch (Exception ex) {
                return "转换失败,请联系管理员"+ex.Message;
            }
        }
    }
}
