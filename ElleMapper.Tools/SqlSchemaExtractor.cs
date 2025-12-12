using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElleMapper.Tools
{
    public class SqlSchemaExtractor : ISchemaExtractor
    {
        private readonly string _connectionString;
        private readonly AdoDotNetService _service;

        public SqlSchemaExtractor(string connectionString)
        {
            _connectionString = connectionString;
            _service = new AdoDotNetService(connectionString);
        }

        public async Task<List<TableMetadata>> ExtractSchema()
        {
            try
            {
                var lst = new List<TableMetadata>();
                var tableNames = new List<string>();

                string query = @"SELECT 
    TABLE_NAME
FROM 
    INFORMATION_SCHEMA.TABLES
WHERE 
    TABLE_TYPE = 'BASE TABLE' AND TABLE_CATALOG = DB_NAME();";

                DataTable dt = await _service.QueryDtAsync(query, new List<SqlParameter>());

                if (dt is not null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        tableNames.Add(row["TABLE_NAME"].ToString()!);
                    }
                }

                if (tableNames is not null && tableNames.Count > 0)
                {
                    foreach (var tableName in tableNames)
                    {
                        lst.Add(new TableMetadata
                        {
                            TableName = tableName,
                            Columns = await GetColumnsByTable(tableName)
                        });
                    }
                }

                return lst;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<List<ColumnMetadata>> GetColumnsByTable(string tableName)
        {
            try
            {
                var columnLst = new List<ColumnMetadata>();

                string query = $@"
            SELECT 
                c.name AS COLUMN_NAME, 
                t.name AS DATA_TYPE,
                c.is_nullable AS IS_NULLABLE,
                c.is_identity AS IS_IDENTITY,
                ISNULL(i.is_primary_key, 0) AS IS_PRIMARY_KEY
            FROM 
                sys.columns c
            INNER JOIN 
                sys.types t ON c.user_type_id = t.user_type_id
            LEFT JOIN 
                sys.index_columns ic ON ic.object_id = c.object_id AND ic.column_id = c.column_id
            LEFT JOIN 
                sys.indexes i ON i.object_id = ic.object_id AND i.index_id = ic.index_id AND i.is_primary_key = 1
            WHERE 
                c.object_id = OBJECT_ID(@TableName)
            ORDER BY 
                c.column_id";

                var parameters = new List<SqlParameter>()
            {
                new SqlParameter("@TableName", tableName)
            };

                var lst = await _service.QueryAsync<SqlColumns>(query, parameters);

                if (lst is not null && lst.Count > 0)
                {
                    foreach (var item in lst)
                    {
                        columnLst.Add(new ColumnMetadata
                        {
                            ColumnName = item.COLUMN_NAME,
                            DataType = item.DATA_TYPE,
                            IsIdentity = item.IS_IDENTITY,
                            IsNullable = item.IS_NULLABLE,
                            IsPrimaryKey = item.IS_PRIMARY_KEY
                        });
                    }
                }

                return columnLst;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<TableRelationMetadata>> GetRelationMetadata(string parentTableName)
        {
            try
            {
                string query = @"SELECT
    fk.name AS ConstraintName,
    OBJECT_NAME(fk.parent_object_id) AS DependentTable,
    COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS ForeignColumn,
    OBJECT_NAME(fk.referenced_object_id) AS PrincipalTable,
    COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS PrincipalColumn
FROM 
    sys.foreign_keys AS fk
INNER JOIN 
    sys.foreign_key_columns AS fkc 
    ON fk.object_id = fkc.constraint_object_id
WHERE
     OBJECT_NAME(fk.referenced_object_id) = @TableName
     OR OBJECT_NAME(fk.parent_object_id) = @TableName
ORDER BY 
    DependentTable, ConstraintName;";

                var parameters = new List<SqlParameter>()
                {
                    new SqlParameter("@TableName", parentTableName)
                };

                var relations = new List<TableRelationMetadata>();
                var lst = await _service.QueryAsync<SqlTableRelations>(query, parameters);

                if (lst is not null && lst.Count > 0)
                {
                    foreach (var item in lst)
                    {
                        relations.Add(new TableRelationMetadata
                        {
                            ConstraintName = item.ConstraintName,
                            DependentTable = item.DependentTable,
                            ForeignColumn = item.ForeignColumn,
                            PrincipalTable = item.PrincipalTable,
                            PrincipalColumn = item.PrincipalColumn
                        });
                    }
                }

                return relations;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<TableMetadata>> ExtractViews()
        {
            try
            {
                var lst = new List<TableMetadata>();
                var tableNames = new List<string>();

                string query = @"SELECT 
    TABLE_NAME
FROM 
    INFORMATION_SCHEMA.TABLES
WHERE 
    TABLE_CATALOG = DB_NAME()
    AND TABLE_TYPE = 'VIEW';";

                DataTable dt = await _service.QueryDtAsync(query, new List<SqlParameter>());

                if (dt is not null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        tableNames.Add(row["TABLE_NAME"].ToString()!);
                    }
                }

                if (tableNames is not null && tableNames.Count > 0)
                {
                    foreach (var tableName in tableNames)
                    {
                        lst.Add(new TableMetadata
                        {
                            TableName = tableName,
                            Columns = await GetColumnsByTable(tableName)
                        });
                    }
                }

                return lst;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
