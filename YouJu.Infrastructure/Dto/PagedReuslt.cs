using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouJu.Infrastructure.Dto
{
    public  class PagedReuslt<T> where T:class
    {
        public long TotalCount { get; set; }

        public List<T> Items { get; set; }

        public PagedReuslt(List<T> items,long totalCount) {
            this.Items = items;
            TotalCount=totalCount;
        }
        public PagedReuslt()
        {
            this.Items = new List<T>();
            TotalCount = 0;
        }
    }
}
