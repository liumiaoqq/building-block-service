using MathNet.Numerics.LinearAlgebra.Factorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using NPOI.Util;
using System.ComponentModel;
using Web.Dto.Gengerations;
using Web.Dto.TemporaryFiles;
using Web.Tables;
using YouJu.Infrastructure.IO;

namespace Web.Manager
{
    public class TemporaryFileRecordManager
    {
        protected ISqlSugarClient _sqlSugarClient;
        private readonly IWebHostEnvironment _environment;

        public TemporaryFileRecordManager(ISqlSugarClient sqlSugarClient, IWebHostEnvironment environment)
        {
            _sqlSugarClient = sqlSugarClient;
            _environment = environment;
        }

        /// <summary>
        /// 递归文件然后返回一个列表集合
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>

        public List<TemporaryFileRecord> RecurrenceFiles(TemporaryFileRecord root, DirectoryInfo rootDir, Guid groupId, string url)
        {

            var components = new List<TemporaryFileRecord>();

            foreach (var folder in rootDir.GetDirectories())
            {
                if (SysConst.FilterFolderType.Count(x => folder.Name.Contains(x, StringComparison.OrdinalIgnoreCase)) > 0)
                {
                    continue;
                }
                var dirTemplete = new TemporaryFileRecord()
                {
                    Id = Guid.NewGuid(),
                    GroupId = groupId,
                    ParentId = root != null ? root.Id : null,
                    NewFileName = folder.Name,
                    NewFileSuffix = SysConst.FolderFormat,
                    NewPath = $"{url}/{folder.Name}",
                };
                components.Add(dirTemplete);

                components.AddRange(RecurrenceFiles(dirTemplete, folder, groupId, dirTemplete.NewPath));

            }
            //遍历所有文件
            foreach (var file in rootDir.GetFiles())
            {
                if (SysConst.FilterFileType.Count(x => file.Name.Contains(x, StringComparison.OrdinalIgnoreCase)) > 0)
                {
                    continue;
                }
                //如果是一些不是代码的文件,直接移动



                var templete = new TemporaryFileRecord()
                {
                    Id = Guid.NewGuid(),
                    GroupId = groupId,
                    ParentId = root != null ? root.Id : null,
                    NewFileName = file.Name,
                    NewFileSuffix = file.Extension,
                    HashCode = FileHelper.GetFileHash(file.FullName),
                    IsSpecialFile = SysConst.NotCodeFileType.Contains(file.Extension),
                    NewPath = $"{url}/{file.Name}",

                };
                //如果是代码类型的文件
                if (!templete.IsSpecialFile)
                {
                    using (var fs = file.OpenText())
                    {
                        templete.Content = fs.ReadToEnd();
                    }
                }
                components.Add(templete);


            }
            return components;


        }



