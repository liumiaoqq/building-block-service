using Google.Protobuf.WellKnownTypes;
using SqlSugar;

namespace Web
{
    /// <summary>
    /// 标准创建实体dto
    /// </summary>
    public abstract class CreationAuditedAggregateRoot 
    {
        [SugarColumn(IsPrimaryKey = true,ColumnDataType = "uniqueidentifier")]
        public virtual Guid Id { get; set; }
        public virtual DateTime CreationTime
        {
            get;
            set;
        }

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