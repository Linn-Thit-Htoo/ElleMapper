using ElleMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElleMapper.SqlServer
{
    public static class SqlServerOptionsBuilderExtensions
    {
        public static DbContextOptionsBuilder UseSqlServer(
            this DbContextOptionsBuilder builder,
            string connectionString)
        {
            return builder.UseProvider(connectionString, new SqlServerProvider(), new RawSqlExecutor(connectionString));
        }
    }
}
