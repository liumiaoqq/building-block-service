using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouJu.Infrastructure.Dto
{
    public interface  IPagedSkip
    {
        public abstract int Skip { get; set; }


        public abstract int Page { get; set; }
    }
}
