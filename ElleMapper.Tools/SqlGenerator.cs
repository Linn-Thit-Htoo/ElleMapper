using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElleMapper.Tools
{
    public class SqlGenerator : IGenerator
    {
        public string GenerateEntityClass(TableMetadata table, string targetNamespace, List<TableRelationMetadata> tableRelations)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using ElleMapper;");
            //sb.AppendLine($"using {targetNamespace};");
            sb.AppendLine();
            sb.AppendLine($"public class {table.TableName.Capitalize()}");
            sb.AppendLine("{");

            foreach (var col in table.Columns)
            {
                string cSharpType = MapSqlToCSharp(col.DataType, col.IsNullable);
                sb.AppendLine($"    public {cSharpType} {col.ColumnName} {{ get; set; }}");
            }

            var references = tableRelations.Where(x => x.DependentTable == table.TableName).ToList();
            if (references is not null && references.Count > 0)
            {
                foreach (var reference in references)
                {
                    string principalName = reference.PrincipalTable;
                    sb.AppendLine($"    public virtual {principalName} {principalName} {{ get; set; }}");
                }
            }

            var collections = tableRelations.Where(r => r.PrincipalTable == table.TableName).ToList();

            if (collections is not null && collections.Count > 0)
            {
                foreach (var collection in collections)
                {
                    string dependentName = collection.DependentTable;
                    string collectionName = dependentName.Pluralize();

                    sb.AppendLine($"    public virtual ICollection<{dependentName}> {collectionName} {{ get; set; }} = new List<{dependentName}>();");
                }
            }

            sb.AppendLine("}");

            return sb.ToString();
        }

        public string GenerateDbContext(List<TableMetadata> tables, string targetNamespace, string contextName)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using ElleMapper;");
            //sb.AppendLine($"using {targetNamespace};");
            sb.AppendLine();
            sb.AppendLine($"public class {contextName} : DbContext");
            sb.AppendLine("{");

            // Constructor that passes options up to the base DbContext
            sb.AppendLine($"    public {contextName}(DbContextOptions options) : base(options) {{ }}");
            sb.AppendLine();

            // Generate DbSet properties
            foreach (var table in tables)
            {
                // Use pluralization convention for DbSet properties
                string dbSetName = table.TableName.EndsWith("y") ? table.TableName.Substring(0, table.TableName.Length - 1) + "ies" : table.TableName + "s";

                sb.AppendLine($"    public DbSet<{table.TableName}> {dbSetName} {{ get; set; }}");
            }

            sb.AppendLine("}");
            return sb.ToString();
        }

        private string MapSqlToCSharp(string sqlType, string isNullable)
        {
            string cSharpType = sqlType.ToLower() switch
            {
                "bigint" => "long",
                "int" => "int",
                "smallint" => "short",
                "tinyint" => "byte",
                "bit" => "bool",
                "datetime" => "DateTime",
                "datetime2" => "DateTime",
                "date" => "DateTime",
                "uniqueidentifier" => "Guid",
                "decimal" => "decimal",
                "float" => "double",
                "real" => "float",
                "varchar" => "string",
                "nvarchar" => "string",
                "char" => "string",
                "nchar" => "string",
                "text" => "string",
                "ntext" => "string",
                _ => "object"
            };

            if (Convert.ToBoolean(isNullable))
            {
                cSharpType += "?";
            }

            return cSharpType;
        }
    }
}
