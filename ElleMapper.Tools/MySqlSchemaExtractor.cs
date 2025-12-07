using Microsoft.Data.SqlClient;
using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElleMapper.Tools
{
    public class MySqlSchemaExtractor : ISchemaExtractor
    {
        private readonly string _connectionString;
        private readonly AdoDotNetService _service;

        public MySqlSchemaExtractor(string connectionString)
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

                string query = @"SELECT table_name FROM information_schema.tables WHERE table_schema = DATABASE() AND table_type = 'BASE TABLE';";

                DataTable dt = await _service.QueryDtAsync(query, new List<MySqlParameter>());

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
                            Columns = await GetColumnsByTableName(tableName)
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

        public async Task<List<TableRelationMetadata>> GetRelationMetadata(string parentTableName)
        {
            try
            {
                string query = @"SELECT
    RC.CONSTRAINT_NAME AS ConstraintName,
    RC.TABLE_NAME AS DependentTable,
    KCU.COLUMN_NAME AS ForeignColumn,
    RC.REFERENCED_TABLE_NAME AS PrincipalTable,
    KCU.REFERENCED_COLUMN_NAME AS PrincipalColumn
FROM 
    INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS AS RC
INNER JOIN 
    INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KCU
    ON RC.CONSTRAINT_SCHEMA = KCU.CONSTRAINT_SCHEMA 
    AND RC.CONSTRAINT_NAME = KCU.CONSTRAINT_NAME
WHERE
    RC.CONSTRAINT_SCHEMA = DATABASE()
    AND ( 
        RC.REFERENCED_TABLE_NAME = 'tbl_author' 
        OR RC.TABLE_NAME = @TableName
    ) 
ORDER BY
    RC.TABLE_NAME, RC.CONSTRAINT_NAME 
LIMIT 0, 1000;";

                var parameters = new List<MySqlParameter>()
                {
                    new MySqlParameter("@TableName", parentTableName)
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

        private async Task<List<ColumnMetadata>> GetColumnsByTableName(string tableName)
        {
            try
            {
                var columns = new List<ColumnMetadata>();
                string query = @"SELECT 
    C.COLUMN_NAME,  
    C.DATA_TYPE,    
    C.IS_NULLABLE,  
    -- Check the EXTRA column for 'auto_increment' status
    CASE WHEN C.EXTRA LIKE '%auto_increment%' THEN 1 ELSE 0 END AS IS_IDENTITY,
    (
        SELECT 1 FROM information_schema.KEY_COLUMN_USAGE AS KCU
        WHERE 
            KCU.TABLE_SCHEMA = DATABASE() 
            AND KCU.TABLE_NAME = C.TABLE_NAME  
            AND KCU.COLUMN_NAME = C.COLUMN_NAME
            AND KCU.CONSTRAINT_NAME = 'PRIMARY'
        LIMIT 1
    ) AS IS_PRIMARY_KEY
FROM 
    information_schema.COLUMNS AS C
WHERE 
    C.TABLE_SCHEMA = DATABASE() 
    AND C.TABLE_NAME = @TableName
ORDER BY 
    C.ORDINAL_POSITION";

                var parameters = new List<MySqlParameter>()
                {
                    new MySqlParameter("@TableName", tableName)
                };

                var lst = await _service.QueryAsync<SqlColumns>(query, parameters);

                if (lst is not null && lst.Count > 0)
                {
                    foreach (var item in lst)
                    {
                        columns.Add(new ColumnMetadata
                        {
                            ColumnName = item.COLUMN_NAME,
                            DataType = item.DATA_TYPE,
                            IsIdentity = item.IS_IDENTITY,
                            IsNullable = item.IS_NULLABLE,
                            IsPrimaryKey = item.IS_PRIMARY_KEY
                        });
                    }
                }

                return columns;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
