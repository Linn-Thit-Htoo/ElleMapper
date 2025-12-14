using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElleMapper
{
    public interface IRawSqlExecutor
    {
        Task<int> ExecuteRawSqlAsync(string query, Dictionary<string, object>? parameters = null, CancellationToken cs = default);
        Task<T> FromSqlAsync<T>(string storedProcedureName, Dictionary<string, object>? parameters = null, CancellationToken cs = default);
    }
}
