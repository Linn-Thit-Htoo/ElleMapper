using ElleMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElleMapper.MySql
{
    public static class MySqlOptionBuilderExtensions
    {
        public static DbContextOptionsBuilder UseMySql(this DbContextOptionsBuilder builder, string connectionString)
        {
            return builder.UseProvider(connectionString, new MySqlProvider(), new RawMySqlExecutor(connectionString));
        }
    }
}
