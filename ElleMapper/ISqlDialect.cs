using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElleMapper
{
    public interface ISqlDialect
    {
        public string ParameterPrefix { get; }
        public string QuoteIdentifier(string name);
        public string IdentitySelect();
        public string LimitOffset(int? limit, int? offset);
    }
}
