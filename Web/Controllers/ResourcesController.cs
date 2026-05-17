
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NPOI.HPSF;
using System.Collections.Generic;
using System.IO;
using YouJu.Infrastructure.Expection;
using YouJu.Infrastructure.IO;
using YouJu.Infrastructure.JWT;
using YouJu.Infrastructure.Oss.Qiniu;


namespace Web.Controllers
{
    /// <summary>
    /// 上传图片控制器
    /// </summary>
    [ApiController]

    [Route("[controller]")]
    public class ResourcesController : ControllerBase
    {

        private readonly ILogger<ResourcesController> _logger;

        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly IQiuNiuOssHelper _qiuNiuOssHelper;

        public ResourcesController(IWebHostEnvironment webHostEnvironment, ILogger<ResourcesController> logger, IQiuNiuOssHelper qiuNiuOssHelper)
        {
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _qiuNiuOssHelper = qiuNiuOssHelper;
        }

        [NonAction]
        private void CheckIsImageType()
        {
            var files = Request.Form.Files;
            const string fileFilt = ".gif|.jpg|.jpeg|.png";
            foreach (var itemFile in files)
            {
                if (!fileFilt.Contains(itemFile.GetFileType()))
                {
                    throw new YouJuException("只支持.gif|.jpg|.jpeg|.png几个格式");
                }
            }
        }

        /// <summary>
        /// 批量上传模板文件
        /// </summary>
        [HttpPost("BatchUploadAsync")]
        [DisableRequestSizeLimit]
        public async Task<List<ResoureceFileResult>> BatchUploadAsync()
        {

            List<ResoureceFileResult> result = new List<ResoureceFileResult>();
            try
            {
                var files = Request.Form.Files;


                #region 创建文件夹
                // var directory = $"Files/{DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss")}";

                // var uploadDirectory = Path.Combine(_webHostEnvironment.WebRootPath, directory);

                // DirectoryHelper.CreateIfNotExists(uploadDirectory);//按类型创建文件夹格式

                #endregion

                foreach (var itemFile in Request.Form.Files)
                {
                    //得到文件的类型 并且判断是不是图片 比如png jpg jpeg gif
                    var fType = itemFile.GetFileType();
                    if (fType == "png" || fType == "jpg" || fType == "jpeg" || fType == "gif")
                    {
                        //检查图片不能大于3mb 否则直接报错
                        if (itemFile.Length > 3 * 1024 * 1024)
                        {
                            throw new YouJuException("图片不能大于3mb");
                        }
                    }

                    //如果是其他文件也不能大于10mb
                    if (itemFile.Length > 10 * 1024 * 1024)
                    {
                        throw new YouJuException("其他文件不能大于10mb");
                    }
                }



                #region 批量上传
                foreach (var itemFile in Request.Form.Files)
                {
                    //检查类型是图片并且不能大于3mb 否则直接报错


                    var fType = itemFile.GetFileType();
                    var fName = itemFile.GetFileName();
                    // var fullFName = $"{fName}.{fType}";
                    // var uploadUrl = $"{uploadDirectory}/{fullFName}";

                    // using (var stream = new FileStream(uploadUrl, FileMode.Create))
                    // {
                    //     await itemFile.CopyToAsync(stream);

                    // }
                    //得到流
                    using (var stream = itemFile.OpenReadStream())
                    {
                        var saveKey = $"building_block_service/Files/{Guid.NewGuid().ToString("N")}.{fType}";
                        var url = await _qiuNiuOssHelper.UploadFileStream(saveKey, stream);
                        //这个saveKey你来取一个唯一的
                        if (string.IsNullOrEmpty(url))
                        {
                            throw new YouJuException("上传失败");
                        }

                        result.Add(new ResoureceFileResult() { OldFileName = itemFile.FileName, Type = fType, FileName = fName, Url = url });
                    }


                }

            }
            catch (Exception ex)
            {
                _logger.LogInformation("BatchUploadAsync失败了", ex.Message);
                throw new YouJuException(ex.Message);
            }

            #endregion
            return result;
        }




    }
}
