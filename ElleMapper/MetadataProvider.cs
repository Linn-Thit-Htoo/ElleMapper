using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ElleMapper
{
    public class MetadataProvider
    {
        private static readonly ConcurrentDictionary<Type, EntityType> _cache = new();

        public EntityType GetEntityType(Type clrType)
        {
            return _cache.GetOrAdd(clrType, BuildEntityType(clrType));
        }

        public EntityType BuildEntityType(Type type)
        {
            try
            {
                EntityType entityType = new();
                entityType.ClrType = type;

                var tableAttribute = type.GetCustomAttribute<TableAttribute>();
                entityType.TableName = tableAttribute is not null ? tableAttribute.Name : type.Name;

                var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (var prop in props)
                {
                    var ignoreAttribute = prop.GetCustomAttribute<IgnoreAttribute>();
                    if (ignoreAttribute is not null)
                    {
                        continue;
                    }

                    var property = new Property()
                    {
                        ClrProperty = prop,
                        PropertyName = prop.Name,
                        PropertyType = prop.PropertyType,
                    };

                    var columnAttribute = prop.GetCustomAttribute<ColumnAttribute>();
                    property.ColumnName = columnAttribute is not null ? columnAttribute.Name : prop.Name;

                    var keyAttribute = prop.GetCustomAttribute<KeyAttribute>();
                    if (keyAttribute is not null)
                    {
                        if (entityType.KeyProperty is not null)
                        {
                            throw new Exception("Can only contain one primary key in a table.");
                        }

                        property.IsKey = true;
                        entityType.KeyProperty = property;
                    }

                    var identityAttribute = prop.GetCustomAttribute<IdentityAttribute>();
                    if (identityAttribute is not null)
                    {
                        property.IsIdentity = true;
                    }

                    entityType.Properties.Add(property);
                }

                //if (entityType.KeyProperty is null)
                //{
                //    throw new ArgumentNullException($"Entity {type.Name} does not have a Key property.");
                //}

                //if (entityType.Properties.Where(x => x.IsIdentity).Count() > 1)
                //{
                //    throw new Exception("Only one auto increment column should exist.");
                //}

                return entityType;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
