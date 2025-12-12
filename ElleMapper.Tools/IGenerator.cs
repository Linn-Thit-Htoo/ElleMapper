using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElleMapper.Tools
{
    public interface IGenerator
    {
        string GenerateEntityClass(TableMetadata table, string targetNamespace, List<TableRelationMetadata> tableRelations);
        string GenerateEntityClassForViews(TableMetadata view);
        string GenerateDbContext(List<TableMetadata> tables, string targetNamespace, string contextName);
    }
}
