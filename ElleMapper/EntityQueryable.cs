using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ElleMapper
{
    public class EntityQueryable<TElement> : IQueryable<TElement>
    {
        public IQueryProvider Provider { get; }

        public Expression Expression { get; }

        public Type ElementType => typeof(TElement);

        public EntityQueryable(IQueryProvider provider)
        {
            Provider = provider;
            Expression = Expression.Constant(this);
        }

        public EntityQueryable(IQueryProvider provider, Expression expression)
        {
            Provider = provider;
            Expression = expression;
        }

        public IEnumerator<TElement> GetEnumerator()
            => Provider.Execute<IEnumerable<TElement>>(Expression).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
