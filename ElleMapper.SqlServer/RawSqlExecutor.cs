using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElleMapper.SqlServer
{
    public class RawSqlExecutor : IRawSqlExecutor
    {
        private readonly string _connectionString;

        public RawSqlExecutor(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> ExecuteRawSqlAsync(string query, Dictionary<string, object>? parameters = null)
        {
            try
            {
                if (string.IsNullOrEmpty(query))
                {
                    throw new ArgumentNullException("Query cannot be empty.");
                }

                SqlConnection connection = new(_connectionString);
                await connection.OpenAsync();

                SqlCommand command = new(query, connection);
                if (parameters is not null && parameters.Count > 0)
                {
                    foreach (var item in parameters)
                    {
                        command.Parameters.AddWithValue($"{item.Key}", $"{item.Value.ToString()}");
                    }
                }

                int result = await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<T> FromSqlAsync<T>(string storedProcedureName, Dictionary<string, object>? parameters = null)
        {
            try
            {
                if (string.IsNullOrEmpty(storedProcedureName))
                {
                    throw new ArgumentNullException("Stored Procedure Name cannot be empty.");
                }

                SqlConnection connection = new(_connectionString);
                await connection.OpenAsync();

                SqlCommand command = new(storedProcedureName, connection);
                command.CommandType = CommandType.StoredProcedure;
                if (parameters is not null && parameters.Count > 0)
                {
                    foreach (var item in parameters)
                    {
                        command.Parameters.AddWithValue($"{item.Key}", $"{item.Value.ToString()}");
                    }
                }

                SqlDataAdapter adapter = new(command);
                DataTable dt = new();
                adapter.Fill(dt);

                await connection.CloseAsync();

                string jsonStr = JsonConvert.SerializeObject(dt);
                var lst = JsonConvert.DeserializeObject<T>(jsonStr);

                return lst!;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
