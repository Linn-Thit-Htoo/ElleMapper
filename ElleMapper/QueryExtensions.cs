using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElleMapper
{
    public static class QueryExtensions
    {
        public static async Task<List<T>> ToListAsync<T>(this IQueryable<T> source, CancellationToken cs = default)
        {
            try
            {
                cs.ThrowIfCancellationRequested();

                if (source.Provider is IQueryProvider provider)
                {
                    var result = provider.Execute<IEnumerable<T>>(source.Expression);
                    return await Task.FromResult(result.ToList());
                }

                return new List<T>();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static async Task<T?> FirstAsync<T>(this IQueryable<T> source, CancellationToken cs = default)
        {
            try
            {
                cs.ThrowIfCancellationRequested();

                if (source.Provider is IQueryProvider provider)
                {
                    var result = provider.Execute<IEnumerable<T>>(source.Expression);
                    return await Task.FromResult(result.First());
                }

                return default;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static async Task<T?> FirstOrDefaultAsync<T>(this IQueryable<T> source, CancellationToken cs = default)
        {
            try
            {
                cs.ThrowIfCancellationRequested();

                if (source.Provider is IQueryProvider provider)
                {
                    var result = provider.Execute<IEnumerable<T>>(source.Expression);
                    return await Task.FromResult(result.FirstOrDefault());
                }

                return default;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static async Task<T?> SingleOrDefaultAsync<T>(this IQueryable<T> source, CancellationToken cs = default)
        {
            try
            {
                cs.ThrowIfCancellationRequested();

                if (source.Provider is IQueryProvider provider)
                {
                    var result = provider.Execute<IEnumerable<T>>(source.Expression);
                    return await Task.FromResult(result.SingleOrDefault());
                }

                return default;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
