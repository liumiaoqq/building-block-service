using System.Text;
using System.Text.RegularExpressions;

namespace Web.Manager
{
    public class ColumnPropManager
    {
        protected ISqlSugarClient _sqlSugarClient;

        public ColumnPropManager(ISqlSugarClient sqlSugarClient)
        {
            _sqlSugarClient = sqlSugarClient;
        }

        /// <summary>
        /// 得到对应列的数据
        /// </summary>
        public async Task<List<ColumnPropDto>> GetColumnPropList(ColumnPropPagedInput input)
        {

            var columns = await _sqlSugarClient.Queryable<ColumnProp>().Where(x => x.TableEntityId == input.TableEntityId).Select<ColumnPropDto>().ToListAsync();
            foreach (var item in columns)
            {
                item.ColumnPropTypeValue = item.ColumnPropType != ColumnPropType.枚举型 ? item.ColumnPropType.ToInt().ToString() : item.PlanEnumId.ToString();
            }
            return columns.OrderBy(x => x.CreationTime).ToList();

        }
        public async Task<List<ColumnPropDto>> GetColumnProps(ColumnPropPagedInput input)
        {

            if (input.PlanId.HasValue)
            {
                var tableEntityIds = await _sqlSugarClient.Queryable<TableEntity>().Where(x => x.PlanId == input.PlanId).Select(X => X.Id).ToListAsync();
                input.TableEntityIds = tableEntityIds;
            }

            var props = await _sqlSugarClient.Queryable<ColumnProp>()
                .WhereIF(input.TableEntityIds.HasItem(), x => input.TableEntityIds.Contains(x.TableEntityId))
                .WhereIF(input.UserId.HasValue, x => x.CreatorId == input.UserId)
                .WhereIF(input.TableEntityId.HasValue, x => x.TableEntityId == input.TableEntityId)
                .OrderBy(x => x.CreationTime)
                .Select<ColumnPropDto>()
                .ToListAsync();

            foreach (var item in props)
            {
                item.ColumnPropTypeValue = item.ColumnPropType != ColumnPropType.枚举型 ? item.ColumnPropType.ToInt().ToString() : item.PlanEnumId.ToString();
            }

            return props;
        }
        public PagedReuslt<SelectResult> GetColumnPropTypes(ColumnPropPagedInput input)
        {
            var selects = new List<SelectResult>();

            selects.Add(new SelectResult()
            {
                Name = ColumnPropType.字符串.ToDescription(),
                Value = ColumnPropType.字符串.ToInt().ToString(),
                Label = "基础",
                Prop = "0"
            });
            selects.Add(new SelectResult()
            {
                Name = ColumnPropType.整型.ToDescription(),
                Value = ColumnPropType.整型.ToInt().ToString(),
                Label = "基础",
                Prop = "0"
            });
            selects.Add(new SelectResult()
            {
                Name = ColumnPropType.双浮点型.ToDescription(),
                Value = ColumnPropType.双浮点型.ToInt().ToString(),
                Label = "基础",
                Prop = "0"
            });
            selects.Add(new SelectResult()
            {
                Name = ColumnPropType.布尔型.ToDescription(),
                Value = ColumnPropType.布尔型.ToInt().ToString(),
                Label = "基础",
                Prop = "0"
            });
            selects.Add(new SelectResult()
            {
                Name = ColumnPropType.时间.ToDescription(),
                Value = ColumnPropType.时间.ToInt().ToString(),
                Label = "基础",
                Prop = "0"
            });
            selects.Add(new SelectResult()
            {
                Name = ColumnPropType.多行文本.ToDescription(),
                Value = ColumnPropType.多行文本.ToInt().ToString(),
                Label = "基础",
                Prop = "0"
            });
            selects.Add(new SelectResult()
            {
                Name = ColumnPropType.长文本.ToDescription(),
                Value = ColumnPropType.长文本.ToInt().ToString(),
                Label = "基础",
                Prop = "0"
            });
            var specPropType = new List<string>() {
                 ColumnPropType.起始截至日期.ToInt().ToString(),
                 ColumnPropType.起始截至时间.ToInt().ToString(),
                 ColumnPropType.图片.ToInt().ToString(),
                 ColumnPropType.视频.ToInt().ToString(),
                 ColumnPropType.音频.ToInt().ToString(),
                 ColumnPropType.文件.ToInt().ToString(),
                  };
            foreach (var x in typeof(ColumnPropType).GetEnumList())
            {
                // if (x.Value == ColumnPropType.日期.ToInt().ToString())
                // {
                //     continue;
                // }
                // if (x.Value == ColumnPropType.小数点.ToInt().ToString())
                // {
                //     continue;
                // }

                // if (x.Value == ColumnPropType.枚举型.ToInt().ToString())
                // {
                //     continue;
                // }


                if (specPropType.Contains(x.Value))
                {
                    var select = new SelectResult()
                    {
                        Name = x.Key,
                        Value = x.Value,
                        Label = "基础",
                        Prop = "0"
                    };
                    select.Label = "高级扩展";
                    selects.Add(select);
                }


            }




            if (input.PlanId.HasValue)
            {

                var enums = _sqlSugarClient.Queryable<PlanEnum>().Where(x => x.PlanId == input.PlanId).OrderBy(x=>x.Code).ToList().Select(x => new SelectResult
                {
                    Name = x.Code,
                    Value = x.Id.ToString(),
                    Label = "枚举",
                    Prop = "1"

                });
                selects.AddRange(enums);
            }


            return new PagedReuslt<SelectResult>(selects, selects.Count);

        }
        public async Task BatchCreateOrEditColumnProp(List<ColumnPropDto> input)
        {


            //对编码和名称为空的字段直接进行过滤
            input = input.Where(x => x.Name.IsNotNullOrNotWhiteSpace() && x.Code.IsNotNullOrNotWhiteSpace()).ToList();

            //去除操作可能的空格
            foreach (var item in input)
            {
                item.Code = item.Code.Replace(" ", "");
                item.Name = item.Name.Replace(" ", "");

            }

            var tableEntityId = input.FirstOrDefault().TableEntityId;
            var planId = input.FirstOrDefault().PlanId;
            var props = GetColumnPropTypes(new ColumnPropPagedInput() { PlanId = planId });
            var list = new List<ColumnProp>();
            var content = new StringBuilder();

            var ids = input.Where(x => x.Id.HasValue).Select(x => x.Id.Value).ToList();

            var columns = await _sqlSugarClient.Queryable<ColumnProp>().Where(x => ids.Contains(x.Id)).ToListAsync();

            if (input.GroupBy(x => x.Code).Select(x => new { Count = x.Count() }).ToList().Exists(h => h.Count > 1))
            {
                throw new YouJuException("存在2个相同的表字段");
            }



            foreach (var item in input)
            {
                ValidFiledCode(item);
                ValidFiledDisplay(item.Display);
                ValidFiledLength(item.Length);
                ValidFiledName(item.Name);
            }


            foreach (var item in input)
            {
                var ct = string.Empty;
                var index = input.IndexOf(item) + 1;


                if (item.Code.IsNullOrWhiteSpace())
                {

                    ct += "编码不能为空  ";
                }
                if (item.Name.IsNullOrWhiteSpace())
                {
                    ct += "名称不能为空  ";
                }
                if (ct.IsNotNullOrNotWhiteSpace())
                {
                    ct = $"第{index}行,{ct}";
                    content.AppendLine(ct);
                }
                else
                {
                    var columnProp = new ColumnProp()
                    {
                        Id = item.Id.HasValue && ids.Contains(item.Id.Value) ? item.Id.Value : Guid.Empty,
                        TableEntityId = tableEntityId.Value,
                        Code = item.Code.Trim(),
                        ColumnPropType = item.ColumnPropType,
                        Display = item.Display.Trim(),
                        IsNull = item.IsNull,
                        Length = item.Length,
                        PlanEnumId = item.PlanEnumId,
                        Name = item.Name.Trim(),
                        CreationTime = DateTime.Now.AddSeconds(index),


                    };
                    var prop = props.Items.FirstOrDefault(x => x.Value == item.ColumnPropTypeValue);
                    if (prop.Prop.ToString() == "1")
                    {
                        columnProp.ColumnPropType = ColumnPropType.枚举型;
                        columnProp.PlanEnumId = Guid.Parse(item.ColumnPropTypeValue);
                    }
                    else
                    {
                        columnProp.ColumnPropType = (ColumnPropType)Enum.Parse(typeof(ColumnPropType), item.ColumnPropTypeValue);
                    }

                    list.Add(columnProp);
                }
                if (content.ToString().IsNotNullOrNotWhiteSpace())
                {
                    throw new YouJuException(content.ToString());
                }
            }


            await _sqlSugarClient.Deleteable<ColumnProp>().Where(x => x.TableEntityId == tableEntityId).ExecuteCommandAsync();

            await _sqlSugarClient.Insertable(list).ExecuteCommandAsync();
        }


