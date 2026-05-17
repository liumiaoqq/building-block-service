using Microsoft.AspNetCore.Hosting;

using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

using Utils.Zip;

using Web.GenerationCode.Dto;

namespace Web.GenerationCode
{
    public class GengerationTranferTreeFileService : IGengerationTranferTreeFileService
    {
        private readonly IConfiguration _configuration;

        private readonly IWebHostEnvironment _environment;

        private string _rootDir;

        private string _zipFileName;

        public GengerationTranferTreeFileService(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }
  

        public string GetTranferDownAddress(List<GengerationTranferTree> gengerationTranferTrees)
        {
        
            var uploadDirectory = Path.Combine(_environment.WebRootPath, SysConst.GengerationCodeRootDir, DateTime.Now.ToString("yyyy-MM-dd"));

            var tempDir = Path.Combine(uploadDirectory, _rootDir, "temp");

            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }



            GengerationTranferTreeRecurrence(gengerationTranferTrees, tempDir);

            var targetZipPath = Path.Combine(uploadDirectory, _rootDir, $"{this._zipFileName}.zip");

            if (File.Exists(targetZipPath))
            {
                File.Delete(targetZipPath);
            }

            ZipHelper.CompressDirectoryZip(tempDir, targetZipPath);
            Directory.Delete(tempDir, true);


            return  targetZipPath.Replace($"{_environment.WebRootPath}\\", "");
        }

        private void GengerationTranferTreeRecurrence(List<GengerationTranferTree> gengerationTranferTrees, string path)
        {
            foreach (var item in gengerationTranferTrees)
            {

                if (item.IsFolder == true)
                {
                    var dir = Path.Combine(path, item.FileName);
                    Directory.CreateDirectory(dir);
                    GengerationTranferTreeRecurrence(item.Children, dir);
                }
                else
                {
                    var fileUrl = Path.Combine(path, item.FileName);
                    if (item.NetworkAddress.IsNotNullOrNotWhiteSpace())
                    {
                        var url = Path.Combine(_environment.WebRootPath, item.NetworkAddress);
                        if (File.Exists(url))
                        {
                            var fs = new FileInfo(url);
                            fs.CopyTo(fileUrl);
                        }
                    }
                    else
                    {

                        //写入不带With Bom的文件格式
                        //using (StreamWriter writer = new StreamWriter(fileUrl, false, Encoding.UTF8))
                        //{
                        //    writer.Write(item.Content);
                        //}
                        Encoding encoding = new System.Text.UTF8Encoding(false);
                        FileStream fs = File.Create(fileUrl);
                        fs.Dispose();
                        File.WriteAllText(fileUrl, item.Content, encoding);
                    }


                }

            }

        }

        public IGengerationTranferTreeFileService InitSettings(string rootDir, string zipFileName)
        {
            _rootDir = rootDir;

            _zipFileName = zipFileName;
            return this;
        }
    }
}
