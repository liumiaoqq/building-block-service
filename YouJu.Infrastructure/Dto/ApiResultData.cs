using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouJu.Infrastructure.Dto
{
    public  class ApiResultData<TData>: ApiResult
    {


        public TData Data { get; set; }
    }

}
