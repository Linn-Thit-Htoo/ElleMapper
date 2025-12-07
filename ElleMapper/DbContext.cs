using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens.Experimental;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ElleMapper
{
    public class DbContext
    {
        internal readonly ChangeTracker ChangeTracker;
        internal readonly DbContextOptions _options;
        internal readonly DbConnectionFactory _connectionFactory;
        internal readonly MetadataProvider MetadataProvider = new();

        public DbContext(DbContextOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _connectionFactory = new DbConnectionFactory(options.DatabaseProvider, options.ConnectionString);
            ChangeTracker = new ChangeTracker();
            InitializeDbSets();
        }

        private void InitializeDbSets()
        {
            try
            {
                var props = this.GetType()
                                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                .Where(p => p.PropertyType.IsGenericType
                                            && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

                foreach (var prop in props)
                {
                    var entityType = prop.PropertyType.GetGenericArguments()[0];
                    var dbSetType = typeof(DbSet<>).MakeGenericType(entityType);
                    var ctor = dbSetType.GetConstructor(new Type[] { typeof(DbContext) }) ?? throw new ArgumentNullException(nameof(DbContext));
                    var instance = ctor.Invoke(new object[] { this });

                    prop.SetValue(this, instance);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public virtual void SaveChanges()
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                connection.Open();
                var transaction = connection.BeginTransaction();

                try
                {
                    var entries = ChangeTracker.Entries();
                    foreach (var entry in entries)
                    {
                        var entity = entry.Key;
                        var state = entry.Value;

                        if (state == EntityState.Added)
                        {
                            var dialect = _options.DatabaseProvider.Dialect;
                            var data = MetadataProvider.GetEntityType(entity.GetType());
                            var nonKeys = data.Properties.Where(x => !x.IsIdentity).ToList();
                            var columns = string.Join(",", nonKeys.Select(x => dialect.QuoteIdentifier(x.ColumnName)));
                            var parameters = string.Join(",", nonKeys.Select((x, i) => $"@p{i}"));
                            string query = $"INSERT INTO {dialect.QuoteIdentifier(data.TableName)} ({columns}) VALUES ({parameters})";

                            // INSERT INTO [Tbl_Blog] ([BlogTitle],[BlogAuthor],[BlogContent],[CreatedAt],[UpdatedAt],[DeletedAt],[IsDeleted]) VALUES (@p0,@p1,@p2,@p3,@p4,@p5,@p6)

                            using var command = connection.CreateCommand();
                            command.CommandText = query;
                            command.Transaction = transaction;

                            for (int i = 0; i < nonKeys.Count; i++)
                            {
                                var property = nonKeys[i];
                                var value = property.ClrProperty.GetValue(entity) ?? DBNull.Value;

                                var parameter = command.CreateParameter();
                                parameter.ParameterName = $"@p{i}";
                                parameter.Value = value;

                                command.Parameters.Add(parameter);
                            }

                            command.ExecuteNonQuery();
                        }

                        else if (state == EntityState.Modified)
                        {
                            var dialect = _options.DatabaseProvider.Dialect;
                            var data = MetadataProvider.GetEntityType(entity.GetType());
                            var nonKeys = data.Properties.Where(x => !x.IsIdentity).ToList();
                            var key = data.KeyProperty;
                            var set = string.Join(", ", nonKeys.Select((x, i) => $"{dialect.QuoteIdentifier(x.ColumnName)} = @p{i}"));

                            string query = $"UPDATE {dialect.QuoteIdentifier(data.TableName)} SET {set} WHERE {dialect.QuoteIdentifier(data.KeyProperty.ColumnName)} = @{key.ColumnName}";

                            using var command = connection.CreateCommand();
                            command.CommandText = query;
                            command.Transaction = transaction;

                            var keyValue = key.ClrProperty.GetValue(entity) ?? DBNull.Value;
                            var keyParam = command.CreateParameter();
                            keyParam.ParameterName = $"@{key.ColumnName}";
                            keyParam.Value = keyValue;
                            command.Parameters.Add(keyParam);

                            for (int i = 0; i < nonKeys.Count; i++)
                            {
                                var property = nonKeys[i];
                                var value = property.ClrProperty.GetValue(entity) ?? DBNull.Value;

                                var parameter = command.CreateParameter();
                                parameter.ParameterName = $"@p{i}";
                                parameter.Value = value;

                                command.Parameters.Add(parameter);
                            }

                            command.ExecuteNonQuery();
                        }

                        else if (state == EntityState.Deleted)
                        {
                            var dialect = _options.DatabaseProvider.Dialect;
                            var data = MetadataProvider.GetEntityType(entity.GetType());
                            var key = data.KeyProperty;
                            string query = $"DELETE FROM {data.TableName} WHERE {dialect.QuoteIdentifier(key.ColumnName)} = @{key.ColumnName}";

                            var command = connection.CreateCommand();
                            command.CommandText = query;
                            command.Transaction = transaction;

                            var parameter = command.CreateParameter();
                            parameter.ParameterName = $"@{key.ColumnName}";
                            parameter.Value = key.ClrProperty.GetValue(entity) ?? DBNull.Value;
                            command.Parameters.Add(parameter);

                            command.ExecuteNonQuery();
                        }

                        ChangeTracker.Detach(entity);
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async virtual Task SaveChangesAsync(CancellationToken cs = default)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync(cs);
                var transaction = connection.BeginTransaction();

                try
                {
                    var entries = ChangeTracker.Entries();
                    foreach (var entry in entries)
                    {
                        var entity = entry.Key;
                        var state = entry.Value;

                        if (state == EntityState.Added)
                        {
                            var dialect = _options.DatabaseProvider.Dialect;
                            var data = MetadataProvider.GetEntityType(entity.GetType());
                            var nonKeys = data.Properties.Where(x => !x.IsIdentity).ToList();
                            var columns = string.Join(",", nonKeys.Select(x => dialect.QuoteIdentifier(x.ColumnName)));
                            var parameters = string.Join(",", nonKeys.Select((x, i) => $"@p{i}"));
                            string query = $"INSERT INTO {dialect.QuoteIdentifier(data.TableName)} ({columns}) VALUES ({parameters})";

                            // INSERT INTO [Tbl_Blog] ([BlogTitle],[BlogAuthor],[BlogContent],[CreatedAt],[UpdatedAt],[DeletedAt],[IsDeleted]) VALUES (@p0,@p1,@p2,@p3,@p4,@p5,@p6)

                            using var command = connection.CreateCommand();
                            command.CommandText = query;
                            command.Transaction = transaction;

                            for (int i = 0; i < nonKeys.Count; i++)
                            {
                                var property = nonKeys[i];
                                var value = property.ClrProperty.GetValue(entity) ?? DBNull.Value;

                                var parameter = command.CreateParameter();
                                parameter.ParameterName = $"@p{i}";
                                parameter.Value = value;

                                command.Parameters.Add(parameter);
                            }

                            await command.ExecuteNonQueryAsync(cs);
                        }

                        else if (state == EntityState.Modified)
                        {
                            var dialect = _options.DatabaseProvider.Dialect;
                            var data = MetadataProvider.GetEntityType(entity.GetType());
                            var nonKeys = data.Properties.Where(x => !x.IsIdentity).ToList();
                            var key = data.KeyProperty;
                            var set = string.Join(", ", nonKeys.Select((x, i) => $"{dialect.QuoteIdentifier(x.ColumnName)} = @p{i}"));

                            string query = $"UPDATE {dialect.QuoteIdentifier(data.TableName)} SET {set} WHERE {dialect.QuoteIdentifier(data.KeyProperty.ColumnName)} = @{key.ColumnName}";

                            using var command = connection.CreateCommand();
                            command.CommandText = query;
                            command.Transaction = transaction;

                            var keyValue = key.ClrProperty.GetValue(entity) ?? DBNull.Value;
                            var keyParam = command.CreateParameter();
                            keyParam.ParameterName = $"@{key.ColumnName}";
                            keyParam.Value = keyValue;
                            command.Parameters.Add(keyParam);

                            for (int i = 0; i < nonKeys.Count; i++)
                            {
                                var property = nonKeys[i];
                                var value = property.ClrProperty.GetValue(entity) ?? DBNull.Value;

                                var parameter = command.CreateParameter();
                                parameter.ParameterName = $"@p{i}";
                                parameter.Value = value;

                                command.Parameters.Add(parameter);
                            }

                            await command.ExecuteNonQueryAsync(cs);
                        }

                        else if (state == EntityState.Deleted)
                        {
                            var dialect = _options.DatabaseProvider.Dialect;
                            var data = MetadataProvider.GetEntityType(entity.GetType());
                            var key = data.KeyProperty;
                            string query = $"DELETE FROM {data.TableName} WHERE {dialect.QuoteIdentifier(key.ColumnName)} = @{key.ColumnName}";

                            var command = connection.CreateCommand();
                            command.CommandText = query;
                            command.Transaction = transaction;

                            var parameter = command.CreateParameter();
                            parameter.ParameterName = $"@{key.ColumnName}";
                            parameter.Value = key.ClrProperty.GetValue(entity) ?? DBNull.Value;
                            command.Parameters.Add(parameter);

                            await command.ExecuteNonQueryAsync(cs);
                        }

                        ChangeTracker.Detach(entity);
                    }

                    await transaction.CommitAsync(cs);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cs);
                    throw;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<int> ExecuteRawSqlAsync(string query, Dictionary<string, object>? parameters = null)
        {
            try
            {
                var rawSqlExecutor = _options.RawSqlExecutor;
                return await rawSqlExecutor.ExecuteRawSqlAsync(query, parameters);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<T>> FromSql<T>(string query, Dictionary<string, object>? parameters = null)
        {
            try
            {
                var rawSqlExecutor = _options.RawSqlExecutor;
                var lst = await rawSqlExecutor.FromSqlAsync<T>(query, parameters);

                return lst;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
