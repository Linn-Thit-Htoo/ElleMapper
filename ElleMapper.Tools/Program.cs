using ElleMapper.Tools;
using System.Diagnostics;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main(string[] args)
    {
        //if (!args[0].Equals("scaffold", StringComparison.OrdinalIgnoreCase))
        //{
        //    Console.WriteLine("Invalid scaffold command.");
        //    return;
        //}

        string connectionString = args[2];
        string providerName = args[3];

        if (string.IsNullOrEmpty(providerName))
        {
            Console.WriteLine("Database Provider cannot be empty.");
            return;
        }

        string defaultOuputDir = "Models";
        string defaultDbContext = "AppDbContext";
        var generator = GetGenerator(providerName);
        var extractor = GetSchemaExtractor(providerName, connectionString);

        if (string.IsNullOrEmpty(connectionString))
        {
            Console.WriteLine("ConnectionString cannot be empty.");
            return;
        }

        // catch output and dbcontext param
        for (int i = 4; i < args.Length; i++)
        {
            if (args[i].Equals("-o", StringComparison.OrdinalIgnoreCase))
            {
                defaultOuputDir = args[i + 1];
                i++; // need to skip
            }

            if (args[i].Equals("-c", StringComparison.OrdinalIgnoreCase))
            {
                defaultDbContext = args[i + 1];
                i++;
            }
        }

        string targetNamespace = Path.GetFileName(defaultOuputDir);
        string dir = Path.Combine(Environment.CurrentDirectory, defaultOuputDir);
        Directory.CreateDirectory(dir);

        var tables = await extractor.ExtractSchema();
        var views = await extractor.ExtractViews();

        if (tables is null || tables.Count == 0)
        {
            Console.WriteLine("No tables found.");
            return;
        }

        // create entities for tables
        foreach (var table in tables)
        {
            var relations = await extractor.GetRelationMetadata(table.TableName);
            var entityClassOutput = generator.GenerateEntityClass(table, targetNamespace, relations);
            table.TableName = table.TableName.Capitalize();
            string entityPath = Path.Combine(defaultOuputDir, $"{table.TableName}.cs");
            await File.WriteAllTextAsync(entityPath, entityClassOutput);
        }

        // create entities for views
        if (views is not null && views.Count > 0)
        {
            foreach (var view in views)
            {
                var entityClassOutput = generator.GenerateEntityClassForViews(view);
                view.TableName = view.TableName.Capitalize();
                string entityPath = Path.Combine(defaultOuputDir, $"{view.TableName}.cs");
                await File.WriteAllTextAsync(entityPath, entityClassOutput);
            }
        }

        // create db context file
        var contextOutput = generator.GenerateDbContext(tables, targetNamespace, defaultDbContext);
        string contextPath = Path.Combine(defaultOuputDir, $"{defaultDbContext}.cs");
        await File.WriteAllTextAsync(contextPath, contextOutput);

        Console.WriteLine("Scaffolding Success.");
    }

    private static IGenerator GetGenerator(string providerName)
    {
        if (!string.IsNullOrEmpty(providerName) && providerName.Trim().EndsWith(".SqlServer", StringComparison.OrdinalIgnoreCase))
        {
            return new SqlGenerator();
        }

        if (!string.IsNullOrEmpty(providerName) && providerName.Trim().EndsWith(".Mysql", StringComparison.OrdinalIgnoreCase))
        {
            return new MySqlGenerator();
        }

        throw new NotSupportedException("Invalid Provider Name.");
    }

    private static ISchemaExtractor GetSchemaExtractor(string providerName, string connectionString)
    {
        if (!string.IsNullOrEmpty(providerName) && providerName.Trim().EndsWith(".SqlServer", StringComparison.OrdinalIgnoreCase))
        {
            return new SqlSchemaExtractor(connectionString);
        }

        if (!string.IsNullOrEmpty(providerName) && providerName.Trim().EndsWith(".Mysql", StringComparison.OrdinalIgnoreCase))
        {
            return new MySqlSchemaExtractor(connectionString);
        }

        throw new NotSupportedException("Invalid Provider Name.");
    }
}