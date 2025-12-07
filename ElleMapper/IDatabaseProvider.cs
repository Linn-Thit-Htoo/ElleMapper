using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElleMapper
{
    public interface IDatabaseProvider
    {
        public DbProviderFactory ProviderFactory { get; }
        public ISqlDialect Dialect { get; }
    }
}
