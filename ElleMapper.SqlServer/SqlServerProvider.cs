using ElleMapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElleMapper.SqlServer
{
    public class SqlServerProvider : IDatabaseProvider
    {
        public DbProviderFactory ProviderFactory => SqlClientFactory.Instance;

        public ISqlDialect Dialect { get; } = new SqlServerDialect();
    }
}
