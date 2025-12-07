using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElleMapper
{
    public class DbContextOptions
    {
        public string ConnectionString { get; internal set; }
        public IDatabaseProvider DatabaseProvider { get; internal set; }
        public IRawSqlExecutor RawSqlExecutor { get; internal set; }
    }
}