        /// <summary>
        /// AI自动修复长度
        /// </summary>
        public async Task AIFixColumnLengthSaveAsync(List<AiFixColumnLengthTableDto> input)
        {
            var columnsList = input.SelectMany(x => x.Columns).ToList();

            var columnPropIds = columnsList.Select(x => x.ColumnPropId).Distinct().ToList();
            var columnProps = await _sqlSugarClient.Queryable<ColumnProp>().Where(x => columnPropIds.Contains(x.Id)).ToListAsync();
            foreach (var columnProp in columnProps)
            {
                var newColumn = columnsList.FirstOrDefault(x => x.ColumnPropId == columnProp.Id);
                if (newColumn != null)
                {
                    columnProp.Length = newColumn.NewLength;
                }

            }
            await _sqlSugarClient.Updateable(columnProps).ExecuteCommandAsync();
        }

        /// <summary>
        /// AI自动列排序 - 通过调整创建时间实现排序
        /// </summary>
        public async Task AiSortColumnSaveAsync(List<AiSortColumnTableDto> input)
        {
            // 按表分组处理
            foreach (var table in input)
            {
                var columnsList = table.Columns.Where(x => x.NewSort.HasValue).ToList();
                if (columnsList.Count == 0)
                {
                    continue;
                }

                var columnPropIds = columnsList.Select(x => x.ColumnPropId).ToList();
                var columnProps = await _sqlSugarClient.Queryable<ColumnProp>()
                    .Where(x => columnPropIds.Contains(x.Id))
                    .ToListAsync();

                // 使用一个基准时间，从当前时间开始
                var baseTime = DateTime.Now;

                // 根据NewSort重新分配创建时间
                foreach (var columnProp in columnProps)
                {
                    var newColumn = columnsList.FirstOrDefault(x => x.ColumnPropId == columnProp.Id);
                    if (newColumn != null && newColumn.NewSort.HasValue)
                    {
                        // 按照NewSort的顺序设置创建时间，每个列间隔1秒
                        columnProp.CreationTime = baseTime.AddSeconds(newColumn.NewSort.Value);
                    }
                }

                // 只更新CreationTime字段
                await _sqlSugarClient.Updateable(columnProps).UpdateColumns(x => new { x.CreationTime }).ExecuteCommandAsync();
            }
        }

