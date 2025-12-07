using ElleMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElleMapper.SqlServer
{
    public class SqlServerDialect : ISqlDialect
    {
        public string ParameterPrefix => "@";

        public string IdentitySelect() => "SELECT SCOPE_IDENTITY();";

        public string LimitOffset(int? limit, int? offset) => $"OFFSET {offset ?? 0} ROWS FETCH NEXT {limit} ROWS ONLY";

        public string QuoteIdentifier(string name) => $"[{name}]";
    }
}
