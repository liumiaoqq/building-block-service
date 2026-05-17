using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouJu.Infrastructure.Dto
{
    public  class PagedBaseInput : IPagedSkip, IPagedMaxResult
    {

        public bool IsAuth { get; set; }
        public string Keywords { get; set; }
        /// <summary>
        /// 跳过多少条
        /// </summary>
        public virtual int Skip { get; set; } = 0;

        /// <summary>
        /// 最大取值
        /// </summary>
        public virtual int MaxResult { get; set; } = 10;

        private int _Page = 0;

        private int _Size = 0;
        public virtual int Page
        {
            get
            {

                return _Page < 1 ? 1 : _Page;
            }
            set { this._Page = value; }
        }

        public virtual int Size
        {
            get
            {

                return _Size <=0? 999: _Size;
            }
            set { this._Size = value; }
        }

   

    }
}
