using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouJu.Infrastructure.Dto
{
  
    public interface IPagedMaxResult
    {
        public abstract int MaxResult { get; set; }

        public abstract int Size { get; set; }
        
    }
}
