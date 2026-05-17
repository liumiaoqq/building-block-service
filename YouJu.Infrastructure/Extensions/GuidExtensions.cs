using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouJu.Infrastructure
{
    public static class GuidExtensions
    {
        public static bool CheckGuidIsNullOrWhiteSpace(this Guid? value)
        {
            if (!value.HasValue)
                return true;
            if (value.Value == Guid.Empty)
                return true;
            return false;
        }
        public static bool CheckGuidIsNullOrWhiteSpace(this Guid value)
        {
            if (value == Guid.Empty)
                return true;
            return false;
        }
        public static string GuidNullToString(this long? value)
        {

            return value.HasValue ? value.Value.ToString() : "";

        }
        public static void GuidNullToEmpty<T>(this T value)
        {
            var list = typeof(T).GetProperties().Where(x => x.PropertyType == typeof(Guid?)).ToList();
            foreach (var item in list)
            {
                if (item.GetValue(value) == null)
                {
                    item.SetValue(value, Guid.Empty);
                }

            }

        }
    }
}
