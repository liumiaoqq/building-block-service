
using System.Collections.Generic;

using Web.GenerationCode.Dto;

using YouJu.Infrastructure.Dto;
using Scriban.Runtime;
using Scriban;
using Web.Dto.Gengerations;
using System.Reflection;
using Newtonsoft.Json;
using System.Linq;

namespace Web.GenerationCode.Provider
{
    public class NetCoreProvider : IGengerationTranslateProvider
    {
       
        private PlanGenerationWriteInput _context;
        
        public List<GengerationTranferTree> GenerateResult(PlanGenerationWriteInput context, List<GengerationComponentTreeData> treeDatas)
        {
            _context = context;

            return GengerationTranferTreeRecurrence(treeDatas);
        }

        public List<GengerationTranferTree> GengerationTranferTreeRecurrence(List<GengerationComponentTreeData> treeDatas)
        {

            List<GengerationTranferTree> result = new List<GengerationTranferTree>();

            foreach (var treeData in treeDatas)
            {
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
                        tree.Content = treeData.Content.IsNotNullOrNotWhiteSpace() ? ScribanHelper.Render(treeData.Content, new { Model = _context }, MemberRenamer) : "";
                        tree.FileName = treeData.Label.IsNotNullOrNotWhiteSpace() ? ScribanHelper.Render(treeData.Label, new { Model = _context }, MemberRenamer) : "";
                        _context.TableEntity = new TableEntityWriteInput();
                        tree.Children.AddRange(GengerationTranferTreeRecurrence(treeData.Children));

                        result.Add(tree);
                    }

                }
                else {
                    var tree = new GengerationTranferTree()
                    {
                          FileType = treeData.FileType,
                        IsTemplete = treeData.IsTemplete,
                        IsFolder = treeData.IsFolder,
                        NetworkAddress = treeData.NetworkAddress,
                    };
                    tree.Content = treeData.Content.IsNotNullOrNotWhiteSpace() ? ScribanHelper.Render(treeData.Content, new { Model = _context }, MemberRenamer) : "";
                    tree.FileName = treeData.Label.IsNotNullOrNotWhiteSpace() ? ScribanHelper.Render(treeData.Label, new { Model = _context }, MemberRenamer) : "";

                    tree.Children.AddRange(GengerationTranferTreeRecurrence(treeData.Children));

                    result.Add(tree);
                }


            }
            return result;



        }

        private string MemberRenamer(MemberInfo member)
        {
            return member.Name;
        }


    }
}