        public static void ValidFiledName(string value)
        {
            if (value.IsNullOrWhiteSpace())
            {
                throw new YouJuException("列名称不能为空");
            }
            if (value.Length > 40)
            {
                throw new YouJuException("列名称长度不能超过40个字符");
            }

        }
        public static void ValidFiledDisplay(string value)
        {

            if (value.IsNotNullOrNotWhiteSpace() && value.Length > 20)
            {
                throw new YouJuException("列解释长度不能超过20个字符");
            }

        }

        public static void ValidFiledCode(ColumnPropDto input)
        {
            if (input.Code.IsNullOrWhiteSpace())
            {
                throw new YouJuException("列编码不能为空");
            }
            if (input.Code.Length > 40)
            {
                throw new YouJuException("列编码长度不能超过40个字符");
            }
            //编码只能是字母
            if (!Regex.IsMatch(input.Code, "^[a-zA-Z]+$"))
            {
                throw new YouJuException("列编码只能是字母,请勿输入数字或则其他特殊符号");
            }
            //编码首字母只能是大写
            if (!char.IsUpper(input.Code[0]))
            {
                throw new YouJuException("列编码首字母只能是大写,请使用首字母大写的驼峰命名规则");
            }
            //如果编码结尾是Ld 则提示他错误
            if (input.Code.EndsWith("ld") && input.ColumnPropType == ColumnPropType.整型)
            {
                throw new YouJuException("列编码请使用首字母大写的id,而不是首字母大写的ld");
            }

        }

        public static void ValidFiledLength(int? value)
        {
            if (value.HasValue && value.Value > 9999)
            {
                throw new YouJuException("列的长度不能大于9999,建议使用text类型");
            }


        }


    }
}
