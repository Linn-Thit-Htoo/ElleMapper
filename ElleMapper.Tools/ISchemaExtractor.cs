using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElleMapper.Tools
{
    public interface ISchemaExtractor
    {
        Task<List<TableMetadata>> ExtractSchema();
        Task<List<TableRelationMetadata>> GetRelationMetadata(string parentTableName);
    }
}
