using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElleMapper.Tools
{
    public class MySqlGenerator : IGenerator
    {
        public string GenerateEntityClass(TableMetadata table, string targetNamespace, List<TableRelationMetadata> tableRelations)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using ElleMapper;");
            //sb.AppendLine($"using {targetNamespace};");
            sb.AppendLine();
            sb.AppendLine($"[Table(\"{table.TableName}\")]");
            sb.AppendLine($"public class {table.TableName.Capitalize()}");
            sb.AppendLine("{");

            foreach (var col in table.Columns)
            {
                string cSharpType = MapSqlToCSharp(col.DataType, col.IsNullable);

                if (col.IsPrimaryKey == "1")
                {
                    sb.AppendLine("    [Key]");
                }

                if (col.IsIdentity == "1")
                {
                    sb.AppendLine("    [Identity]");
                }

                sb.AppendLine($"    public {cSharpType} {col.ColumnName} {{ get; set; }}");
            }

            var references = tableRelations.Where(x => x.DependentTable == table.TableName).ToList();
            if (references is not null && references.Count > 0)
            {
                foreach (var reference in references)
                {
                    string principalName = reference.PrincipalTable;
                    sb.AppendLine($"    public virtual {principalName.Capitalize()} {principalName} {{ get; set; }}");
                }
            }

            var collections = tableRelations.Where(r => r.PrincipalTable == table.TableName).ToList();

            if (collections is not null && collections.Count > 0)
            {
                foreach (var collection in collections)
                {
                    string dependentName = collection.DependentTable;
                    string collectionName = dependentName.Pluralize();

                    sb.AppendLine($"    public virtual ICollection<{dependentName.Capitalize()}> {collectionName} {{ get; set; }} = new List<{dependentName.Capitalize()}>();");
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
            //sb.AppendLine($"using {targetNamespace};");
            sb.AppendLine("using ElleMapper;");
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
            string cSharpType = string.Empty;

            cSharpType = sqlType.ToLower() switch
            {
                //"tinyint" when sqlType.Contains("(1)") => "bool",
                "tinyint" => "bool",
                //"tinyint" => "sbyte",
                "smallint" => "short",
                "mediumint" => "int",
                "int" => "int",
                "integer" => "int",
                "bigint" => "long",

                "float" => "float",
                "double" => "double",
                "decimal" => "decimal",
                "numeric" => "decimal",

                "char" => "string",
                "varchar" => "string",
                "tinytext" => "string",
                "text" => "string",
                "mediumtext" => "string",
                "longtext" => "string",

                "date" => "DateTime",
                "datetime" => "DateTime",
                "timestamp" => "DateTime",
                "time" => "TimeSpan",

                "blob" => "byte[]",
                "tinyblob" => "byte[]",
                "mediumblob" => "byte[]",
                "longblob" => "byte[]",
                "binary" => "byte[]",
                "varbinary" => "byte[]",

                "uuid" => "Guid",
                "json" => "string",
                "enum" => "string",
                _ => "object"
            };

            if (isNullable.Trim() != "NO")
            {
                cSharpType += "?";
            }

            return cSharpType;
        }

        public string GenerateEntityClassForViews(TableMetadata view)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using ElleMapper;");
            //sb.AppendLine($"using {targetNamespace};");
            sb.AppendLine();
            sb.AppendLine($"[View(\"{view.TableName}\")]");
            sb.AppendLine($"public class {view.TableName.Capitalize()}");
            sb.AppendLine("{");

            foreach (var col in view.Columns)
            {
                string cSharpType = MapSqlToCSharp(col.DataType, col.IsNullable);

                if (col.IsPrimaryKey == "1")
                {
                    sb.AppendLine("    [Key]");
                }

                if (col.IsIdentity == "1")
                {
                    sb.AppendLine("    [Identity]");
                }

                sb.AppendLine($"    public {cSharpType} {col.ColumnName} {{ get; set; }}");
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
