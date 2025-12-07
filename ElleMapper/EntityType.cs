using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElleMapper
{
    public class EntityType
    {
        public Type ClrType { get; set; }
        public string TableName { get; set; }
        public Property KeyProperty { get; set; }
        public List<Property> Properties { get; set; } = new();

        public string GetColumnName(string propertyName)
        {
            var property = Properties.FirstOrDefault(x => x.PropertyName == propertyName) ?? throw new ArgumentNullException($"Property {propertyName} is null.");

            return property.ColumnName;
        }
    }
}
