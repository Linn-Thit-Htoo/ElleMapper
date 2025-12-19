using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ElleMapper
{
    public class DbSet<TEntity> : IQueryable<TEntity>, IQueryProvider
    {
        private readonly DbContext _context;
        public Expression Expression { get; }
        public IQueryProvider Provider => this;
        public Type ElementType => typeof(TEntity);

        public DbSet(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            this.Expression = Expression.Constant(this);
        }

        public virtual void Add(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            _context.ChangeTracker.Track(entity, EntityState.Added);
        }

        public virtual Task AddAsync(TEntity entity, CancellationToken cs = default)
        {
            ArgumentNullException.ThrowIfNull(entity);
            _context.ChangeTracker.Track(entity, EntityState.Added);

            return Task.CompletedTask;
        }

        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            ArgumentNullException.ThrowIfNull(entities);

            foreach (var entity in entities)
            {
                _context.ChangeTracker.Track(entity, EntityState.Added);
            }
        }

        public virtual Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cs = default)
        {
            ArgumentNullException.ThrowIfNull(entities);

            foreach (var entity in entities)
            {
                _context.ChangeTracker.Track(entity, EntityState.Added);
            }

            return Task.CompletedTask;
        }

        public virtual void Update(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            _context.ChangeTracker.Track(entity, EntityState.Modified);
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entities)
        {
            ArgumentNullException.ThrowIfNull(entities);

            foreach (var entity in entities)
            {
                _context.ChangeTracker.Track(entity, EntityState.Modified);
            }
        }

        public virtual void Remove(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            _context.ChangeTracker.Track(entity, EntityState.Deleted);
        }

        public virtual void RemoveRange(IEnumerable<TEntity> entities)
        {
            ArgumentNullException.ThrowIfNull(entities);

            foreach (var entity in entities)
            {
                _context.ChangeTracker.Track(entity, EntityState.Deleted);
            }
        }

        public IEnumerator<TEntity> GetEnumerator()
            => Provider.Execute<IEnumerable<TEntity>>(Expression).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IQueryable CreateQuery(Expression expression)
        {
            Type elementType = expression.Type.GetGenericArguments().First();
            var genericMethod = GetType()
                .GetMethod(nameof(CreateQuery), 1, new[] { typeof(Expression) })!
                .MakeGenericMethod(elementType);
            return (IQueryable)genericMethod.Invoke(this, new object[] { expression })!;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new EntityQueryable<TElement>(this, expression);
        }

        public object? Execute(Expression expression) => Execute<object?>(expression);

        public TResult Execute<TResult>(Expression expression)
        {
            try
            {
                var dialect = _context._options.DatabaseProvider.Dialect;
                var metaData = _context.MetadataProvider.GetEntityType(typeof(TEntity));

                var methodCall = expression as MethodCallExpression;

                string terminalOperator = string.Empty;

                if (methodCall is not null)
                {
                    terminalOperator = methodCall.Method.Name;
                }

                var whereClauses = new List<string>();
                var selectClauses = new List<string>();
                string whereClause = string.Empty;
                string selectClause = string.Empty;
                var parameters = new List<object?>();
                LambdaExpression selectorLambda = null;
                int skip = 0;
                int take = 0;
                bool isCount = false;

                Expression current = methodCall;

                if (current is not null)
                {
                    while (current.NodeType == ExpressionType.Call)
                    {
                        var mce = (MethodCallExpression)current;
                        if (mce.Method.Name == "Where")
                        {
                            var lambda = UnwrapLambda(mce.Arguments[1]);
                            var translator = new SqlExpressionVisitor(_context._options.DatabaseProvider.Dialect, parameters);
                            whereClauses.Add(translator.Translate(lambda.Body));
                        }

                        if (mce.Method.Name == "Select")
                        {
                            var selectorArg = mce.Arguments[1];
                            selectorLambda = UnwrapLambda(selectorArg);
                            ExtractSelectColumns(selectorLambda, metaData, dialect, selectClauses);
                        }

                        if (mce.Method.Name == "Skip")
                        {
                            var arg = mce.Arguments[1];

                            if (arg is ConstantExpression constantExpr)
                            {
                                skip = (int)constantExpr.Value!;
                            }
                        }

                        if (mce.Method.Name == "Take")
                        {
                            var arg = mce.Arguments[1];

                            if (arg is ConstantExpression constantExpr)
                            {
                                take = (int)constantExpr.Value!;
                            }
                        }

                        if (mce.Method.Name == "Count")
                        {
                            isCount = true;
                        }

                        current = mce.Arguments[0];
                    }
                }

                whereClause = string.Join(" AND ", whereClauses);
                selectClause = string.Join(",", selectClauses);
                string query = string.Empty;

                if (!string.IsNullOrEmpty(selectClause))
                {
                    query = $"SELECT {selectClause} FROM {dialect.QuoteIdentifier(metaData.TableName)}";
                }
                else
                {
                    query = $"SELECT {string.Join(',', metaData.Properties
                    .Select(x => dialect.QuoteIdentifier(x.ColumnName)))} FROM {dialect.QuoteIdentifier(metaData.TableName)}";
                }

                if (!string.IsNullOrEmpty(whereClause))
                {
                    query += $" WHERE {whereClause}";
                }

                if (skip > 0 || take > 0)
                {
                    query += $" ORDER BY {dialect.QuoteIdentifier(metaData.KeyProperty.ColumnName)} ASC";
                    query += $" {dialect.LimitOffset(take, skip)}";
                }

                if (isCount)
                {
                    query = $"SELECT COUNT(*) FROM {dialect.QuoteIdentifier(metaData.TableName)} WHERE {whereClause}";
                }

                Console.WriteLine(query);

                Type mapTargetType = typeof(TEntity); // Default mapping target is TEntity

                if (selectorLambda is not null)
                {
                    mapTargetType = selectorLambda.Body.Type;
                }

                DataTable dt = RunQueryAndMapRaw(query, parameters);

                var mapMethod = GetType()
                    .GetMethod(nameof(MapResults), BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(mapTargetType);

                object? resultsObj = mapMethod.Invoke(this, new object[] { dt });

                var resultsList = (IList)resultsObj!;

                var projectedResults = resultsList.Cast<TResult>();

                if (!string.IsNullOrEmpty(terminalOperator))
                {
                    if (terminalOperator == nameof(Queryable.Single))
                        return projectedResults.Single();

                    if (terminalOperator == nameof(Queryable.First))
                        return projectedResults.First();

                    if (terminalOperator == nameof(Queryable.FirstOrDefault))
                        return projectedResults.FirstOrDefault()!;

                    if (terminalOperator == nameof(Queryable.SingleOrDefault))
                        return projectedResults.SingleOrDefault()!;

                    if (terminalOperator == nameof(Queryable.Count))
                    {
                        object rawValue = dt.Rows.Count > 0 ? dt.Rows[0][0] : 0;
                        int count = Convert.ToInt32(rawValue);

                        return (TResult)(object)count;
                    }
                }

                return (TResult)resultsObj!;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static LambdaExpression UnwrapLambda(Expression expr)
        {
            if (expr is LambdaExpression lambda)
                return lambda;

            if (expr is UnaryExpression unary && unary.Operand is LambdaExpression innerLambda)
                return innerLambda;

            throw new NotSupportedException("Unsupported expression type for lambda extraction.");
        }

        private void ExtractSelectColumns(
            LambdaExpression selector,
            EntityType metaData,
            ISqlDialect dialect,
            List<string> selectClauses)
        {
            try
            {
                var body = StripConvert(selector.Body);

                if (body is MemberExpression mem)
                {
                    var col = metaData.GetColumnName(mem.Member.Name);
                    selectClauses.Add(dialect.QuoteIdentifier(col));
                    return;
                }

                if (body is NewExpression nex)
                {
                    foreach (var arg in nex.Arguments)
                    {
                        var stripped = StripConvert(arg);

                        if (stripped is not MemberExpression memArg)
                            throw new NotSupportedException("Only simple member projections supported.");

                        var col = metaData.GetColumnName(memArg.Member.Name);
                        selectClauses.Add(dialect.QuoteIdentifier(col));
                    }
                    return;
                }

                if (body is MemberInitExpression init)
                {
                    foreach (var binding in init.Bindings)
                    {
                        if (binding is MemberAssignment ma)
                        {
                            var stripped = StripConvert(ma.Expression);

                            if (stripped is not MemberExpression memArg)
                                throw new NotSupportedException("Initializer must assign simple members.");

                            var col = metaData.GetColumnName(memArg.Member.Name);
                            selectClauses.Add(dialect.QuoteIdentifier(col));
                        }
                    }
                    return;
                }

                throw new NotSupportedException("Unsupported Select projection.");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private Expression StripConvert(Expression expr)
        {
            try
            {
                while (expr.NodeType == ExpressionType.Convert || expr.NodeType == ExpressionType.ConvertChecked)
                    expr = ((UnaryExpression)expr).Operand;

                return expr;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private List<TTarget> MapResults<TTarget>(DataTable dt) where TTarget : new()
        {
            try
            {
                var targetType = typeof(TTarget);
                var properties = targetType.GetProperties();
                var lst = new List<TTarget>();

                foreach (DataRow row in dt.Rows)
                {
                    var entity = Activator.CreateInstance<TTarget>();

                    foreach (var property in properties)
                    {
                        string columnName = GetColumnNameFromProperty(targetType, property.Name);

                        if (!string.IsNullOrEmpty(columnName)) // need to skip if column name is null
                        {
                            if (dt.Columns.Contains(columnName))
                            {
                                object? value = row[columnName];
                                object? convertedValue = null;

                                if (value == DBNull.Value)
                                {
                                    convertedValue = null;
                                }
                                else
                                {
                                    var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                                    convertedValue = Convert.ChangeType(value, type);
                                }

                                property.SetValue(entity, convertedValue);
                            }
                        }
                    }

                    lst.Add(entity);
                }

                return lst;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private DataTable RunQueryAndMapRaw(string query, List<object?> parameters)
        {
            try
            {
                DbProviderFactory factory = _context._connectionFactory.GetDbProviderFactory();

                using var connection = _context._connectionFactory.CreateConnection();
                connection.Open();

                using DbCommand command = factory.CreateCommand()!;
                command.Connection = connection;
                command.CommandText = query;
                var sqlParams = new List<DbParameter>();

                for (int i = 0; i < parameters.Count; i++)
                {
                    var param = command.CreateParameter();
                    param.ParameterName = $"@p{i}";
                    param.Value = parameters[i] ?? DBNull.Value;
                    sqlParams.Add(param);
                }

                if (sqlParams is not null && sqlParams.Count > 0)
                {
                    command.Parameters.AddRange(sqlParams.ToArray());
                }

                using DbDataReader reader = command.ExecuteReader();
                DataTable dt = new();
                dt.Load(reader);

                connection.Close();
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string GetColumnNameFromProperty(Type type, string propertyName)
        {
            if (type == typeof(TEntity))
            {
                return _context.MetadataProvider.GetEntityType(type).GetColumnName(propertyName);
            }

            return propertyName;
        }
    }
}