        /// <summary>
        /// 得到待合并的树
        /// </summary>
        public async Task<List<CombineTemporaryFileTree>> GetCombineTemporaryFileTree(Guid groupId, Guid? componentTempleteId, Guid componentId)
        {
            List<TemporaryFileRecordNode> temporaryFileRecordNodes = await GetTemporaryFileRecordNode(groupId);
            List<ComponentTempleteNode> componentTempleteNodes = await GetComponentTempleteNode(componentId, componentTempleteId);
            //得到2个树的层级
            var combineTemporaryFileFlats = RecurrenceTemporaryFileRecordNodeList(temporaryFileRecordNodes);

            var componentTempleteFlats = RecurrenceComponentTempleteNodeList(componentTempleteNodes);

            //合并所有的路径
            var fullUrls = componentTempleteFlats.Select(x => x.FullUrl).Union(combineTemporaryFileFlats.Select(x => x.FullUrl)).Distinct().ToList();

            List<CombineTemporaryFileTree> combineTree = new List<CombineTemporaryFileTree>();
            //完成二者的合并
            foreach (var fullUrl in fullUrls)
            {
                var combineTemporaryFileNode = combineTemporaryFileFlats.FirstOrDefault(x => x.FullUrl == fullUrl);
                if (combineTemporaryFileNode != null && combineTemporaryFileNode.TemporaryFileRecordNode != null)
                {
                    combineTemporaryFileNode.TemporaryFileRecordNode.Children.Clear();
                }

              

                var componentTempleteNode = componentTempleteFlats.FirstOrDefault(x => x.FullUrl == fullUrl);
                if (componentTempleteNode != null && componentTempleteNode.ComponentTempleteNode != null)
                {
                    componentTempleteNode.ComponentTempleteNode.Children.Clear();
                }



                combineTree.Add(new CombineTemporaryFileTree()
                {
                    Depth = combineTemporaryFileNode != null ? combineTemporaryFileNode.Depth : componentTempleteNode.Depth,
                    FullUrl = fullUrl,
                    IsFolder = combineTemporaryFileNode != null ? combineTemporaryFileNode.IsFolder : componentTempleteNode.IsFolder,
                    ParentFullUrl = combineTemporaryFileNode != null ? combineTemporaryFileNode.ParentFullUrl : componentTempleteNode.ParentFullUrl,
                    TemporaryFileRecordNode = combineTemporaryFileNode?.TemporaryFileRecordNode,
                    ComponentTempleteNode = componentTempleteNode?.ComponentTempleteNode,
                    IsAdd = IsAdd(combineTemporaryFileNode?.TemporaryFileRecordNode, componentTempleteNode?.ComponentTempleteNode),
                    IsEdit = IsEdit(combineTemporaryFileNode?.TemporaryFileRecordNode, componentTempleteNode?.ComponentTempleteNode),

                });
            }
            return GengerationCombineTemporaryFileTreeRecurrence(null, 0, combineTree);
        }


