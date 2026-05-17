using Web.Dto.Components;
using Web.Dto.Gengerations;
using Web.Dto.Modules;
using Web.Dto.Plans;

namespace Web.Manager
{
    public class SystemModuleManager
    {
        protected ISqlSugarClient _sqlSugarClient;

        public ComponentManager _componentManager;

        public SystemModuleManager(ISqlSugarClient sqlSugarClient, ComponentManager componentManager)
        {
            _sqlSugarClient = sqlSugarClient;
            _componentManager = componentManager;
        }


        /// <summary>
        /// 得到当前模块
        /// </summary>
        public async Task<SystemModuleDto> GetAsync(Guid id) {
           return await _sqlSugarClient.Queryable<SystemModule>().Where(x=>x.Id==id).Select<SystemModuleDto>().FirstAsync();
        }

        #region 选择关联模块
        /// <summary>
        /// 得到自己关联的模块
        /// </summary>
        public async Task<List<ModuleRelativeDtos>> GetPlanRelativeModules(ModuleRelativePagedInput input)
        {
            //得到我关联的所有模块
            var relativeModuleIds = await _sqlSugarClient.Queryable<PlanModuleRelative>().Where(x =>  x.PlanId == input.PlanId).Select(x => x.ModuleId).ToListAsync();

            //得到所有的模块
            var allModules = await _sqlSugarClient.Queryable<SystemModule>()
                .WhereIF(input.WarehouseId.HasValue,x=>x.WarehouseId==input.WarehouseId).Select<SystemModuleDto>().ToListAsync();


            //根据类型进行分组
            var relativeModules = allModules.GroupBy(x =>new { x.SystemModuleType ,x.SortLabel}).Select(h => new ModuleRelativeDtos()
            {
                SystemModuleType = h.Key.SystemModuleType,
                SortLabel=h.Key.SortLabel,
                SystemModuleWapperDtoList = h.ToList().Select(z => new SystemModuleWapperDto()
                {
                    IsCheck = relativeModuleIds.Contains(z.Id.Value),
                    SystemModuleDto = z
                }).ToList()

            }).ToList();
            foreach (var item in relativeModules)
            {
                //选中的或者属于该仓库的 都显示
                item.SystemModuleWapperDtoList= item.SystemModuleWapperDtoList.WhereIF(input.WarehouseId.HasValue, x => input.WarehouseId == x.SystemModuleDto.WarehouseId || x.IsCheck == true).ToList();

                item.CheckdModuleIds = item.SystemModuleWapperDtoList.Where(x => x.IsCheck).Select(h => h.SystemModuleDto.Id.Value).ToList();
            }
            
            return relativeModules;
        }

        #endregion



        public async Task<PagedReuslt<SystemModuleDto>> GetAllModule(ModulePagedInput input)
        {
            RefAsync<int> totalCount = 0;
            var relatives = new List<PlanModuleRelative>();
            if (input.PlanIds.HasItem())
            {
                relatives = await _sqlSugarClient.Queryable<PlanModuleRelative>().Where(x => input.PlanIds.Contains(x.PlanId)).ToListAsync();
                input.ModuleIds = relatives.Select(x => x.ModuleId).ToList();


            }

            var modules = await _sqlSugarClient.Queryable<SystemModule>()
                 .WhereIF(input.Label.IsNotNullOrNotWhiteSpace(), x => x.Label.Contains(input.Label))
                 .WhereIF(input.Name.IsNotNullOrNotWhiteSpace(), x => x.Name.Contains(input.Name))
                 .WhereIF(input.ModuleIds.HasItem(), x => input.ModuleIds.Contains(x.Id))
                .WhereIF(input.ModuleIds.HasItem(), x => input.ModuleIds.Contains(x.Id)).Select<SystemModuleDto>().OrderBy(x => x.Label).ToPageListAsync(input.Page, input.Size, totalCount);

            var moduleIds = modules.Select(x => x.Id.Value).ToArray();



            var components = await _componentManager.GetModuleComponentData(moduleIds);


           
            


            foreach (var module in modules)
            {

                module.Components = components.Where(x => x.ModuleId == module.Id).ToList();
                if (relatives.Any(x => x.ModuleId == module.Id))
                {
                    //冗余字段 
                    module.PlanId = relatives.FirstOrDefault(x => x.ModuleId == module.Id).PlanId;
                }

            }

            return new PagedReuslt<SystemModuleDto>(modules, totalCount);

        }



