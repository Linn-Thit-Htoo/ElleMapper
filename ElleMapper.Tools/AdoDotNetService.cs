using Microsoft.Data.SqlClient;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElleMapper.Tools
{
    public class AdoDotNetService
    {
        private readonly string _connectionString;

        public AdoDotNetService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<T>> QueryAsync<T>(string query, List<SqlParameter>? parameters = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                SqlConnection connection = new(_connectionString);
                await connection.OpenAsync();

                SqlCommand command = new(query, connection);
                command.CommandType = commandType;
                if (parameters is not null)
                {
                    command.Parameters.AddRange(parameters.ToArray());
                }

                SqlDataAdapter adapter = new(command);
                DataTable dt = new();
                adapter.Fill(dt);

                await connection.CloseAsync();

                if (dt is not null && dt.Rows.Count > 0)
                {
                    string jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                    var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(jsonStr);

                    return lst!;
                }

                return new List<T>();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<T>> QueryAsync<T>(string query, List<MySqlParameter>? parameters = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                MySqlConnection connection = new(_connectionString);
                await connection.OpenAsync();

                MySqlCommand command = new(query, connection);
                command.CommandType = commandType;
                if (parameters is not null)
                {
                    command.Parameters.AddRange(parameters.ToArray());
                }

                MySqlDataAdapter adapter = new(command);
                DataTable dt = new();
                adapter.Fill(dt);

                await connection.CloseAsync();

                if (dt is not null && dt.Rows.Count > 0)
                {
                    string jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                    var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(jsonStr);

                    return lst!;
                }

                return new List<T>();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<DataTable> QueryDtAsync(string query, List<SqlParameter>? parameters = null, CommandType commandType = CommandType.Text)
        {
            SqlConnection connection = new(_connectionString);
            await connection.OpenAsync();

            SqlCommand command = new(query, connection);
            command.CommandType = commandType;
            if (parameters is not null)
            {
                command.Parameters.AddRange(parameters.ToArray());
            }

            SqlDataAdapter adapter = new(command);
            DataTable dt = new();
            adapter.Fill(dt);

            await connection.CloseAsync();

            return dt;
        }

        public async Task<DataTable> QueryDtAsync(string query, List<MySqlParameter>? parameters = null, CommandType commandType = CommandType.Text)
        {
            MySqlConnection connection = new(_connectionString);
            await connection.OpenAsync();

            MySqlCommand command = new(query, connection);
            command.CommandType = commandType;
            if (parameters is not null)
            {
                command.Parameters.AddRange(parameters.ToArray());
            }

            MySqlDataAdapter adapter = new(command);
            DataTable dt = new();
            adapter.Fill(dt);

            await connection.CloseAsync();

            return dt;
        }
    }
}
