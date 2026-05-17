using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.Util;
using NPOI.XWPF.UserModel;
using System.IO;
using System.Reflection;
using System.Security.Policy;
using Web.Dto.Gengerations;
using Web.Dto.Modules;
using Web.Dto.Plans;
using Web.Extensions;
using Web.GenerationCode;
using Web.GenerationCode.Dto;
using Web.GenerationCode.Manager;
using Web.Manager;
using Web.Service;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlanController : YouJuController<Plan, PlanDto, PlanPagedInput>
    {

        private readonly PlanManager _planManager;

        public readonly SystemModuleManager _systemModuleManager;

        private readonly IGenerationCodeManager _generationCodeManager;

        private readonly IGengerationTranferTreeFileService _gengerationTranferTreeFileService;

        private readonly PlanService _planService;

        public PlanController(IServiceProvider serviceProvider, PlanManager planManager, SystemModuleManager systemModuleManager, IGenerationCodeManager generationCodeManager, IGengerationTranferTreeFileService gengerationTranferTreeFileService, PlanService planService) : base(serviceProvider)
        {
            _planManager = planManager;
            _systemModuleManager = systemModuleManager;
            _generationCodeManager = generationCodeManager;
            _gengerationTranferTreeFileService = gengerationTranferTreeFileService;
            _planService = planService;
        }


        [HttpPost("ListAsync")]
        [CustomAuthorization(RoleType.系统管理员, RoleType.用户)]
        public override async Task<PagedReuslt<PlanDto>> ListAsync(PlanPagedInput input)
        {
            return await _planService.ListAsync(input);
        }
        [HttpPost("GetAsync")]
        [CustomAuthorization(RoleType.系统管理员, RoleType.用户)]
        public override async Task<PlanDto> GetAsync(IdInput<Guid> input)
        {
            var query = new PlanPagedInput() { PlanId = input.Id };

            var dto = (await ListAsync(query)).Items.FirstOrDefault();
            return dto ?? new PlanDto();
        }

        /// <summary>
        /// 得到计划关联的模块
        /// </summary>
        [HttpPost("GetPlanRelativeModules")]
        [CustomAuthorization(RoleType.系统管理员)]
        public async Task<List<ModuleRelativeDtos>> GetPlanRelativeModules(ModuleRelativePagedInput input)
        {
            return await _systemModuleManager.GetPlanRelativeModules(input);
        }
        [HttpPost("ExportGenerationResult")]
        public async Task<string> ExportGenerationResult(PlanPagedInput input)
        {
            var result = await _planManager.GetGenerationModelAndTempleteByPlan(input);

            var generationCodeResult = _generationCodeManager
            .LoadModule(result.PlanGenerationWriteInput, result.GengerationComponentTreeDatas)
            .TempleteConvert()
            .PostFilter(input.PostFilterKeyWords)
            .GetConvertResult();

            var fileUrl = _gengerationTranferTreeFileService.InitSettings(result.PlanGenerationWriteInput.PlanName, result.PlanGenerationWriteInput.PlanName).GetTranferDownAddress(generationCodeResult);

            return fileUrl;

        }

        #region 预览
        /// <summary>
        /// 根据关联模块预览代码
        /// </summary>
        [HttpPost("LookByModuleId")]

        public async Task<List<GengerationTranferTree>> LookByModuleId(PlanPagedInput input)
        {

            var result = await _planManager.GewtGenerationModelAndTempleteByModuleId(input);
            var generationCodeResult = _generationCodeManager.LoadModule(result.PlanGenerationWriteInput, result.GengerationComponentTreeDatas).TempleteConvert().GetConvertResult();
            return generationCodeResult;
        }
        /// <summary>
        /// 根据计划模块预览代码
        /// </summary>
        [HttpPost("Look")]

        public async Task<List<GengerationTranferTree>> Look(PlanPagedInput input)
        {

            if (input.LookType == LookType.Plan)
            {
                var result = await _planManager.GetGenerationModelAndTempleteByPlan(input);
                var generationCodeResult = _generationCodeManager
                .LoadModule(result.PlanGenerationWriteInput, result.GengerationComponentTreeDatas)
                .TempleteConvert()
                .PostFilter(input.PostFilterKeyWords)
                .GetConvertResult();
                return generationCodeResult;
            }
            else
            {
                var result = await _planManager.GewtGenerationModelAndTempleteByModuleId(input);
                var generationCodeResult = _generationCodeManager
                .LoadModule(result.PlanGenerationWriteInput, result.GengerationComponentTreeDatas)
                .TempleteConvert()
                .PostFilter(input.PostFilterKeyWords)
                .GetConvertResult();
                return generationCodeResult;
            }
        }



        #endregion

        /// <summary>
        /// 查看计划输入模型上下文
        /// </summary>
        [HttpPost("LookJson")]


        public async Task<PlanGenerationWriteInput> LookJson(PlanPagedInput input)
        {
            var result = await _planManager.GetGenerationModelAndTempleteByPlan(input);
            return result.PlanGenerationWriteInput;

        }

        [HttpPost("GetSqlResult")]

        public async Task<PlanGengerationSqlDto> GetSqlResult(PlanPagedInput input)
        {
            var result = await _planManager.PlanSqlTempletePagedInput(input);
            return result;

        }

        [HttpPost("GetLanguageWay")]

        public PagedReuslt<SelectResult> GetLanguageWay()
        {
            var roles = new List<SelectResult>();

            roles = typeof(LanguageWay).GetEnumList().Select(x => new SelectResult() { Name = x.Key, Value = x.Value }).ToList();
            return new PagedReuslt<SelectResult>(roles, roles.Count);

        }
        /// <summary>
        /// 得到方案类型
        /// </summary>
        /// <returns></returns>

        [HttpPost("GetPlanType")]

        public PagedReuslt<SelectResult> GetPlanType()
        {
            var roles = new List<SelectResult>();

            roles = typeof(PlanType).GetEnumList().Select(x => new SelectResult() { Name = x.Key, Value = x.Value }).ToList();
            return new PagedReuslt<SelectResult>(roles, roles.Count);

        }


        /// <summary>
        /// 导出表结构
        /// </summary>

        [HttpGet("ExportTableStructAsnyc")]
        public async Task<FileContentResult> ExportTableStructAsnyc(string query)
        {
            var input = JsonConvert.DeserializeObject<PlanPagedInput>(query);

            var result = await _planManager.GetGenerationModelAndTempleteByPlan(input);

            XWPFDocument doc = new XWPFDocument(); //创建新的word文档


            #region 开始写标题
            // 添加段落
            XWPFParagraph gp = doc.CreateParagraph();
            gp.Alignment = ParagraphAlignment.CENTER;//水平居中

            XWPFRun gr = gp.CreateRun();
            gr.GetCTR().AddNewRPr().AddNewRFonts().ascii = "黑体";
            gr.GetCTR().AddNewRPr().AddNewRFonts().eastAsia = "黑体";
            gr.GetCTR().AddNewRPr().AddNewRFonts().hint = ST_Hint.eastAsia;
            gr.GetCTR().AddNewRPr().AddNewSz().val = (ulong)36;
            gr.GetCTR().AddNewRPr().AddNewSzCs().val = (ulong)36;
            gr.GetCTR().AddNewRPr().AddNewB().val = true; //加粗
            gr.GetCTR().AddNewRPr().AddNewColor().val = "black";//字体颜色


            gr.SetText(result.PlanGenerationWriteInput.PlanName + "系统数据库表格word文档");


            gr.AddCarriageReturn();//换行回车             
            gr.AddCarriageReturn();
            gr.AddCarriageReturn();

            #endregion


            var tableTypes = DbOptions.GetTables;


            foreach (var item in result.PlanGenerationWriteInput.TableEntitys)
            {



                gp = doc.CreateParagraph();
                gp.Alignment = ParagraphAlignment.LEFT;//水平居中

                gr = gp.CreateRun();//创建一个段落

                gr.GetCTR().AddNewRPr().AddNewRFonts().ascii = "黑体";
                gr.GetCTR().AddNewRPr().AddNewRFonts().eastAsia = "黑体";
                gr.GetCTR().AddNewRPr().AddNewRFonts().hint = ST_Hint.eastAsia;
                gr.GetCTR().AddNewRPr().AddNewSz().val = (ulong)36;
                gr.GetCTR().AddNewRPr().AddNewSzCs().val = (ulong)36;
                gr.GetCTR().AddNewRPr().AddNewB().val = false; //加粗
                gr.GetCTR().AddNewRPr().AddNewColor().val = "black";//字体颜色
                gr.FontSize = 10;
                gr.SetText($"{item.Code}（{item.Name}表）");

                //6 字段名	数据类型	长度	Null	主键	说明

                //count 是多占一行的
                var count = item.ColumnProps.Count(x => x.ColumnPropType.IsIn(ColumnPropType.起始截至日期, ColumnPropType.起始截至时间));



                XWPFTable firstXwpfTable = doc.CreateTable(item.ColumnProps.Count + 4 + count, 6);//显示的行列数rows:8行,cols:3列

                firstXwpfTable.Width = 5400;//总宽度
                firstXwpfTable.SetColumnWidth(0, 900); /* 设置列宽 */
                firstXwpfTable.SetColumnWidth(1, 900); /* 设置列宽 */
                firstXwpfTable.SetColumnWidth(2, 900); /* 设置列宽 */
                firstXwpfTable.SetColumnWidth(3, 900); /* 设置列宽 */
                firstXwpfTable.SetColumnWidth(4, 900); /* 设置列宽 */
                firstXwpfTable.SetColumnWidth(5, 900); /* 设置列宽 */

                firstXwpfTable.SetTopBorder(XWPFTable.XWPFBorderType.SINGLE, 0, 0, "#000000");
                firstXwpfTable.SetBottomBorder(XWPFTable.XWPFBorderType.SINGLE, 0, 0, "#000000");
                firstXwpfTable.SetLeftBorder(XWPFTable.XWPFBorderType.SINGLE, 0, 0, "#000000");
                firstXwpfTable.SetRightBorder(XWPFTable.XWPFBorderType.SINGLE, 0, 0, "#000000");
                firstXwpfTable.SetInsideHBorder(XWPFTable.XWPFBorderType.SINGLE, 0, 0, "#000000");
                firstXwpfTable.SetInsideVBorder(XWPFTable.XWPFBorderType.SINGLE, 0, 0, "#000000");
                firstXwpfTable.SetCellMargins(5, 5, 5, 5);

                var index = 0;

                firstXwpfTable.GetRow(index).GetCell(0).SetParagraph(SetTableParagraphInstanceSetting(doc, firstXwpfTable, "字段名", ParagraphAlignment.CENTER, 24, true, 10));
                firstXwpfTable.GetRow(index).GetCell(1).SetParagraph(SetTableParagraphInstanceSetting(doc, firstXwpfTable, "数据类型", ParagraphAlignment.CENTER, 24, true, 10));
                firstXwpfTable.GetRow(index).GetCell(2).SetParagraph(SetTableParagraphInstanceSetting(doc, firstXwpfTable, "长度", ParagraphAlignment.CENTER, 24, true, 10));
                firstXwpfTable.GetRow(index).GetCell(3).SetParagraph(SetTableParagraphInstanceSetting(doc, firstXwpfTable, "是否可空", ParagraphAlignment.CENTER, 24, true, 10));
                firstXwpfTable.GetRow(index).GetCell(4).SetParagraph(SetTableParagraphInstanceSetting(doc, firstXwpfTable, "主外键", ParagraphAlignment.CENTER, 24, true, 10));
                firstXwpfTable.GetRow(index).GetCell(5).SetParagraph(SetTableParagraphInstanceSetting(doc, firstXwpfTable, "说明", ParagraphAlignment.CENTER, 24, true, 10));


                index++;

                firstXwpfTable.GetRow(index).GetCell(0).SetParagraph(SetTableParagraphInstanceSetting(doc, firstXwpfTable, "Id", ParagraphAlignment.CENTER, 24, true, 10));

                firstXwpfTable.GetRow(index).GetCell(1).SetParagraph(SetTableParagraphInstanceSetting(doc, firstXwpfTable, "int", ParagraphAlignment.CENTER, 24, true, 10));


                firstXwpfTable.GetRow(index).GetCell(2).SetParagraph(SetTableParagraphInstanceSetting(doc, firstXwpfTable, "11", ParagraphAlignment.CENTER, 24, false));


                firstXwpfTable.GetRow(index).GetCell(3).SetParagraph(SetTableParagraphInstanceSetting(doc, firstXwpfTable, "非空", ParagraphAlignment.CENTER, 24, false));

                firstXwpfTable.GetRow(index).GetCell(4).SetParagraph(SetTableParagraphInstanceSetting(doc, firstXwpfTable, $"主键", ParagraphAlignment.CENTER, 24, false));

                firstXwpfTable.GetRow(index).GetCell(5).SetParagraph(SetTableParagraphInstanceSetting(doc, firstXwpfTable, $"{item.Name}主键", ParagraphAlignment.CENTER, 24, false));
                index++;

                foreach (var prop in item.ColumnProps)
                {
                    if (prop.ColumnPropType.IsIn(ColumnPropType.起始截至日期, ColumnPropType.起始截至时间))
                    {

                        firstXwpfTable.GetRow(index).GetCell(0).SetParagraph(SetTableParagraphInstanceSetting(doc, firstXwpfTable, $"Begin{prop.Code}", ParagraphAlignment.CENTER, 24, true, 10));

                        firstXwpfTable.GetRow(index).GetCell(1).SetParagraph(SetTableParagraphInstanceSetting(doc, firstXwpfTable, GetPropColumnType(prop), ParagraphAlignment.CENTER, 24, true, 10));


                        firstXwpfTable.GetRow(index).GetCell(2).SetParagraph(SetTableParagraphInstanceSetting(doc, firstXwpfTable, GetPropColumnLength(prop), ParagraphAlignment.CENTER, 24, false));


                        firstXwpfTable.GetRow(index).GetCell(3).SetParagraph(SetTableParagraphInstanceSetting(doc, firstXwpfTable, GetPropColumnIsNull(prop), ParagraphAlignment.CENTER, 24, false));

                        firstXwpfTable.GetRow(index).GetCell(5).SetParagraph(SetTableParagraphInstanceSetting(doc, firstXwpfTable, $"开始{prop.Name}", ParagraphAlignment.CENTER, 24, false));


                        index++;

                        firstXwpfTable.GetRow(index).GetCell(0).SetParagraph(SetTableParagraphInstanceSetting(doc, firstXwpfTable, $"End{prop.Code}", ParagraphAlignment.CENTER, 24, true, 10));

                        firstXwpfTable.GetRow(index).GetCell(1).SetParagraph(SetTableParagraphInstanceSetting(doc, firstXwpfTable, GetPropColumnType(prop), ParagraphAlignment.CENTER, 24, true, 10));


                        firstXwpfTable.GetRow(index).GetCell(2).SetParagraph(SetTableParagraphInstanceSetting(doc, firstXwpfTable, GetPropColumnLength(prop), ParagraphAlignment.CENTER, 24, false));


                        firstXwpfTable.GetRow(index).GetCell(3).SetParagraph(SetTableParagraphInstanceSetting(doc, firstXwpfTable, GetPropColumnIsNull(prop), ParagraphAlignment.CENTER, 24, false));

                        firstXwpfTable.GetRow(index).GetCell(5).SetParagraph(SetTableParagraphInstanceSetting(doc, firstXwpfTable, $"结束{prop.Name}", ParagraphAlignment.CENTER, 24, false));


                        index++;
                    }
                    else
                    {
                        firstXwpfTable.GetRow(index).GetCell(0).SetParagraph(SetTableParagraphInstanceSetting(doc, firstXwpfTable, prop.Code, ParagraphAlignment.CENTER, 24, true, 10));

                        firstXwpfTable.GetRow(index).GetCell(1).SetParagraph(SetTableParagraphInstanceSetting(doc, firstXwpfTable, GetPropColumnType(prop), ParagraphAlignment.CENTER, 24, true, 10));


                        firstXwpfTable.GetRow(index).GetCell(2).SetParagraph(SetTableParagraphInstanceSetting(doc, firstXwpfTable, GetPropColumnLength(prop), ParagraphAlignment.CENTER, 24, false));


                        firstXwpfTable.GetRow(index).GetCell(3).SetParagraph(SetTableParagraphInstanceSetting(doc, firstXwpfTable, GetPropColumnIsNull(prop), ParagraphAlignment.CENTER, 24, false));

                        var tableNavigateRelativeWirteInput = item.TableNavigateRelatives.Where(x => x.TableNavigateType == TableNavigateType.OneToOne && x.AssociationAColumnId == prop.ColumnPropId).FirstOrDefault();
                        if (tableNavigateRelativeWirteInput != null)
                        {
                            firstXwpfTable.GetRow(index).GetCell(4).SetParagraph(SetTableParagraphInstanceSetting(doc, firstXwpfTable, $"{tableNavigateRelativeWirteInput.AssociationATableEntity.Name}表外键", ParagraphAlignment.CENTER, 24, false));
                        }

                        firstXwpfTable.GetRow(index).GetCell(5).SetParagraph(SetTableParagraphInstanceSetting(doc, firstXwpfTable, $"{prop.Name}", ParagraphAlignment.CENTER, 24, false));

                        if (prop.ColumnPropType == ColumnPropType.枚举型)
                        {

                            var planEnum = result.PlanGenerationWriteInput.PlanEnums.FirstOrDefault(x => x.Code.Contains(prop.Code));
                            if (planEnum != null)
                            {
                                var enumProps = planEnum.EnumPropsList.Select(x => $"{x.Value}:{x.Code}").JoinAsString(",");
                                string comment = $"{planEnum.Name}（{enumProps}）";

                                firstXwpfTable.GetRow(index).GetCell(5).SetParagraph(SetTableParagraphInstanceSetting(doc, firstXwpfTable, comment, ParagraphAlignment.CENTER, 24, false));
                            }
                        }




                        index++;
                    }
                }





                #region 创建时间

                firstXwpfTable.GetRow(index).GetCell(0).SetParagraph(SetTableParagraphInstanceSetting(doc, firstXwpfTable, "CreationTime", ParagraphAlignment.CENTER, 24, true, 10));

                firstXwpfTable.GetRow(index).GetCell(1).SetParagraph(SetTableParagraphInstanceSetting(doc, firstXwpfTable, "timestamp", ParagraphAlignment.CENTER, 24, true, 10));


                firstXwpfTable.GetRow(index).GetCell(2).SetParagraph(SetTableParagraphInstanceSetting(doc, firstXwpfTable, "0", ParagraphAlignment.CENTER, 24, false));


                firstXwpfTable.GetRow(index).GetCell(3).SetParagraph(SetTableParagraphInstanceSetting(doc, firstXwpfTable, "非空", ParagraphAlignment.CENTER, 24, false));

                firstXwpfTable.GetRow(index).GetCell(4).SetParagraph(SetTableParagraphInstanceSetting(doc, firstXwpfTable, $"", ParagraphAlignment.CENTER, 24, false));

                firstXwpfTable.GetRow(index).GetCell(5).SetParagraph(SetTableParagraphInstanceSetting(doc, firstXwpfTable, $"创建时间", ParagraphAlignment.CENTER, 24, false));
                index++;
                #endregion
            }



            //写入字节数组输出流
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            doc.Write(baos);
            //写入内存流
            var memory = new MemoryStream(baos.ToByteArray());


            var mime = new FileExtensionContentTypeProvider().Mappings[".doc"];
            HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "content-disposition");
            return File(baos.ToByteArray(), mime, result.PlanGenerationWriteInput.PlanName + "系统表结构" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".doc");//返回文件流，type是文件格式

        }



        [NonAction]
        private string GetPropColumnType(ColumnPropWriteInput prop)
        {
            if (prop.ColumnPropType == ColumnPropType.整型)
            {
                return "int";
            }
            else if (prop.ColumnPropType == ColumnPropType.双浮点型)
            {
                return "double";
            }
            else if (prop.ColumnPropType == ColumnPropType.布尔型)
            {
                return "tinyint";
            }
            else if (prop.ColumnPropType == ColumnPropType.时间)
            {
                return "timestamp";
            }
            else if (prop.ColumnPropType == ColumnPropType.小数点)
            {
                return "double";
            }
            else if (prop.ColumnPropType == ColumnPropType.字符串)
            {
                return "varchar";
            }
            else if (prop.ColumnPropType == ColumnPropType.长文本)
            {
                return "text";
            }
            else if (prop.ColumnPropType == ColumnPropType.枚举型)
            {
                return "int";
            }
            else if (prop.ColumnPropType == ColumnPropType.日期)
            {
                return "timestamp";
            }
            return "varchar";
        }

        [NonAction]
        private string GetPropColumnLength(ColumnPropWriteInput prop)
        {

            if (prop.ColumnPropType == ColumnPropType.整型)
            {
                return "11";
            }
            else if (prop.ColumnPropType == ColumnPropType.双浮点型)
            {
                return "20";
            }
            else if (prop.ColumnPropType == ColumnPropType.布尔型)
            {
                return "1";
            }
            else if (prop.ColumnPropType == ColumnPropType.时间)
            {
                return "0";
            }
            else if (prop.ColumnPropType == ColumnPropType.小数点)
            {
                return "20";
            }
            else if (prop.ColumnPropType == ColumnPropType.字符串)
            {
                return prop.Length.HasValue && prop.Length > 0 ? prop.Length.ToString() : "128";
            }
            else if (prop.ColumnPropType == ColumnPropType.长文本)
            {
                return "0";
            }
            else if (prop.ColumnPropType == ColumnPropType.枚举型)
            {
                return "11";
            }
            else if (prop.ColumnPropType == ColumnPropType.日期)
            {
                return "0";
            }
            return "0";
        }

        [NonAction]
        private string GetPropColumnIsNull(ColumnPropWriteInput prop)
        {

            return prop.IsNull ? "非空" : "空";
        }

        /// <summary> 
        /// 创建Word文档中表格段落实例和设置表格段落文本的基本样式（字体大小，字体，字体颜色，字体对齐位置）
        /// </summary> 
        /// <param name="document">document文档对象</param> 
        /// <param name="table">表格对象</param> 
        /// <param name="fillContent">要填充的文字</param> 
        /// <param name="paragraphAlign">段落排列（左对齐，居中，右对齐）</param>
        /// <param name="textPosition">设置文本位置（设置两行之间的行间,从而实现表格文字垂直居中的效果），从而实现table的高度设置效果 </param>
        /// <param name="isBold">是否加粗（true加粗，false不加粗）</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="fontColor">字体颜色--十六进制</param>
        /// <param name="isItalic">是否设置斜体（字体倾斜）</param>
        /// <returns></returns> 


        [NonAction]
        private XWPFParagraph SetTableParagraphInstanceSetting(XWPFDocument document, XWPFTable table, string fillContent, ParagraphAlignment paragraphAlign, int textPosition = 24, bool isBold = false, double fontSize = 10, string fontColor = "000000", bool isItalic = false)
        {
            var para = new CT_P();
            //设置单元格文本对齐
            // para.AddNewPPr().AddNewTextAlignment();

            XWPFParagraph paragraph = new XWPFParagraph(para, table.Body);//创建表格中的段落对象
            paragraph.Alignment = paragraphAlign;//文字显示位置,段落排列（左对齐，居中，右对齐）
                                                 //paragraph.FontAlignment =Convert.ToInt32(ParagraphAlignment.CENTER);//字体在单元格内显示位置与 paragraph.Alignment效果相似

            XWPFRun xwpfRun = paragraph.CreateRun();//创建段落文本对象

            xwpfRun.SetText(fillContent);

            xwpfRun.FontSize = fontSize;//字体大小
            xwpfRun.SetColor(fontColor);//设置字体颜色--十六进制
            xwpfRun.IsItalic = isItalic;//是否设置斜体（字体倾斜）
            xwpfRun.IsBold = isBold;//是否加粗
            xwpfRun.SetFontFamily("宋体", FontCharRange.None);//设置字体（如：微软雅黑,华文楷体,宋体）
            return paragraph;

        }

        #region 用户

        /// <summary>
        /// 得到用户选模板
        /// </summary>
        [CustomAuthorization(RoleType.用户)]
        [HttpPost("GetUserPlanTempleteList")]
        public async Task<PagedReuslt<SelectResult>> GetUserPlanTempleteList()
        {

            var userId = CurrentUser.GetUserId();

            var plans = await SqlSugarClient.Queryable<Plan>().Where(x => x.IsTemplete == true).ToListAsync();

            var rs = new List<SelectResult>();

            rs = plans.Select(x => new SelectResult() { Name = x.PlanName, Value = x.Id.ToString() }).ToList();
            return new PagedReuslt<SelectResult>(rs, rs.Count);
        }


        /// <summary>
        /// 用户选中对于的模板
        /// </summary>
        [CustomAuthorization(RoleType.用户)]
        [HttpPost("UserPlanTempleteSubmitAsync")]
        public async Task UserPlanTempleteSubmitAsync(UserPlanTempleteSubmitInput input)
        {
            await _planService.UserPlanTempleteSubmitAsync(input);
        }

        #endregion
        /// <summary>
        /// 维护方法
        /// </summary>
        [CustomAuthorization(RoleType.系统管理员)]
        [HttpPost("ResetUserPlanTempleteSubmitAsync")]
        public async Task ResetUserPlanTempleteSubmitAsync()
        {
            await _planService.ResetUserPlanTempleteSubmitAsync();
        }

    }
}
