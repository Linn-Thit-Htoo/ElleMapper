using ElleMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElleMapper.MySql
{
    public class MySqlDialect : ISqlDialect
    {
        public string ParameterPrefix => "@";

        public string IdentitySelect() => "SELECT LAST_INSERT_ID();";

        public string LimitOffset(int? limit, int? offset) => $"LIMIT {limit} OFFSET {offset}";

        public string QuoteIdentifier(string name) => $"`{name}`";
    }
}
