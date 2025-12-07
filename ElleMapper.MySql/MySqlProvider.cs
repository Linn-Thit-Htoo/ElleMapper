using ElleMapper;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElleMapper.MySql
{
    public class MySqlProvider : IDatabaseProvider
    {
        public DbProviderFactory ProviderFactory => MySqlConnectorFactory.Instance;

        public ISqlDialect Dialect => new MySqlDialect();
    }
}
