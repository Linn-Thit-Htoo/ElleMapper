using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElleMapper.Tools
{
    public class TableMetadata
    {
        public string TableName { get; set; }
        public List<ColumnMetadata> Columns { get; set; } = new();
    }

    public class ColumnMetadata
    {
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public string IsNullable { get; set; }
        public string IsPrimaryKey { get; set; }
        public string IsIdentity { get; set; }
    }

    public class TableRelationMetadata
    {
        public string ConstraintName { get; set; }
        public string PrincipalTable { get; set; }
        public string DependentTable { get; set; }
        public string ForeignColumn { get; set; }
        public string PrincipalColumn { get; set; }
    }
}
