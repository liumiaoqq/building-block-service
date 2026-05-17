using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using YouJu.Infrastructure;

namespace Web
{ 
    public static  class DbOptions
    {
        
        public static Type[] GetTables
        {
            get
            {
                var all = AssemblyExtension.AllAssemblies;
                var table = all.SelectMany(l => l.DefinedTypes.Where(x => x.BaseType == typeof(CreationAuditedAggregateRoot))).ToList();
                return table.Select(x => x.AsType()).ToArray();
            }
        }

      
    }
}
