using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElleMapper.Tools
{
    public class SqlTableRelations
    {
        public string ConstraintName { get; set; }
        public string DependentTable { get; set; }
        public string ForeignColumn { get; set; }
        public string PrincipalTable { get; set; }
        public string PrincipalColumn { get; set; }
    }
}
