
using Microsoft.AspNetCore.Components;
using Org.BouncyCastle.Asn1.X509;
using Scriban;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Web.Dto.Components;
using YouJu.Infrastructure.Dto;
using YouJu.Infrastructure.IO;

namespace Web.Manager
{
    /// <summary>
    /// 系统组件(SystemComponent)
    /// </summary>
    public class ComponentManager
    {

        protected ISqlSugarClient _sqlSugarClient;

        private readonly IWebHostEnvironment _webHostEnvironment;



        public ComponentManager(ISqlSugarClient sqlSugarClient, IWebHostEnvironment webHostEnvironment)
        {
            _sqlSugarClient = sqlSugarClient;
            _webHostEnvironment = webHostEnvironment;
        }



        public async Task<List<SystemComponentSettingDetDto>> GetSettingDetListAsync(Guid systemComponentId)
        {
          return await  _sqlSugarClient.Queryable<SystemComponentSettingDet>().Where(x => x.SystemComponentId == systemComponentId).Select<SystemComponentSettingDetDto>().ToListAsync();
        
        }


        /// <summary>
        /// 添加或者修改系统组件的配置明细
        /// </summary>
        public async Task AddOrEditSystemComponentSettingDetAsync(Guid systemComponentId, List<SystemComponentSettingDetDto> inputs)
        {
            inputs = inputs.Where(x => x.Id.HasValue == true||(x.Key.IsNotNullOrNotWhiteSpace()&&x.Value.IsNotNullOrNotWhiteSpace())).ToList();
            if (inputs.Exists(x => x.Key.IsNullOrWhiteSpace() || x.Value.IsNullOrWhiteSpace())) {
                throw new YouJuException("键值对不能为空");
            }


            var inputIds = inputs.Where(x=>x.Id.HasValue).Select(x => x.Id.Value).Distinct().ToList();

            var rs = await _sqlSugarClient.Queryable<SystemComponentSettingDet>().Where(x => x.SystemComponentId == systemComponentId).ToListAsync();

            var entityIds = rs.Select(x => x.Id).ToList();

            //得到2个的交集 不需要动
            var intersectIds = entityIds.Intersect(inputIds);
    
            //数据库原始的排除交集就是需要删除的
            var deleteIds = entityIds.Where(x => !intersectIds.Contains(x)).ToList();
            //删除
            if (deleteIds.Count > 0)
            {

                await _sqlSugarClient.Deleteable<SystemComponentSettingDet>().Where(x => deleteIds.Contains(x.Id)).ExecuteCommandAsync();
            }
            //修改
            if (intersectIds.Count() > 0)
            {
                foreach (var id in intersectIds)
                {
                    var newEntity = inputs.FirstOrDefault(x => x.Id == id);
                    var oldEntity = rs.FirstOrDefault(x => x.Id == id);
                    oldEntity.Key = newEntity.Key;
                    oldEntity.Code = newEntity.Code;
                    oldEntity.Value = newEntity.Value;
                    await _sqlSugarClient.Updateable<SystemComponentSettingDet>(oldEntity).ExecuteCommandAsync();

                }

            }
            if (inputs.Count(x => x.Id.HasValue==false)> 0)
            {

                foreach (var newEntity in inputs.Where(x=>x.Id.HasValue==false).ToList())
                {
                    var det = new SystemComponentSettingDet();
                    det.SystemComponentId = systemComponentId;
                    det.Key = newEntity.Key;
                    det.Value = newEntity.Value;
                    det.Code = newEntity.Code;
                    await _sqlSugarClient.Insertable(det).ExecuteCommandAsync();

                }
            }


        }



        public async Task<ComponentDto> GetAsync(IdInput<Guid> input)
        {
            var component = await _sqlSugarClient.Queryable<SystemComponent>()
               .Where(x => x.Id == input.Id).Select<ComponentDto>().FirstAsync();
            return component;
        }


        /// <summary>
        /// 根据模块id获取
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public Task<List<ComponentDto>> GetModuleComponentDataById(Guid moduleId)
        {
            return GetModuleComponentData(new Guid[1] { moduleId });
        }

