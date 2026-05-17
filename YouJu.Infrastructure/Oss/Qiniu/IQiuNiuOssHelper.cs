using System.Threading.Tasks;
using System.IO;

namespace YouJu.Infrastructure.Oss.Qiniu
{
    public interface IQiuNiuOssHelper
    {
        public Task UploadFile(string saveKey, string localFile);

        public Task<string> UploadFileStream(string saveKey, Stream stream);

        public Task<string> GetDomain();
    }
}
