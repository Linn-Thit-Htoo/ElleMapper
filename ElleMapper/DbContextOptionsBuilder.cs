using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace ElleMapper
{
    public class DbContextOptionsBuilder
    {
        private readonly DbContextOptions _options = new();

        public DbContextOptionsBuilder UseProvider(string connectionString, IDatabaseProvider databaseProvider, IRawSqlExecutor rawSqlExecutor)
        {
            _options.ConnectionString = connectionString;
            _options.DatabaseProvider = databaseProvider;
            _options.RawSqlExecutor = rawSqlExecutor;

            return this;
        }

        public DbContextOptions Build() => _options;
    }
}
