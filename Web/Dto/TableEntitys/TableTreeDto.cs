namespace Web.Dto.TableEntitys
{
    public class TableTreeDto
    {
        public Guid Id { get; set; }
        [Description("实体名称")]
        public string Label { get; set; }

        [Description("实体编码")]
        public string Code { get; set; }

        [Description("父级模板")]
        public Guid? ParentId { get; set; }

        [Description("是否额外")]
        public bool IsExtra { get; set; }

        [Description("计划ID")]
        public Guid? PlanId { get; set; }

        [Description("列数量")]
        public int ColumnCount { get; set; }


        public bool IsTable { get; set; }

        public bool IsIndex { get; set; }

        public bool IsCol { get; set; }


        public bool IsBuiltin { get; set; }
        /// <summary>
        /// 是否可编辑
        /// </summary>
        public bool IsCanEdit { get; set; }

        /// <summary>
        /// 是否可编辑
        /// </summary>
        public bool IsCanDelete { get; set; }
        /// <summary>
        /// 显示右击
        /// </summary>
        public bool VisiableRightClick { get; set; }


        /// <summary>
        /// 是否保存配置
        /// </summary>
        public bool IsSeetingSaved { get; set; }

        /// <summary>
        /// 关联表数
        /// </summary>
        public int TableNavigateRelativeCount { get; set; }

        public List<TableTreeDto> Children { get; set; }


        public TableTreeDto()
        {
            Children = new List<TableTreeDto>();


        }


    }
}