        /// <summary>
        /// 保存临时文件到正式的模板里面(不要导入模板语法内容)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task TemporaryFileSave(TemporaryFileSaveInput input)
        {
            List<ComponentTempleteNode> componentTempleteNodes = await GetComponentTempleteNode(input.ComponentId, input.ComponentTempleteId);
            var componentTempleteFlats = RecurrenceComponentTempleteNodeList(componentTempleteNodes);
            //得到模板树的路径
            var templetes = new List<ComponentTemplete>();
            var dic = new Dictionary<string, Guid?>();

            foreach (var node in input.CheckedCombineTemporaryFileTrees)
            {
                var urlSplits = node.FullUrl.Split("/").ToList();
                //如果是新增
                if (node.IsAdd.HasValue && node.IsAdd == true)
                {

                    #region 补齐路径的数据
                    //无论如何补齐路径
                    var directoryUrl = urlSplits;
                    //如果最后一个不是文件夹 移除列表
                    if (node.IsFolder == false)
                    {
                        directoryUrl = urlSplits.Take(urlSplits.Count - 1).ToList();
                    }


                    //如果是选择到了某个模板下的上传文件夹 需要先赋值 目的是以他为根节点
                    Guid? rootcomponentTempleteId = input.ComponentTempleteId;

                    Guid? parentcomponentTempleteId = componentTempleteFlats.FirstOrDefault(x => x.FullUrl == directoryUrl.JoinAsString("/"))?.ComponentTempleteNode?.Id;
                    

                    //如果不存在该目录,需要先补齐
                    if (parentcomponentTempleteId.HasValue==false)
                    {
                    
                        //循环
                        var index = 0;
                        while (index != directoryUrl.Count)
                        {
                            index++;
                            var currentUrl = directoryUrl.Take(index).JoinAsString("/");
                            var componentTempleteFlat = componentTempleteFlats.FirstOrDefault(x => x.FullUrl == currentUrl);
                            if (componentTempleteFlat != null)
                            {
                                parentcomponentTempleteId = componentTempleteFlat.ComponentTempleteNode.Id;
                            }
                            if (dic.ContainsKey(currentUrl))
                            {
                                parentcomponentTempleteId = dic[currentUrl];
                            }
                            else
                            {
                                var dirTemplete = new ComponentTemplete()
                                {
                                    Id = Guid.NewGuid(),
                                    ComponentId = input.ComponentId,
                                    ParentId = parentcomponentTempleteId?? rootcomponentTempleteId,//如果是没有父亲id则第一次要直接赋值根的
                                    FileName = directoryUrl[index - 1],
                                    FileType = SysConst.FolderFormat,
                                };
                                parentcomponentTempleteId = dirTemplete.Id;
                                dic.Add(currentUrl, dirTemplete.Id);
                                templetes.Add(dirTemplete);

                            }
                        }
                    }

                    #endregion

                    //如果最后一个是文件
                    if (node.IsFolder == false)
                    {
                        var templete = new ComponentTemplete()
                        {
                            Id = Guid.NewGuid(),
                            ComponentId = input.ComponentId,
                            ParentId = parentcomponentTempleteId,
                            FileName = node.TemporaryFileRecordNode.NewFileName,
                            NetworkAddress = "",
                            FileType = node.TemporaryFileRecordNode.NewFileSuffix,
                            IsTempleteGrammar = true,
                            Content = node.TemporaryFileRecordNode.Content,

                        };

                        //如果是特殊文件
                        if (node.TemporaryFileRecordNode.IsSpecialFile == true)
                        {
                            var dir = Path.Combine(_environment.WebRootPath, SysConst.TempleteFileRootDir, templete.Id.ToString());

                            DirectoryHelper.CreateIfNotExists(dir);//按类型创建文件夹格式

                            var fileInfo = new FileInfo(Path.Combine(_environment.WebRootPath, node.TemporaryFileRecordNode.NewPath));

                            templete.NetworkAddress = Path.Combine(SysConst.TempleteFileRootDir, templete.Id.ToString(), templete.FileName);

                            fileInfo.CopyTo(Path.Combine(dir, templete.FileName));
                        }
                        templetes.Add(templete);
                    }
                }

                //如果是编辑
                if (node.IsEdit.HasValue && node.IsEdit == true)
                {

                    var templete = await _sqlSugarClient.Queryable<ComponentTemplete>().Where(x => x.Id == node.ComponentTempleteNode.Id).FirstAsync();
                    if (node.TemporaryFileRecordNode.IsSpecialFile == true)
                    {
                        var dir = Path.Combine(_environment.WebRootPath, SysConst.TempleteFileRootDir, templete.Id.ToString());

                        DirectoryHelper.CreateIfNotExists(dir);//按类型创建文件夹格式

                        var fileInfo = new FileInfo(Path.Combine(_environment.WebRootPath, node.TemporaryFileRecordNode.NewPath));

                        templete.NetworkAddress = Path.Combine(SysConst.TempleteFileRootDir, templete.Id.ToString(), templete.FileName);

                        fileInfo.CopyTo(Path.Combine(dir, templete.FileName));


                    }
                    else
                    {
                        templete.Content = node.TemporaryFileRecordNode.Content;
                    }
                    await _sqlSugarClient.Updateable(templete).ExecuteCommandAsync();
                }




            }
            if (templetes.Count > 0)
            {
                await _sqlSugarClient.Insertable(templetes).ExecuteCommandAsync();
            }
        }






        private List<CombineTemporaryFileTree> GengerationCombineTemporaryFileTreeRecurrence(string path, int depth, List<CombineTemporaryFileTree> treePaths)
        {
            List<CombineTemporaryFileTree> treeDatas = new List<CombineTemporaryFileTree>();

            var maxDepth = treePaths.Max(x => x.Depth);

  

            foreach (var item in treePaths.Where(x => x.Depth == depth).WhereIF(path != null, x => x.FullUrl.StartsWith(path+"/")).OrderBy(x=>x.FullUrl))
            {

                 item.Chidren.AddRange(GengerationCombineTemporaryFileTreeRecurrence(item.FullUrl, depth + 1, treePaths));
                treeDatas.Add(item);
            }
            return treeDatas;
        }


