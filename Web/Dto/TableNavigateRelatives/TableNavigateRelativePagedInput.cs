using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Dto.TableNavigateRelatives
{
    public  class TableNavigateRelativePagedInput : PagedBaseInput
    {
        public TableNavigateType? TableNavigateType { get; set; }

        public Guid? RelativeTableId { get; set; }
        /// <summary>
        /// 表A
        /// </summary>
        public Guid? AssociationATableId { get; set; }

        /// <summary>
        /// 表A列Id
        /// </summary>
        public Guid? AssociationAColumnId { get; set; }
    }
}
