using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouJu.Infrastructure.Dto
{
    public  class SelectResult
    {
        public string Name { get; set; }

        public string Label { get; set; }

        public string Value { get; set; }


        public object Prop { get; set; }
    }
}