        #region 得到临时记录的节点树
        private async Task<List<TemporaryFileRecordNode>> GetTemporaryFileRecordNode(Guid groupId)
        {
            List<Guid> builderIds = new List<Guid>();
            var components = await _sqlSugarClient.Queryable<TemporaryFileRecord>().Where(x => x.GroupId == groupId).ToListAsync();

            var componentNodeList = new List<TemporaryFileRecordNode>();
            foreach (var item in components.Where(x => x.ParentId.HasValue == false))
            {
                builderIds.Add(item.Id);

                componentNodeList.Add(new TemporaryFileRecordNode()
                {
                    FullUrl = item.NewFileName,
                    ParentFullUrl = string.Empty,
                    Depth = 0,
                    IsFolder = item.NewFileSuffix.CheckIsFolder(),
                    HashCode = item.HashCode,
                    ParentId = Guid.Empty,
                    Children = RecurrenceTemporaryFileRecordNode(item.Id, item.NewFileName, components.Where(x => !builderIds.Contains(x.Id)).ToList(), builderIds, 0),
                    Content = item.Content,
                    NewPath = item.NewPath,
                    NewFileName = item.NewFileName,
                    IsSpecialFile = item.IsSpecialFile,
                    Id = item.Id,
                    NewFileSuffix = item.NewFileSuffix,
                });

            }
            return componentNodeList;
        }


        private List<TemporaryFileRecordNode> RecurrenceTemporaryFileRecordNode(Guid parentId, string fullUrl, List<TemporaryFileRecord> components, List<Guid> builderIds, int depth = 0)
        {
            var componentNodeList = new List<TemporaryFileRecordNode>();
            foreach (var item in components.Where(x => x.ParentId == parentId))
            {
                builderIds.Add(item.Id);
                componentNodeList.Add(new TemporaryFileRecordNode()
                {
                    FullUrl = fullUrl + "/" + item.NewFileName,
                    ParentFullUrl = string.Empty,
                    Depth = depth + 1,
                    IsFolder = item.NewFileSuffix.CheckIsFolder(),
                    HashCode = item.HashCode,
                    ParentId = parentId,
                    Children = RecurrenceTemporaryFileRecordNode(item.Id, fullUrl + "/" + item.NewFileName, components.Where(x => !builderIds.Contains(x.Id)).ToList(), builderIds, depth + 1),
                    Content = item.Content,
                    NewPath = item.NewPath,
                    NewFileName = item.NewFileName,
                    IsSpecialFile = item.IsSpecialFile,
                    Id = item.Id,
                    NewFileSuffix = item.NewFileSuffix,
                });

            }
            return componentNodeList;
        }

        private List<CombineTemporaryFileFlat> RecurrenceTemporaryFileRecordNodeList(List<TemporaryFileRecordNode> chidren)
        {
            List<CombineTemporaryFileFlat> rs = new List<CombineTemporaryFileFlat>();
            foreach (var item in chidren)
            {
                rs.Add(new CombineTemporaryFileFlat()
                {
                    Depth = item.Depth,
                    FullUrl = item.FullUrl,
                    ParentFullUrl = item.ParentFullUrl,
                    TemporaryFileRecordNode = item,
                    IsFolder = item.IsFolder,
                });
                rs.AddRange(RecurrenceTemporaryFileRecordNodeList(item.Children));
            }
            return rs;
        }


        #endregion

