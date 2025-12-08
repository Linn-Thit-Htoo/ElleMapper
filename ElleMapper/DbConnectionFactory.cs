using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElleMapper
{
    public class DbConnectionFactory
    {
        private readonly IDatabaseProvider _provider;
        private readonly string _connectionString;

        public DbConnectionFactory(IDatabaseProvider provider, string connectionString)
        {
            _provider = provider;
            _connectionString = connectionString;
        }

        public DbConnection CreateConnection()
        {
            try
            {
                var connection = _provider.ProviderFactory.CreateConnection() ?? throw new ArgumentNullException("Db Connection is null.");
                connection.ConnectionString = _connectionString;

                return connection;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DbProviderFactory GetDbProviderFactory() => _provider.ProviderFactory;
    }
}
