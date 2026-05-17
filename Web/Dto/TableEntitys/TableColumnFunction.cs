using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Dto.TableEntitys
{
    public  class TableColumnFunction
    {
        public string Name { get; set; }

        public string Label { get; set; }

        public string Value { get; set; }

        public List<TableNavigateRelativeDto> TableNavigateRelativeDtos { get; set; }
        public object Prop { get; set; }
    }
}