        #region 得到模板树的节点
        private async Task<List<ComponentTempleteNode>> GetComponentTempleteNode(Guid componentId, Guid? parentId)
        {
            List<Guid> builderIds = new List<Guid>();
            var components = await _sqlSugarClient.Queryable<ComponentTemplete>().Where(x => x.ComponentId == componentId).ToListAsync();

            var componentNodeList = new List<ComponentTempleteNode>();
            foreach (var item in components.Where(x => x.ParentId == parentId))
            {
                builderIds.Add(item.Id);

                componentNodeList.Add(new ComponentTempleteNode()
                {
                    FullUrl = item.FileName,
                    ParentFullUrl = string.Empty,
                    Depth = 0,
                    IsFolder = item.FileType.CheckIsFolder(),
                    Id = item.Id,
                    Label = item.FileName,
                    IsTemplete = true,
                    HashCode = GetHash256(item.NetworkAddress),
                    FileType = item.FileType,
                    Horizontal = item.Horizontal,
                    EnumHorizontal=item.EnumHorizontal,
                    IsTempleteGrammar = item.IsTempleteGrammar,
                    Decompressed = item.Decompressed,
                    NetworkAddress = item.NetworkAddress,
                    ComponentId = item.ComponentId,
                    ParentId = null,
                    Content = item.Content,
                    Children = RecurrenceComponentNode(item.Id, item.FileName, components.Where(x => !builderIds.Contains(x.Id)).ToList(), builderIds, 0),
                });

            }
            return componentNodeList;
        }

        private List<ComponentTempleteNode> RecurrenceComponentNode(Guid parentId, string fullUrl, List<ComponentTemplete> components, List<Guid> builderIds, int depth = 0)
        {
            var componentNodeList = new List<ComponentTempleteNode>();
            foreach (var item in components.Where(x => x.ParentId == parentId))
            {

                builderIds.Add(item.Id);
                componentNodeList.Add(new ComponentTempleteNode()
                {
                    FullUrl = fullUrl + "/" + item.FileName,
                    ParentFullUrl = string.Empty,
                    Depth = depth + 1,
                    IsFolder = item.FileType.CheckIsFolder(),
                    Id = item.Id,
                    Label = item.FileName,
                    IsTemplete = true,
                    ParentId = parentId,
                    HashCode = GetHash256(item.NetworkAddress),
                    FileType = item.FileType,
                    Horizontal = item.Horizontal,
                    EnumHorizontal=item.EnumHorizontal,
                    IsTempleteGrammar = item.IsTempleteGrammar,
                    Decompressed = item.Decompressed,
                    NetworkAddress = item.NetworkAddress,
                    ComponentId = item.ComponentId,
                    Content = item.Content,
                    Children = RecurrenceComponentNode(item.Id, fullUrl + "/" + item.FileName, components.Where(x => !builderIds.Contains(x.Id)).ToList(), builderIds, depth + 1),
                });
            }
            return componentNodeList;
        }

        private List<CombineTemporaryFileFlat> RecurrenceComponentTempleteNodeList(List<ComponentTempleteNode> chidren)
        {
            List<CombineTemporaryFileFlat> rs = new List<CombineTemporaryFileFlat>();
            foreach (var item in chidren)
            {
                rs.Add(new CombineTemporaryFileFlat()
                {
                    Depth = item.Depth,
                    FullUrl = item.FullUrl,
                    ParentFullUrl = item.ParentFullUrl,
                    ComponentTempleteNode = item,
                    IsFolder = item.IsFolder,
                });
                rs.AddRange(RecurrenceComponentTempleteNodeList(item.Children));
            }
            return rs;
        }

        #endregion

        private string GetHash256(string url)
        {
            try
            {
                if (url == null)
                {
                    return null;
                }
                return FileHelper.GetFileHash(Path.Combine(_environment.WebRootPath, url));
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private bool? IsAdd(TemporaryFileRecordNode nodeA, ComponentTempleteNode nodeB)
        {
            if (nodeB == null)
            {
                return true;
            }
            return null;
        }
        private bool? IsEdit(TemporaryFileRecordNode nodeA, ComponentTempleteNode nodeB)
        {
            if (nodeA != null && nodeB != null)
            {
                if (nodeA.IsFolder)
                {
                    return null;
                }
                if (nodeA.IsSpecialFile && nodeB.NetworkAddress.IsNotNullOrNotWhiteSpace())
                {
                    return nodeA.HashCode != nodeB.HashCode;
                }
                if (nodeB.IsTemplete)
                {
                    return nodeA.Content != nodeB.Content;
                }
            }
            return null;
        }
    }
}
