namespace Web.Tables
{
    [Description("表关联关系")]
    [YoungTable("TableNavigateRelative")]
    public class TableNavigateRelative : CreationAuditedAggregateRoot
    {
        public TableNavigateType TableNavigateType { get; set; }

        public Guid RelativeTableId { get; set; }
        /// <summary>
        /// 表A
        /// </summary>
        public Guid? AssociationATableId { get; set; }

        /// <summary>
        /// 表A列Id
        /// </summary>
        public Guid? AssociationAColumnId { get; set; }

        /// <summary>
        ///  表B
        /// </summary>
        public Guid? AssociationBTableId { get; set; }

        /// <summary>
        /// 表B列Id
        /// </summary>
        public Guid? AssociationBColumnId { get; set; }
    }
}