        /// <summary>
        /// 获取模块下的所有模板
        /// </summary>
        public async Task<List<ComponentDto>> GetModuleComponentData(Guid[] moduleIds)
        {
            var components = await _sqlSugarClient.Queryable<SystemComponent>()
                .Where(x => moduleIds.Contains(x.ModuleId)).Select<ComponentDto>().ToListAsync();



            var componentIds = components.Select(x => x.Id.Value).ToList();

            var allTempletes = await _sqlSugarClient.Queryable<ComponentTemplete>().Where(x => componentIds.Contains(x.Id)).Select<ComponentTempleteDto>().ToListAsync();


           var settingDets= await _sqlSugarClient.Queryable<SystemComponentSettingDet>().Where(x => componentIds.Contains(x.SystemComponentId)).Select<SystemComponentSettingDetDto>().ToListAsync();


            foreach (var component in components)
            {
                component.ComponentSettingDetDtos = settingDets.Where(x => x.SystemComponentId == component.Id).ToList();

                var componentTempletes = allTempletes.Where(x => x.Id == component.Id && x.ParentId == null).ToList();


                foreach (var item in componentTempletes)
                {
                  

                    item.ComponentTempletes = GetTempleteItemRecurrence(item, componentTempletes);
                }
                component.ComponentTempletes = componentTempletes;

            }
            return components;
        }


        /// <summary>
        /// 递归构建模板树
        /// </summary>
        /// <param name="root"></param>
        /// <param name="templeteItems"></param>
        /// <returns></returns>
        private List<ComponentTempleteDto> GetTempleteItemRecurrence(ComponentTempleteDto root, List<ComponentTempleteDto> templeteItems)
        {

            if (root is null)
            {
                return new List<ComponentTempleteDto>();
            }
            var childrens = templeteItems.Where(x => x.ParentId == root.Id).ToList();
            foreach (var children in childrens)
            {
                root.ComponentTempletes.AddRange(GetTempleteItemRecurrence(children, childrens));
            }
            return childrens;

        }



        #region 获取模块的显示树结构
        /// <summary>
        /// 根据模块获取组件
        /// </summary>
        /// <param name="moduleId">模块的id</param>
        /// <param name="isHasContent">是否需要包含内容</param>
        /// <returns></returns>
        public async Task<List<ComponentTreeData>> GetComponentTreeData(Guid moduleId, bool isHasContent)
        {

            var components = await _sqlSugarClient.Queryable<SystemComponent>()
                 .Where(x => moduleId == x.ModuleId).Select<ComponentDto>().ToListAsync();

            var componentIds = components.Select(x => x.Id.Value).ToList();

            var allTempletes = await _sqlSugarClient.Queryable<ComponentTemplete>().Where(x => componentIds.Contains(x.ComponentId)).Select<ComponentTempleteDto>().ToListAsync();

            var componentTrees = new List<ComponentTreeData>();

            foreach (var component in components)
            {

                var treeData = new ComponentTreeData()
                {
                    Id = component.Id,
                    IsTemplete = false,
                    Label = component.LanguageWay.ToDescription(),
                    LanguageWay = component.LanguageWay,
                    IsFolder = true,
                    FileType = SysConst.FolderFormat,

                };

                var componentTempletes = allTempletes.Where(x => x.ComponentId == component.Id).ToList();
                foreach (var templete in componentTempletes.Where(x => x.ParentId == null || x.ParentId == Guid.Empty).ToList())
                {
                    var treeTemplete = new ComponentTreeData()
                    {
                        Id = templete.Id,
                        IsTemplete = true,
                        Label = templete.FileName,
                        ComponentId = component.Id,
                        Horizontal = templete.Horizontal,
                        EnumHorizontal = templete.EnumHorizontal,
                        Decompressed = templete.Decompressed,
                        NetworkAddress = templete.NetworkAddress,
                        IsTempleteGrammar = templete.IsTempleteGrammar,
                        Content = isHasContent ? templete.Content : null,
                        FileType = templete.FileType,
                        IsFolder = templete.FileName.CheckIsFolder(),
                    };
                    treeTemplete.Children = GetComponentTreeRecurrence(treeTemplete, componentTempletes, isHasContent).OrderBy(x => x.FileType).ThenBy(x => x.Label).ToList();

                    treeData.Children.Add(treeTemplete);
                }

                componentTrees.Add(treeData);
            }
            return componentTrees.OrderByDescending(x => x.IsFolder).ThenBy(x => x.Label).ToList();
        }

