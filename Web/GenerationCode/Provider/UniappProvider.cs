using Web.GenerationCode.Dto;
using Scriban.Runtime;
using Scriban;
using Web.Dto.Gengerations;
using System.Reflection;
using System.Linq;

namespace Web.GenerationCode.Provider
{
    public class UniappProvider : IGengerationTranslateProvider
    {
        private PlanGenerationWriteInput _context;
        
        public List<GengerationTranferTree> GenerateResult(PlanGenerationWriteInput context, List<GengerationComponentTreeData> treeDatas)
        {
            _context = context;

            return GengerationTranferTreeRecurrence(treeDatas);
        }

        public List<GengerationTranferTree> GengerationTranferTreeRecurrence(List<GengerationComponentTreeData> treeDatas)
        {
            GengerationComponentTreeData eachObj = null;    
            List<GengerationTranferTree> result = new List<GengerationTranferTree>();
            try
            {
                foreach (var treeData in treeDatas)
                {
                    eachObj = treeData;
                    if (treeData.Horizontal)
                    {
                        foreach (var table in _context.TableEntitys.Where(x => x.IsExtra == false))
                        {
                            _context.TableEntity = table;
                            var tree = new GengerationTranferTree()
                            {

                                FileType = treeData.FileType,
                                IsTemplete = treeData.IsTemplete,
                                IsFolder = treeData.IsFolder,
                                NetworkAddress = treeData.NetworkAddress,
                            };
                            if (treeData.IsTempleteGrammar == true)
                            {
                                tree.Content = treeData.Content ?? "";
                                tree.FileName = treeData.Label ?? "";
                            }
                            else
                            {

                                tree.Content = treeData.Content.IsNotNullOrNotWhiteSpace() ? ScribanHelper.Render(treeData.Content, new { Model = _context }, MemberRenamer) : "";
                                tree.FileName = treeData.Label.IsNotNullOrNotWhiteSpace() ? ScribanHelper.Render(treeData.Label, new { Model = _context }, MemberRenamer) : "";
                            }
                            _context.TableEntity = new TableEntityWriteInput();
                            tree.Children.AddRange(GengerationTranferTreeRecurrence(treeData.Children));

                            result.Add(tree);
                        }

                    }
                    else
                    {
                        var tree = new GengerationTranferTree()
                        {
                            FileType = treeData.FileType,
                            IsTemplete = treeData.IsTemplete,
                            IsFolder = treeData.IsFolder,
                            NetworkAddress = treeData.NetworkAddress,
                        };
                        if (treeData.IsTempleteGrammar == true)
                        {
                            tree.Content = treeData.Content ?? "";
                            tree.FileName = treeData.Label ?? "";
                        }
                        else
                        {
                            tree.Content = treeData.Content.IsNotNullOrNotWhiteSpace() ? ScribanHelper.Render(treeData.Content, new { Model = _context }, MemberRenamer) : "";
                            tree.FileName = treeData.Label.IsNotNullOrNotWhiteSpace() ? ScribanHelper.Render(treeData.Label, new { Model = _context }, MemberRenamer) : "";
                        }
                        tree.Children.AddRange(GengerationTranferTreeRecurrence(treeData.Children));

                        result.Add(tree);
                    }


                }
                return result;
            }
            catch (Exception ex)
            {
                throw new YouJuException($"UniappProvider代理出现错误,具体文件名称:{eachObj.Label},错误信息：{ex.Message}");

            }
            
               

            



        }

        private string MemberRenamer(MemberInfo member)
        {
            return member.Name;
        }
    }
}
