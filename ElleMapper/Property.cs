using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElleMapper
{
    public class Property
    {
        public string ColumnName { get; set; }
        public string PropertyName { get; set; }
        public Type PropertyType { get; set; }
        public bool IsKey { get; set; }
        public bool IsIdentity { get; set; }
        public System.Reflection.PropertyInfo ClrProperty { get; set; }
        public void SetValue(object? entity, object value)
        {
            ClrProperty.SetValue(entity, value);
        }
    }
}