        /// <summary>
        /// 递归构建模板树
        /// </summary>
        /// <param name="root"></param>
        /// <param name="templeteItems"></param>
        /// <returns></returns>
        private List<ComponentTreeData> GetComponentTreeRecurrence(ComponentTreeData root, List<ComponentTempleteDto> templeteItems, bool isHasContent)
        {
            if (root is null)
            {
                return new List<ComponentTreeData>();
            }

            var childrenTreeDatas = new List<ComponentTreeData>();

            foreach (var children in templeteItems.Where(x => x.ParentId == root.Id))
            {
                var treeData = new ComponentTreeData()
                {
                    Id = children.Id,
                    Label = children.FileName,
                    IsTemplete = true,
                    FileType = children.FileType,
                    Horizontal = children.Horizontal,
                    EnumHorizontal = children.EnumHorizontal,
                    Decompressed = children.Decompressed,
                    NetworkAddress = children.NetworkAddress,
                    IsTempleteGrammar = children.IsTempleteGrammar,
                    ParentId = root.Id,
                    ComponentId = children.ComponentId,
                    Content = isHasContent ? children.Content : null,
                    IsFolder = children.FileName.CheckIsFolder(),

                };

                treeData.Children = GetComponentTreeRecurrence(treeData, templeteItems, isHasContent).OrderByDescending(x => x.IsFolder).ThenBy(x => x.Label).ToList();

                childrenTreeDatas.Add(treeData);
            }

            return childrenTreeDatas.OrderByDescending(x => x.IsFolder).ThenBy(x => x.Label).ToList();
        }
        #endregion


        #region 删除操作
        /// <summary>
        /// 删除组件以及模板
        /// </summary>
        public async Task DeleteComponentsOrTemplete(DeleteComponentInput input)
        {
            List<Guid> componentIds = new List<Guid>();


            //删除组件
            if (input.IsTemplete == false)
            {


                await _sqlSugarClient.Updateable<SystemComponent>().Where(x => x.Id == input.ComponentId).SetColumns(x => x.IsDeleted == true).ExecuteCommandAsync();

                await _sqlSugarClient.Updateable<ComponentTemplete>().Where(x => input.ComponentId == x.ComponentId).SetColumns(x => x.IsDeleted == true).ExecuteCommandAsync();
            }
            else
            {
                var componentTempletes = await _sqlSugarClient.Queryable<ComponentTemplete>().Where(x => x.ComponentId == input.ComponentId).ToListAsync();

                var childrenTemplteIds = GetAllComponentTempleteChildrenRecurrence(input.TempleteId.Value, componentTempletes);
                childrenTemplteIds.Add(input.TempleteId.Value);
                await _sqlSugarClient.Updateable<ComponentTemplete>().Where(x => childrenTemplteIds.Contains(x.Id)).SetColumns(x => x.IsDeleted == true).ExecuteCommandAsync();
            }

        }
        /// <summary>
        /// 获取所有子节点的数据
        /// </summary>
        public List<Guid> GetAllComponentTempleteChildrenRecurrence(Guid rootId, List<ComponentTemplete> componentTempletes)
        {
            List<Guid> templeteIds = new List<Guid>();

            foreach (var item in componentTempletes.Where(x => x.ParentId == rootId))
            {
                templeteIds.Add(item.Id);
                templeteIds.AddRange(GetAllComponentTempleteChildrenRecurrence(item.Id, componentTempletes));
            }

            return templeteIds;


        }
        #endregion


        #region 导入模板操作 
        /// <summary>
        /// 递归文件然后返回一个列表集合
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>

        public List<ComponentTemplete> RecurrenceFiles(ComponentTemplete root, DirectoryInfo rootDir, Guid componentId)
        {
            var templetes = new List<ComponentTemplete>();

            foreach (var folder in rootDir.GetDirectories())
            {
                if (SysConst.FilterFolderType.Count(x => folder.Name.Contains(x, StringComparison.OrdinalIgnoreCase)) > 0)
                {
                    continue;
                }
                var dirTemplete = new ComponentTemplete()
                {
                    Id = Guid.NewGuid(),
                    ComponentId = componentId,
                    ParentId = root != null ? root.Id : null,
                    FileName = folder.Name,
                    FileType = SysConst.FolderFormat,

                };
                templetes.Add(dirTemplete);

                templetes.AddRange(RecurrenceFiles(dirTemplete, folder, componentId));

            }
            //遍历所有文件
            foreach (var file in rootDir.GetFiles())
            {
                if (SysConst.FilterFileType.Count(x => file.Name.Contains(x, StringComparison.OrdinalIgnoreCase)) > 0)
                {
                    continue;
                }
                //如果是一些不是代码的文件,直接移动
                if (SysConst.NotCodeFileType.Contains(file.Extension) || file.Extension.IsNullOrWhiteSpace())
                {

                    var templete = new ComponentTemplete()
                    {
                        Id = Guid.NewGuid(),
                        ComponentId = componentId,
                        ParentId = root != null ? root.Id : null,
                        FileName = file.Name,

                        NetworkAddress = "",
                        FileType = file.Extension,

                    };
                    var dir = Path.Combine(_webHostEnvironment.WebRootPath, SysConst.TempleteFileRootDir, templete.Id.ToString());

                    DirectoryHelper.CreateIfNotExists(dir);//按类型创建文件夹格式

                    templete.NetworkAddress = Path.Combine(SysConst.TempleteFileRootDir, templete.Id.ToString(), templete.FileName);

                    file.MoveTo(Path.Combine(dir, templete.FileName));

                    templetes.Add(templete);

                }
                else
                {
                    using (var fs = file.OpenText())
                    {

                        templetes.Add(new ComponentTemplete()
                        {
                            Id = Guid.NewGuid(),
                            ComponentId = componentId,
                            ParentId = root != null ? root.Id : null,
                            FileName = file.Name,
                            FileType = file.Extension,
                            IsTempleteGrammar = true,//忽略语法
                            Content = fs.ReadToEnd(),

                        });
                    }
                }

            }
            return templetes;


        }




