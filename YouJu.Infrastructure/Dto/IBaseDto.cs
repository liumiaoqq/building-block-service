using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouJu.Core.Dto
{
    public interface  IBaseDto<TKey>
    {
        public abstract TKey Id { get; set; }
    }
}
