namespace Web.Dto.TableEntitys
{
    public class TableNavigateRelativeDto : FullBaseDto
    {
        public TableNavigateType TableNavigateType { get; set; }

        public string TableNavigateTypeFormat => TableNavigateType.ToDescription();

        public Guid RelativeTableId { get; set; }

        public TableEntityDto RelativeTable { get; set; }
        /// <summary>
        /// 表A
        /// </summary>
        public Guid? AssociationATableId { get; set; }

        public TableEntityDto AssociationATableEntity { get; set; }

        /// <summary>
        /// 表A列Id
        /// </summary>
        public Guid? AssociationAColumnId { get; set; }

        public ColumnPropDto AssociationAColumnPropDto { get; set; }
        /// <summary>
        ///  表B
        /// </summary>
        public Guid? AssociationBTableId { get; set; }
        public TableEntityDto AssociationBTableEntity { get; set; }
        /// <summary>
        /// 表B列Id
        /// </summary>
        public Guid? AssociationBColumnId { get; set; }

        public ColumnPropDto AssociationBColumnPropDto { get; set; }
    }
}
