using Web.HttpClient;

namespace Web.HttpClientApi.DeepSeek.Service
{
    /// <summary>
    /// DeepSeek API 服务接口
    /// </summary>
    public interface IDeepSeekService
    {
        /// <summary>
        /// 获取系统提示
        /// </summary>
        string GetSystemPrompt(DeepSeekSystemPrompt deepSeekSystemPrompt);
        /// <summary>
        /// 发送聊天请求
        /// </summary>
        Task<TResult> ChatCompletionAsync<TResult>(string systemPrompt, string requestData, int max_tokens = 2000);

        /// <summary>
        /// 发送聊天请求并返回字符串结果
        /// </summary>
        Task<string> ChatAsync(string userContent, string systemPrompt, int max_tokens = 4000);
    }
}
