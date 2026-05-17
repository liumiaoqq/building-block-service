using Microsoft.AspNetCore.Hosting;
using System.ComponentModel;
using System.IO;
using System.Text;
using Utils.Zip;
using Web.Dto.Components;
using Web.Dto.TemporaryFiles;
using Web.Extensions;
using Web.Manager;
using Web.Service;
using YouJu.Infrastructure.Extensions;
using YouJu.Infrastructure.IO;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ComponentController : YouJuController<SystemComponent, ComponentDto, ComponentPagedInput>
    {
        public readonly ComponentManager _componentManager;
        private readonly ComponentService _componentService;
        private readonly TemporaryFileRecordManager _temporaryFileRecordManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ComponentController(IServiceProvider serviceProvider, ComponentManager componentManager, IWebHostEnvironment webHostEnvironment, TemporaryFileRecordManager temporaryFileRecordManager, ComponentService componentService) : base(serviceProvider)
        {
            _componentManager = componentManager;
            _webHostEnvironment = webHostEnvironment;
            _temporaryFileRecordManager = temporaryFileRecordManager;
            _componentService = componentService;
        }

        [HttpPost("SettingDetListAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async Task<List<SystemComponentSettingDetDto>> SettingDetListAsync(IdInput<Guid> input)
        {

            return await _componentManager.GetSettingDetListAsync(input.Id);

        }

        [HttpPost("AddOrEditSystemComponentSettingDetAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async Task AddOrEditSystemComponentSettingDetAsync(AddOrEditSystemComponentSettingDetInput input)
        {
          
         await _componentManager.AddOrEditSystemComponentSettingDetAsync(input.SystemComponentId, input.Dets);
          
        }



        [HttpPost("GetComponentTreeData")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async Task<List<ComponentTreeData>> GetComponentTreeData(ComponentPagedInput input)
        {
            if (input.ModuleId.HasValue)
            {
                return await _componentManager.GetComponentTreeData(input.ModuleId.Value, false);
            }
            else
            {

                return new List<ComponentTreeData>();
            }
        }


        [HttpPost("GetLanguageWay")]
        [CustomAuthorization(RoleType.系统管理员)]
        public PagedReuslt<SelectResult> GetLanguageWay()
        {
            var roles = new List<SelectResult>();

            roles = typeof(LanguageWay).GetEnumList().Select(x => new SelectResult() { Name = x.Key, Value = x.Value }).ToList();
            return new PagedReuslt<SelectResult>(roles, roles.Count);

        }
        [HttpPost("AddOrUpdateComponentAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async Task AddOrUpdateComponentAsync(ComponentDto input)
        {

            var entitys = await SqlSugarClient.Queryable<SystemComponent>().Where(x => x.ModuleId == input.ModuleId).ToListAsync();
            var entity = entitys.FirstOrDefault(x => x.Id == input.Id);
            if (entity == null)
            {
                if (entitys.Count(x => x.LanguageWay == input.LanguageWay) > 0)
                {
                    throw new YouJuException("一个模块只能添加一个类型的组件");
                }
                entity = await SqlSugarClient.Insertable(new SystemComponent()
                {
                    ModuleId = input.ModuleId,
                    LanguageWay = input.LanguageWay.Value,
                    Name = input.Name,

                }).ExecuteReturnEntityAsync();

            }
            else
            {
                entity.ModuleId = input.ModuleId;
                entity.LanguageWay = input.LanguageWay.Value;

                entity.Name = input.Name;
                await SqlSugarClient.Updateable(entity).ExecuteCommandAsync();

            }

        }

        [HttpPost("DeleteComponentsAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async Task DeleteComponentsAsync(DeleteComponentInput input)
        {

            await _componentManager.DeleteComponentsOrTemplete(input);

        }




        [HttpPost("AddComponentTempleteAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async Task<ComponentTreeData> AddComponentTempleteAsync(ComponentTempleteDto input)
        {

            var entity = await SqlSugarClient.Queryable<ComponentTemplete>().FirstAsync(x => x.Id == input.Id);
            if (entity is null)
            {
                input.Id = Guid.Empty;
                entity = input.Clone<ComponentTempleteDto, ComponentTemplete>();

                entity.FileType = entity.FileName.GetFileTypes();
                entity = await SqlSugarClient.Insertable(entity).ExecuteReturnEntityAsync();
            }
            else
            {

                entity = input.Clone<ComponentTempleteDto, ComponentTemplete>();
                entity.FileType = entity.FileName.GetFileTypes();
                await SqlSugarClient.Updateable(entity).ExecuteCommandAsync();
            }

            return new ComponentTreeData()
            {
                Id = entity.Id,
                Label = entity.FileName,
                IsTemplete = true,

                FileType = entity.FileType,
                Horizontal = entity.Horizontal,
                EnumHorizontal = entity.EnumHorizontal,
                IsTempleteGrammar = entity.IsTempleteGrammar,
                Decompressed = entity.Decompressed,
                NetworkAddress = entity.NetworkAddress,
                ComponentId = entity.ComponentId,
                IsFolder = entity.FileName.CheckIsFolder(),

            };

        }

        /// <summary>
        /// 批量上传模板文件
        /// </summary>
        [HttpPost("BatchUploadTempleteFileAsync")]
        [CustomAuthorization(RoleType.系统管理员)]
        [DisableRequestSizeLimit]
        public async Task<bool> BatchUploadTempleteFileAsync()
        {

            try
            {
                bool isUndecompression = Request.Form["undecompression"].ToString() == "1";

                bool isTemplte = Request.Form["isTemplete"].ToString() == "true";

                #region 创建文件夹
                var directory = $"{SysConst.TempleteFileRootDir}/temp/{DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss")}";

                var uploadDirectory = Path.Combine(_webHostEnvironment.WebRootPath, directory);



                DirectoryHelper.CreateIfNotExists(uploadDirectory);//按类型创建文件夹格式
                #endregion

                #region 批量上传

                foreach (var itemFile in Request.Form.Files)
                {
                    var fType = itemFile.GetFileType();
                    var fName = itemFile.GetFileName();
                    var fullFName = $"{fName}";
                    if (fType.IsNotNullOrNotWhiteSpace())
                    {
                        fullFName += $".{fType}";
                    }



                    var uploadUrl = $"{uploadDirectory}/{fullFName}";

                    using (var stream = new FileStream(uploadUrl, FileMode.Create))
                    {
                        await itemFile.CopyToAsync(stream);

                    }
                    //如果需要解压
                    if (isUndecompression)
                    {
                        if (new[] { "zip", "rar" }.Contains(fType))
                        {
                            //解压到当前目录
                            ZipHelper.DecompressZip(uploadUrl, uploadDirectory);
                            //删除压缩包
                            System.IO.File.Delete(uploadUrl);
                        }
                        else
                        {
                            throw new YouJuException("请上传类型为zip和rar的文件");
                        }
                    }


                }

                var id = Guid.Parse(Request.Form["id"].ToString());
                //如果是模板
                if (isTemplte)
                {
                    var rootTemplete = await SqlSugarClient.Queryable<ComponentTemplete>().FirstAsync(x => x.Id == id);

                    List<ComponentTemplete> components = _componentManager.RecurrenceFiles(rootTemplete, new DirectoryInfo(uploadDirectory), rootTemplete.ComponentId);
                    await SqlSugarClient.Insertable(components).ExecuteCommandAsync();

                }
                else
                {
                    //如果不是模板 是组件
                    List<ComponentTemplete> components = _componentManager.RecurrenceFiles(null, new DirectoryInfo(uploadDirectory), id);
                    await SqlSugarClient.Insertable(components).ExecuteCommandAsync();

                }


                DirectoryHelper.Delete(uploadDirectory, true);
                return true;



            }
            catch (Exception ex)
            {

                throw new YouJuException(ex.Message);
            }

            #endregion

        }


        #region 选择上传的文件夹
        [HttpPost("BatchUploadTempleteDirectoryAsync")]
        [DisableRequestSizeLimit]
        public async Task<Guid> BatchUploadTempleteDirectoryAsync()
        {

            var groupId = Guid.NewGuid();

            #region 完成文件的创建
            var directory = $"{SysConst.TemporaryTempletesDir}/{DateTime.Now.ToString("yyyy_MM_dd")}/{groupId.ToString()}";

            var uploadDirectory = Path.Combine(_webHostEnvironment.WebRootPath, directory);

            foreach (var itemFile in Request.Form.Files)
            {
                var fileNameSplits = itemFile.FileName.Split("/").ToList();
                var itemFileDirectory = itemFile.FileName.Substring(0, itemFile.FileName.LastIndexOf("/"));
                var fullFileName = fileNameSplits.LastOrDefault();
                var fileName = fullFileName.GetFileName();
                var fileType = fileNameSplits.LastOrDefault().GetFileType();

                DirectoryHelper.CreateIfNotExists(Path.Combine(uploadDirectory, itemFileDirectory));//按类型创建文件夹格式

                using (var stream = new FileStream(Path.Combine(uploadDirectory, itemFileDirectory, fullFileName), FileMode.Create))
                {
                    await itemFile.CopyToAsync(stream);
                }
            }
            #endregion

            #region 根据根目录获取到所有的文件和文件夹

            var temporaryFileRecord = _temporaryFileRecordManager.RecurrenceFiles(null, new DirectoryInfo(uploadDirectory), groupId, directory);
            await SqlSugarClient.Insertable(temporaryFileRecord).ExecuteCommandAsync();

            #endregion

            return groupId;
        }


        [HttpPost("GetCombineTemporaryFileTree")]
        public async Task<List<CombineTemporaryFileTree>> GetCombineTemporaryFileTree(GetCombineTemporaryFileTreeInput input)
        {
            return await _temporaryFileRecordManager.GetCombineTemporaryFileTree(input.GroupId, input.ComponentTempleteId, input.ComponentId);
        }

        [HttpPost("TemporaryFileSave")]
        public async Task TemporaryFileSave(TemporaryFileSaveInput input)
        {
            await _temporaryFileRecordManager.TemporaryFileSave(input);
        }


        #endregion










    }
}
