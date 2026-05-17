

using System;

using YouJu.Infrastructure;

namespace YouJu.Core.Dto
{

  
    public class FullBaseDto : IBaseDto<Guid?>
    {
    

        public virtual Guid? Id { get; set; }

        public bool IsEidt => Id.HasValue && Id.Value != Guid.Empty;



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
        public virtual string CreatorName
        {
            get;
            set;
        }


        public virtual string CreationTimeString => CreationTime.GetFullDateFormat();

        public virtual string CreationTimeDateBeforeNow => CreationTime.DateBeforeNow();


    }
}
