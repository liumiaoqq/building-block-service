using Google.Protobuf.WellKnownTypes;
using SqlSugar;

namespace Web
{
    /// <summary>
    /// 标准创建实体dto
    /// </summary>
    public abstract class CreationAuditedAggregateRoot 
    {
        [SugarColumn(IsPrimaryKey = true, ColumnDataType = "char(36)")]
        public virtual Guid Id { get; set; }
        public virtual DateTime CreationTime
        {
            get;
            set;
        }

        [SugarColumn(ColumnDataType = "char(36)", IsNullable = true)]
        public virtual Guid? CreatorId
        {
            get;
            set;
        }
        public virtual string  CreatorName
        {
            get;
            set;
        }
        public virtual bool IsDeleted
        {
            get;
            set;
        }

        protected CreationAuditedAggregateRoot()
        {
        }


    }
}
