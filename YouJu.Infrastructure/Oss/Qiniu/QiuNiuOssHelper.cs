using Microsoft.Extensions.Configuration;
using Qiniu.Common;
using Qiniu.Http;
using Qiniu.IO;
using Qiniu.IO.Model;
using Qiniu.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace YouJu.Infrastructure.Oss.Qiniu
{
    public class QiuNiuOssHelper : IQiuNiuOssHelper
    {

        private readonly IConfiguration _configuration;


        public QiuNiuOssHelper(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        public Task<string> GetDomain()
        {
            return Task.FromResult(_configuration["Oss:Qiniu:Domain"]);
        }


        public Task UploadFile(string saveKey, string localFile)
        {

            // 生成(上传)凭证时需要使用此Mac
            // 这个示例单独使用了一个Settings类，其中包含AccessKey和SecretKey
            // 实际应用中，请自行设置您的AccessKey和SecretKey
            Mac mac = new Mac(_configuration["Oss:Qiniu:AccessKey"], _configuration["Oss:Qiniu:SecretKey"]);
            string bucket = _configuration["Oss:Qiniu:Bucket"];

            // 上传策略，参见 
            // https://developer.qiniu.com/kodo/manual/put-policy
            PutPolicy putPolicy = new PutPolicy();
            // 如果需要设置为"覆盖"上传(如果云端已有同名文件则覆盖)，请使用 SCOPE = "BUCKET:KEY"
            // putPolicy.Scope = bucket + ":" + saveKey;
            putPolicy.Scope = bucket;
            // 上传策略有效期(对应于生成的凭证的有效期)          
            putPolicy.SetExpires(3600);
            // 上传到云端多少天后自动删除该文件，如果不设置（即保持默认默认）则不删除

            // 生成上传凭证，参见
            // https://developer.qiniu.com/kodo/manual/upload-token            
            string jstr = putPolicy.ToJsonString();
            string token = Auth.CreateUploadToken(mac, jstr);
            UploadManager um = new UploadManager();
            HttpResult result = um.UploadFile(localFile, saveKey, token);
            Console.WriteLine(result.ToString());
            return Task.CompletedTask;
        }

        public Task<string> UploadFileStream(string saveKey, Stream stream)
        {

            // 生成(上传)凭证时需要使用此Mac
            // 这个示例单独使用了一个Settings类，其中包含AccessKey和SecretKey
            // 实际应用中，请自行设置您的AccessKey和SecretKey

            var accessKey = _configuration["Oss:Qiniu:AccessKey"];
            var secretKey = _configuration["Oss:Qiniu:SecretKey"];
            var bucket = _configuration["Oss:Qiniu:Bucket"];




            Mac mac = new Mac(accessKey, secretKey);


            //亚太-新加坡（原东南亚）	as0	空间管理：http(s)://uc.qiniuapi.com
            // 源站上传：http(s)://up-as0.qiniup.com
            // 源站下载：http(s)://iovip-as0.qiniuio.com
            // 对象管理：http(s)://rs-as0.qiniuapi.com
            // 对象列举：http(s)://rsf-as0.qiniuapi.com
            //计量查询：http(s)://api.qiniuapi.com

            var zone = new Zone()
            {
                UpHost = "https://up-as0.qiniup.com",
                IovipHost = "https://iovip-as0.qiniuio.com",
                RsHost = "https://rs-as0.qiniuapi.com",
                RsfHost = "https://rsf-as0.qiniuapi.com",
                ApiHost = "https://api.qiniuapi.com"
            };

            //设置 Zone
            Config.ZONE = zone;


            // 上传策略，参见 
            // https://developer.qiniu.com/kodo/manual/put-policy
            PutPolicy putPolicy = new PutPolicy();
            // 如果需要设置为"覆盖"上传(如果云端已有同名文件则覆盖)，请使用 SCOPE = "BUCKET:KEY"
            // putPolicy.Scope = bucket + ":" + saveKey;
            putPolicy.Scope = bucket;

            // 上传策略有效期(对应于生成的凭证的有效期)          
            putPolicy.SetExpires(3600);
            // 上传到云端多少天后自动删除该文件，如果不设置（即保持默认默认）则不删除

            // 生成上传凭证，参见
            // https://developer.qiniu.com/kodo/manual/upload-token            
            string jstr = putPolicy.ToJsonString();
            string token = Auth.CreateUploadToken(mac, jstr);
            UploadManager um = new UploadManager();

            HttpResult result = um.UploadStream(stream, saveKey, token);

            if (result.Code == 200)
            {
                return Task.FromResult($"{_configuration["Oss:Qiniu:Domain"]}/{saveKey}");
            }
            else
            {
                return Task.FromResult(string.Empty);
            }
        }



    }
}