        #endregion


        #region 复制一个模板到某个节点

        public async Task CopyModuleById(Guid copyModuleId, Guid moduleId)
        {


            List<ComponentTreeData> componentTreeDatas = await GetComponentTreeData(moduleId, true);

            var componentTempletes = new List<ComponentTemplete>();


            var systemComponents = new List<SystemComponent>();

            //遍历所有模板
            foreach (var componentTreeData in componentTreeDatas)
            {
                //第一层是
                var component = new SystemComponent()
                {
                    LanguageWay = (LanguageWay)Enum.Parse(typeof(LanguageWay), componentTreeData.Label),
                    Id = Guid.NewGuid(),
                    Name = componentTreeData.Label,

                    ModuleId = copyModuleId,
                };
                systemComponents.Add(component);
                componentTempletes.AddRange(ComponentTempleteTreeRecurrence(componentTreeData.Children, component.Id, null));

            }


            await _sqlSugarClient.Insertable(systemComponents).ExecuteCommandAsync();
            await _sqlSugarClient.Insertable(componentTempletes).ExecuteCommandAsync();



        }

        /// <summary>
        /// 递归构建
        /// </summary>
        public List<ComponentTemplete> ComponentTempleteTreeRecurrence(List<ComponentTreeData> componentTreeDatas, Guid componentId, Guid? parentId)
        {

            var componentTempleteList = new List<ComponentTemplete>();
            foreach (var componentTemplete in componentTreeDatas)
            {
                var copyComponentTemplete = new ComponentTemplete()
                {
                    Content = componentTemplete.Content,
                    Id = Guid.NewGuid(),
                    ParentId = parentId,
                    ComponentId = componentId,
                    Decompressed = componentTemplete.Decompressed,
                    EnumHorizontal = componentTemplete.EnumHorizontal,
                    FileName = componentTemplete.Label,
                    NetworkAddress = componentTemplete.NetworkAddress,
                    IsTempleteGrammar = componentTemplete.IsTempleteGrammar,

                    FileType = componentTemplete.FileType,
                    Horizontal = componentTemplete.Horizontal,

                };
                if (componentTemplete.NetworkAddress.IsNotNullOrNotWhiteSpace())
                {

                    var dir = Path.Combine(_webHostEnvironment.WebRootPath, SysConst.TempleteFileRootDir, copyComponentTemplete.Id.ToString());

                    DirectoryHelper.CreateIfNotExists(dir);//按类型创建文件夹格式

                    copyComponentTemplete.NetworkAddress = Path.Combine(SysConst.TempleteFileRootDir, copyComponentTemplete.Id.ToString(), copyComponentTemplete.FileName);

                    var source_abs_path = Path.Combine(_webHostEnvironment.WebRootPath, SysConst.TempleteFileRootDir, componentTemplete.Id.ToString(), componentTemplete.Label);
                    var target_abs_path = Path.Combine(_webHostEnvironment.WebRootPath, SysConst.TempleteFileRootDir, copyComponentTemplete.Id.ToString(), copyComponentTemplete.FileName);
                    //copy一份过去
                    new FileInfo(source_abs_path).CopyTo(target_abs_path);
                }



                componentTempleteList.Add(copyComponentTemplete);
                componentTempleteList.AddRange(ComponentTempleteTreeRecurrence(componentTemplete.Children, componentId, copyComponentTemplete.Id));
            }




            return componentTempleteList;
        }

        #endregion


    }
}