        #region 获取模块的显示树结构
        /// <summary>
        /// 根据模块获取组件
        /// </summary>
        /// <param name="moduleIds"></param>
        /// <returns></returns>
        public async Task<List<GengerationComponentTreeData>> GetGengerationComponentTreeData(Guid[] moduleIds)
        {

            var components = await _sqlSugarClient.Queryable<SystemComponent>()
                 .Where(x => moduleIds.Contains(x.ModuleId)).Select<ComponentDto>().ToListAsync();

            var componentIds = components.Select(x => x.Id.Value).ToList();

            var allTempletes = await _sqlSugarClient.Queryable<ComponentTemplete>().Where(x => componentIds.Contains(x.ComponentId)).Select<ComponentTempleteDto>().ToListAsync();

            var componentTrees = new List<GengerationComponentTreeData>();


            List<GengerationComponentTreeDataWarper> treePaths = new List<GengerationComponentTreeDataWarper>();

            foreach (var component in components)
            {
                var path = "";
                var treeData = new GengerationComponentTreeData()
                {
                    Id = component.Id,
                    IsTemplete = false,

                    LanguageWay = component.LanguageWay.Value,
                    Label = component.LanguageWay.ToDescription(),
                    IsFolder = true,
                    FileType = SysConst.FolderFormat,
                    CreationTime = component.CreationTime,
                };
                path += $"{treeData.Label}";
                treePaths.Add(new GengerationComponentTreeDataWarper() { Path = path, Node = treeData, Depth = 0 });

                var componentTempletes = allTempletes.Where(x => x.ComponentId == component.Id).ToList();
                foreach (var templete in componentTempletes.Where(x => x.ParentId == Guid.Empty || x.ParentId == null).ToList())
                {
                    var treeTemplete = new GengerationComponentTreeData()
                    {
                        Id = templete.Id,
                        IsTemplete = true,
                        Label = templete.FileName,
                        ComponentId = component.Id,
                        Horizontal = templete.Horizontal,
                        EnumHorizontal=templete.EnumHorizontal,
                        IsTempleteGrammar = templete.IsTempleteGrammar,
                        Decompressed = templete.Decompressed,
                        NetworkAddress = templete.NetworkAddress,
                        Content = templete.Content,
                        FileType = templete.FileType,
                        IsFolder = templete.FileType.IsFolder(),
                        CreationTime = templete.CreationTime,
                    };

                    treePaths.Add(new GengerationComponentTreeDataWarper() { Path = $"{path}->{treeTemplete.Label}", Node = treeTemplete, Depth = 1 });

                    treeTemplete.Children = GetGengerationComponentTreeRecurrence(treeTemplete, componentTempletes, $"{path}->{treeTemplete.Label}", 2, treePaths).OrderByDescending(x => x.IsFolder).ThenBy(x => x.Label).ToList();

                    treeData.Children.Add(treeTemplete);


                }

                componentTrees.Add(treeData);
            }

            treePaths = treePaths.GroupBy(x => x.Path).Select(x => new GengerationComponentTreeDataWarper() { Path = x.Key, Depth = x.FirstOrDefault().Depth, Node = x.OrderByDescending(h => h.Node.CreationTime).FirstOrDefault().Node }).ToList();
            foreach (var item in treePaths)
            {
                item.Node.Children.Clear();
            }

            var trees = GengerationComponentTree(treePaths);

            return trees.OrderByDescending(x => x.IsFolder).ThenBy(x => x.Label).ToList();
        }

        /// <summary>
        /// 递归构建模板树
        /// </summary>
        /// <param name="root"></param>
        /// <param name="templeteItems"></param>
        /// <returns></returns>
        private List<GengerationComponentTreeData> GetGengerationComponentTreeRecurrence(GengerationComponentTreeData root, List<ComponentTempleteDto> templeteItems, string path, int depth, List<GengerationComponentTreeDataWarper> treePaths)
        {
            if (root is null)
            {
                return new List<GengerationComponentTreeData>();
            }

            var childrenTreeDatas = new List<GengerationComponentTreeData>();

            foreach (var children in templeteItems.Where(x => x.ParentId == root.Id))
            {
                var treeData = new GengerationComponentTreeData()
                {
                    Id = children.Id,
                    Label = children.FileName,
                    IsTemplete = true,
                    FileType = children.FileType,
                    Horizontal = children.Horizontal,
                    EnumHorizontal=children.EnumHorizontal,
                    IsTempleteGrammar = children.IsTempleteGrammar,
                    Decompressed = children.Decompressed,
                    NetworkAddress = children.NetworkAddress,
                    ComponentId = children.ComponentId,
                    Content = children.Content,
                    IsFolder = children.FileType.IsFolder(),
                    CreationTime = children.CreationTime,
                };

                treePaths.Add(new GengerationComponentTreeDataWarper() { Path = $"{path}->{treeData.Label}", Node = treeData, Depth = depth });

                treeData.Children = GetGengerationComponentTreeRecurrence(treeData, templeteItems, $"{path}->{treeData.Label}", depth + 1, treePaths).OrderByDescending(x => x.IsFolder).ThenBy(x => x.Label).ToList();


                childrenTreeDatas.Add(treeData);
            }

            return childrenTreeDatas;
        }


        /// <summary>
        /// 取树的交集 层序遍历
        /// 1->2->4
        /// 1->2->3
        /// </summary>
        /// <returns></returns>
        private List<GengerationComponentTreeData> GengerationComponentTree(List<GengerationComponentTreeDataWarper> treePaths)
        {
            List<GengerationComponentTreeData> treeDatas = new List<GengerationComponentTreeData>();

            var depth = 0;
            foreach (var item in treePaths.Where(x => x.Depth == depth))
            {

                item.Node.Children.AddRange(GengerationComponentTreeRecurrence(item.Path, 1, treePaths));
                treeDatas.Add(item.Node);
            }


            return treeDatas;
        }
        private List<GengerationComponentTreeData> GengerationComponentTreeRecurrence(string path, int depth, List<GengerationComponentTreeDataWarper> treePaths)
        {
            List<GengerationComponentTreeData> treeDatas = new List<GengerationComponentTreeData>();

            var maxDepth = treePaths.Max(x => x.Depth);


            foreach (var item in treePaths.Where(x => x.Depth == depth && x.Path.StartsWith(path)))
            {

                item.Node.Children.AddRange(GengerationComponentTreeRecurrence(item.Path, depth + 1, treePaths));
                treeDatas.Add(item.Node);
            }
            return treeDatas;
        }




        #endregion



    }
}
