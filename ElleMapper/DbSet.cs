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
    //public class DbSet<TEntity> : IQueryable<TEntity>, IQueryProvider
    //{
    //    private readonly DbContext _context;

    //    List<TEntity> results = new List<TEntity>();

    //    private List<Expression<Func<TEntity, bool>>> _filters = new();

    //    private int? _skip;

    //    private int? _take;

    //    public Expression Expression { get { return Expression.Constant(this); } }

    //    public IQueryProvider Provider => this;

    //    public Type ElementType => typeof(TEntity);

    //    public DbSet(DbContext context)
    //    {
    //        _context = context ?? throw new ArgumentNullException(nameof(context));
    //    }

    //    public DbSet()
    //    {

    //    }

    //    public virtual void Add(TEntity entity)
    //    {
    //        ArgumentNullException.ThrowIfNull(entity);
    //        _context.ChangeTracker.Track(entity, EntityState.Added);
    //    }

    //    public virtual Task AddAsync(TEntity entity, CancellationToken cs = default)
    //    {
    //        ArgumentNullException.ThrowIfNull(entity);
    //        _context.ChangeTracker.Track(entity, EntityState.Added);

    //        return Task.CompletedTask;
    //    }

    //    public virtual void AddRange(IEnumerable<TEntity> entities)
    //    {
    //        ArgumentNullException.ThrowIfNull(entities);

    //        foreach (var entity in entities)
    //        {
    //            _context.ChangeTracker.Track(entity, EntityState.Added);
    //        }
    //    }

    //    public virtual Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cs = default)
    //    {
    //        ArgumentNullException.ThrowIfNull(entities);

    //        foreach (var entity in entities)
    //        {
    //            _context.ChangeTracker.Track(entity, EntityState.Added);
    //        }

    //        return Task.CompletedTask;
    //    }

    //    public virtual void Update(TEntity entity)
    //    {
    //        ArgumentNullException.ThrowIfNull(entity);
    //        _context.ChangeTracker.Track(entity, EntityState.Modified);
    //    }

    //    public virtual void UpdateRange(IEnumerable<TEntity> entities)
    //    {
    //        ArgumentNullException.ThrowIfNull(entities);

    //        foreach (var entity in entities)
    //        {
    //            _context.ChangeTracker.Track(entity, EntityState.Modified);
    //        }
    //    }

    //    public virtual void Remove(TEntity entity)
    //    {
    //        ArgumentNullException.ThrowIfNull(entity);
    //        _context.ChangeTracker.Track(entity, EntityState.Deleted);
    //    }

    //    public virtual void RemoveRange(IEnumerable<TEntity> entities)
    //    {
    //        ArgumentNullException.ThrowIfNull(entities);

    //        foreach (var entity in entities)
    //        {
    //            _context.ChangeTracker.Track(entity, EntityState.Deleted);
    //        }
    //    }

    //    public IEnumerator<TEntity> GetEnumerator()
    //        => Provider.Execute<IEnumerable<TEntity>>(Expression).GetEnumerator();

    //    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    //    public IQueryable CreateQuery(Expression expression)
    //    {
    //        return (this as IQueryProvider).CreateQuery<TEntity>(expression);
    //    }

    //    public DbSet<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
    //    {
    //        _filters.Add(predicate);
    //        return this;
    //    }

    //    public DbSet<TEntity> Skip(int count) { _skip = count; return this; }

    //    public DbSet<TEntity> Take(int count) { _take = count; return this; }

    //    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    //    {
    //        var methodCall = expression as MethodCallExpression;
    //        if (methodCall != null && methodCall.Method.Name == "Where")
    //        {
    //            ProcessExpression(methodCall.Arguments[1]);
    //            return (IQueryable<TElement>)this;
    //        }

    //        return new DbSet<TElement>();
    //    }

    //    public object? Execute(Expression expression)
    //    {
    //        var result = Execute<IEnumerable<TEntity>>(expression);
    //        return result;
    //    }

    //    public TResult Execute<TResult>(Expression expression)
    //    {
    //        // Run SQL
    //        var list = ExecuteToList();

    //        // TResult is IEnumerable<TEntity> => return the list directly
    //        if (typeof(TResult).IsAssignableFrom(typeof(List<TEntity>)))
    //            return (TResult)(object)list;

    //        // TResult is TEntity => return single item
    //        if (typeof(TResult) == typeof(TEntity))
    //            return (TResult)(object)list.First();

    //        // TResult is nullable entity (SingleOrDefault, FirstOrDefault)
    //        if (typeof(TResult) == typeof(TEntity?))
    //            return (TResult)(object?)list.FirstOrDefault();

    //        throw new NotSupportedException($"Unsupported LINQ operator for TResult: {typeof(TResult)}");
    //    }

    //    public TEntity First(Expression<Func<TEntity, bool>>? predicate = null)
    //    {
    //        if (predicate is not null) // adding where clause not fetch all
    //        {
    //            _filters.Add(predicate);
    //        }

    //        var lst = ExecuteToList();

    //        return lst.First();
    //    }

    //    public async Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cs = default)
    //    {
    //        if (predicate is not null)
    //        {
    //            _filters.Add(predicate);
    //        }

    //        var lst = await ExecuteToListAsync(cs);

    //        _filters.Clear();

    //        return lst.First();
    //    }

    //    public TEntity? FirstOrDefault(Expression<Func<TEntity, bool>>? predicate = null)
    //    {
    //        if (predicate is not null)
    //        {
    //            _filters.Add(predicate);
    //        }

    //        var lst = ExecuteToList();

    //        _filters.Clear();

    //        return lst.FirstOrDefault();
    //    }

    //    public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cs = default)
    //    {
    //        if (predicate is not null)
    //        {
    //            _filters.Add(predicate);
    //        }

    //        var lst = await ExecuteToListAsync(cs);

    //        _filters.Clear();

    //        return lst.FirstOrDefault();
    //    }

    //    public TEntity Single(Expression<Func<TEntity, bool>>? predicate = null)
    //    {
    //        if (predicate is not null)
    //        {
    //            _filters.Add(predicate);
    //        }

    //        var lst = ExecuteToList();

    //        _filters.Clear();

    //        return lst.Single();
    //    }

    //    public async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cs = default)
    //    {
    //        if (predicate is not null)
    //        {
    //            _filters.Add(predicate);
    //        }

    //        var lst = await ExecuteToListAsync(cs);

    //        _filters.Clear();

    //        return lst.Single();
    //    }

    //    public TEntity? SingleOrDefault(Expression<Func<TEntity, bool>>? predicate = null)
    //    {
    //        if (predicate is not null)
    //        {
    //            _filters.Add(predicate);
    //        }

    //        var lst = ExecuteToList();

    //        _filters.Clear();

    //        return lst.SingleOrDefault();
    //    }

    //    public async Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cs = default)
    //    {
    //        if (predicate is not null)
    //        {
    //            _filters.Add(predicate);
    //        }

    //        var lst = await ExecuteToListAsync(cs);

    //        _filters.Clear();

    //        return lst.SingleOrDefault();
    //    }

    //    public async Task<List<TEntity>> ToListAsync(CancellationToken cancellationToken = default)
    //    {
    //        return await ExecuteToListAsync(cancellationToken);
    //    }

    //    private async Task<List<TEntity>> ExecuteToListAsync(CancellationToken cs = default)
    //    {
    //        var dialect = _context._options.DatabaseProvider.Dialect;
    //        var metaData = _context.MetadataProvider.GetEntityType(typeof(TEntity));
    //        string query = $"SELECT {string.Join(',', metaData.Properties
    //            .Select(x => dialect.QuoteIdentifier(x.ColumnName)))} FROM {dialect.QuoteIdentifier(metaData.TableName)}";

    //        var parameters = new List<object?>();

    //        if (_filters.Any())
    //        {
    //            var visitor = new SqlExpressionVisitor(dialect, parameters);

    //            var parts = _filters.Select(f => visitor.Translate(f)).ToList();

    //            query += " WHERE " + string.Join(" AND ", parts);
    //        }

    //        if (_skip.HasValue || _take.HasValue)
    //        {
    //            query += $" ORDER BY {dialect.QuoteIdentifier(metaData.KeyProperty.ColumnName)} ASC";
    //            query += $" {dialect.LimitOffset(_take, _skip)}";
    //        }

    //        using var connection = _context._connectionFactory.CreateConnection();
    //        await connection.OpenAsync(cs);

    //        SqlCommand command = new(query, (SqlConnection)connection);
    //        var sqlParams = new List<SqlParameter>();

    //        for (int i = 0; i < parameters.Count; i++)
    //        {
    //            sqlParams.Add(new SqlParameter($"@p{i}", parameters[i] ?? DBNull.Value));
    //        }

    //        if (sqlParams is not null && sqlParams.Count > 0)
    //        {
    //            command.Parameters.AddRange(sqlParams.ToArray());
    //        }

    //        SqlDataAdapter adapter = new(command);
    //        DataTable dt = new();
    //        adapter.Fill(dt);

    //        await connection.CloseAsync();

    //        var lst = new List<TEntity>();

    //        foreach (DataRow row in dt.Rows)
    //        {
    //            var entity = Activator.CreateInstance<TEntity>();

    //            foreach (var property in metaData.Properties)
    //            {
    //                object? value = row[property.ColumnName];
    //                object? convertedValue = null;

    //                if (value == DBNull.Value)
    //                {
    //                    convertedValue = null;
    //                }
    //                else
    //                {
    //                    var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
    //                    convertedValue = Convert.ChangeType(value, type);
    //                }

    //                property.SetValue(entity, convertedValue!);
    //            }

    //            lst.Add((TEntity)entity);
    //        }

    //        return lst;
    //    }

    //    private List<TEntity> ExecuteToList()
    //    {
    //        var dialect = _context._options.DatabaseProvider.Dialect;
    //        var metaData = _context.MetadataProvider.GetEntityType(typeof(TEntity));
    //        string query = $"SELECT {string.Join(',', metaData.Properties
    //            .Select(x => dialect.QuoteIdentifier(x.ColumnName)))} FROM {dialect.QuoteIdentifier(metaData.TableName)}";

    //        var parameters = new List<object?>();

    //        if (_filters.Any())
    //        {
    //            var visitor = new SqlExpressionVisitor(dialect, parameters);

    //            var parts = _filters.Select(f => visitor.Translate(f)).ToList();

    //            query += " WHERE " + string.Join(" AND ", parts);
    //        }

    //        if (_skip.HasValue || _take.HasValue)
    //        {
    //            query += $" ORDER BY {dialect.QuoteIdentifier(metaData.KeyProperty.ColumnName)} ASC";
    //            query += $" {dialect.LimitOffset(_take, _skip)}";
    //        }

    //        using var connection = _context._connectionFactory.CreateConnection();
    //        connection.Open();

    //        SqlCommand command = new(query, (SqlConnection)connection);
    //        var sqlParams = new List<SqlParameter>();

    //        for (int i = 0; i < parameters.Count; i++)
    //        {
    //            sqlParams.Add(new SqlParameter($"@p{i}", parameters[i] ?? DBNull.Value));
    //        }

    //        if (sqlParams is not null && sqlParams.Count > 0)
    //        {
    //            command.Parameters.AddRange(sqlParams.ToArray());
    //        }

    //        SqlDataAdapter adapter = new(command);
    //        DataTable dt = new();
    //        adapter.Fill(dt);

    //        connection.Close();

    //        var lst = new List<TEntity>();

    //        foreach (DataRow row in dt.Rows)
    //        {
    //            var entity = Activator.CreateInstance<TEntity>();

    //            foreach (var property in metaData.Properties)
    //            {
    //                object? value = row[property.ColumnName];
    //                object? convertedValue = null;

    //                if (value == DBNull.Value)
    //                {
    //                    convertedValue = null;
    //                }
    //                else
    //                {
    //                    var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
    //                    convertedValue = Convert.ChangeType(value, type);
    //                }

    //                property.SetValue(entity, convertedValue!);
    //            }

    //            lst.Add((TEntity)entity);
    //        }

    //        return lst;
    //    }

    //    private void ProcessExpression(Expression expression)
    //    {
    //        if (expression is BinaryExpression binaryExpression)
    //        {
    //            if (binaryExpression.NodeType == ExpressionType.Equal)
    //            {
    //                ProcessEqualResult(binaryExpression);
    //            }
    //            else if (binaryExpression.NodeType == ExpressionType.LessThan)
    //            {
    //                var val = GetValue(binaryExpression);
    //            }
    //            else if (binaryExpression.NodeType == ExpressionType.GreaterThan)
    //            {
    //                var val = GetValue(binaryExpression);
    //            }
    //            else if (binaryExpression.NodeType == ExpressionType.And || binaryExpression.NodeType == ExpressionType.AndAlso)
    //            {
    //                ProcessExpression(binaryExpression.Left);
    //                ProcessExpression(binaryExpression.Right);
    //            }
    //            else if (binaryExpression.NodeType == ExpressionType.Or || binaryExpression.NodeType == ExpressionType.OrElse)
    //            {
    //                ProcessExpression(binaryExpression.Left);
    //                ProcessExpression(binaryExpression.Right);
    //            }
    //            else
    //            {
    //                ProcessAndOrResult(binaryExpression);
    //            }
    //        }
    //        else if (expression is UnaryExpression)
    //        {
    //            UnaryExpression uExp = expression as UnaryExpression;
    //            ProcessExpression(uExp.Operand);
    //        }
    //        else if (expression is LambdaExpression)
    //        {
    //            ProcessExpression(((LambdaExpression)expression).Body);
    //        }
    //        else if (expression is ParameterExpression)
    //        {
    //            var type = ((ParameterExpression)expression).Type;
    //            ProcessExpression(((ParameterExpression)expression));
    //        }
    //    }

    //    private void ProcessEqualResult(BinaryExpression expression)
    //    {
    //        if (expression.Left.NodeType == ExpressionType.MemberAccess)
    //        {
    //            var name = ((MemberExpression)expression.Left).Expression;
    //        }

    //        if (expression.Right.NodeType == ExpressionType.Constant)
    //        {
    //            var name = ((ConstantExpression)expression.Right).Value;
    //        }
    //        else
    //        {

    //        }
    //    }

    //    private void ProcessAndOrResult(BinaryExpression expression)
    //    {
    //        if (expression.NodeType == ExpressionType.And || expression.NodeType == ExpressionType.AndAlso)
    //        {
    //            ProcessAndOrResult(expression.Left as BinaryExpression);
    //            ProcessAndOrResult(expression.Right as BinaryExpression);
    //        }
    //        else if (expression.NodeType == ExpressionType.Equal)
    //        {
    //            ProcessEqualResult(expression);
    //        }
    //        else if (expression.NodeType == ExpressionType.Or || expression.NodeType == ExpressionType.OrElse)
    //        {
    //            ProcessOrOrElse(expression);
    //        }
    //        else if (expression.Right.NodeType == ExpressionType.And || expression.Right.NodeType == ExpressionType.AndAlso)
    //        {
    //            string name = (String)((ConstantExpression)expression.Right).Value;
    //        }
    //        else
    //        {

    //        }
    //    }

    //    private void ProcessOrOrElse(BinaryExpression expression)
    //    {
    //        if (expression.NodeType == ExpressionType.Or || expression.NodeType == ExpressionType.OrElse)
    //        {
    //            ProcessAndOrResult(expression.Left as BinaryExpression);
    //            ProcessAndOrResult(expression.Right as BinaryExpression);
    //        }
    //        else if (expression.NodeType == ExpressionType.Equal)
    //        {
    //            ProcessEqualResult(expression);
    //        }
    //    }

    //    private object GetValue(BinaryExpression expression)
    //    {
    //        if (expression.Right.NodeType == ExpressionType.Constant)
    //        {
    //            return ((ConstantExpression)expression.Right).Value;
    //        }
    //        return null;
    //    }
    //}

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

                var methodCall = expression as MethodCallExpression ??
                    throw new NotSupportedException("Only chained query methods are supported.");

                string terminalOperator = methodCall.Method.Name;

                var whereClauses = new List<string>();
                var selectClauses = new List<string>();
                string whereClause = string.Empty;
                string selectClause = string.Empty;
                var parameters = new List<object?>();
                LambdaExpression selectorLambda = null;
                int skip = 0;
                int take = 0;

                Expression current = methodCall;
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

                    current = mce.Arguments[0];
                }

                whereClauses.Reverse();
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

                if (terminalOperator == nameof(Queryable.Single) || terminalOperator == nameof(Queryable.SingleOrDefault) ||
                    terminalOperator == nameof(Queryable.First) || terminalOperator == nameof(Queryable.FirstOrDefault))
                {
                    query = query.Replace("SELECT *", "SELECT TOP 2 *");
                }

                if (skip > 0 || take > 0)
                {
                    query += $" ORDER BY {dialect.QuoteIdentifier(metaData.KeyProperty.ColumnName)} ASC";
                    query += $" {dialect.LimitOffset(take, skip)}";
                }

                Type mapTargetType = typeof(TEntity); // Default mapping target is TEntity

                if (selectorLambda != null)
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

                if (terminalOperator == nameof(Queryable.Single))
                    return projectedResults.Single();

                if (terminalOperator == nameof(Queryable.SingleOrDefault))
                    return projectedResults.SingleOrDefault()!;

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

                            property.SetValue(entity, convertedValue!);
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
                using var connection = _context._connectionFactory.CreateConnection();
                connection.Open();

                SqlCommand command = new(query, (SqlConnection)connection);
                var sqlParams = new List<SqlParameter>();

                for (int i = 0; i < parameters.Count; i++)
                {
                    sqlParams.Add(new SqlParameter($"@p{i}", parameters[i] ?? DBNull.Value));
                }

                if (sqlParams is not null && sqlParams.Count > 0)
                {
                    command.Parameters.AddRange(sqlParams.ToArray());
                }

                SqlDataAdapter adapter = new(command);
                DataTable dt = new();
                adapter.Fill(dt);

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

        private Func<TEntity, object?>? CompileSelector(Expression selectorArg)
        {
            var lambdaExpression = UnwrapLambda(selectorArg);

            var resultType = lambdaExpression.Body.Type;

            return Expression.Lambda<Func<TEntity, object?>>(
                Expression.Convert(lambdaExpression.Body, typeof(object)),
                lambdaExpression.Parameters
            ).Compile();
        }
    }
}
