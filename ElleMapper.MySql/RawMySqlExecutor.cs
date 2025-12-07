using Microsoft.Data.SqlClient;
using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElleMapper.MySql
{
    public class RawMySqlExecutor : IRawSqlExecutor
    {
        private readonly string _connectionString;

        public RawMySqlExecutor(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> ExecuteRawSqlAsync(string query, Dictionary<string, object>? parameters = null)
        {
            try
            {
                MySqlConnection connection = new(_connectionString);
                await connection.OpenAsync();

                MySqlCommand command = new(query, connection);
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

        public async Task<List<T>> FromSqlAsync<T>(string query, Dictionary<string, object>? parameters = null)
        {
            try
            {
                MySqlConnection connection = new(_connectionString);
                await connection.OpenAsync();

                MySqlCommand command = new(query, connection);
                if (parameters is not null && parameters.Count > 0)
                {
                    foreach (var item in parameters)
                    {
                        command.Parameters.AddWithValue($"{item.Key}", $"{item.Value.ToString()}");
                    }
                }

                MySqlDataAdapter adapter = new(command);
                DataTable dt = new();
                adapter.Fill(dt);

                await connection.CloseAsync();

                string jsonStr = JsonConvert.SerializeObject(dt);
                var lst = JsonConvert.DeserializeObject<List<T>>(jsonStr);

                return lst!;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
