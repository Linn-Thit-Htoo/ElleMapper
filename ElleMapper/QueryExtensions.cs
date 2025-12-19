using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ElleMapper
{
    public static class QueryExtensions
    {
        /// <summary>
        /// Creates a List containing the elements of the sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the source.</typeparam>
        /// <param name="source">The source sequence to convert to a list.</param>
        /// <param name="cancellationToken">Used to cancel the asynchronous operation.</param>
        /// <returns>A List containing the elements of the input sequence.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when source is null.
        /// </exception>
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

        /// <summary>
        /// Returns the only element of a sequence, and throws an exception if there is not 
        /// exactly one element in the sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the source.</typeparam>
        /// <param name="source">The source to return the single element from.</param>
        /// <param name="cancellationToken">Used to cancel the asynchronous operation.</param>
        /// <returns>The single element of the input sequence.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when source is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the sequence contains no elements.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the sequence contains more than one element.
        /// </exception>
        public static async Task<T> SingleAsync<T>(this IQueryable<T> source, CancellationToken cs = default)
        {
            try
            {
                cs.ThrowIfCancellationRequested();

                if (source.Provider is IQueryProvider provider)
                {
                    var result = provider.Execute<IEnumerable<T>>(source.Expression);
                    return await Task.FromResult(result.Single());
                }

                return default!;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Returns the first element of a sequence, and throws an exception if the sequence 
        /// contains no elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the source.</typeparam>
        /// <param name="source">The source to return the first element from.</param>
        /// <param name="cancellationToken">Used to cancel the asynchronous operation.</param>
        /// <returns>The first element of the input sequence.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when source is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the source sequence is empty.
        /// </exception>
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

        /// <summary>
        /// Returns the first element of a sequence, or a default value if the sequence contains
        /// no elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the source.</typeparam>
        /// <param name="source">The source to return the first element from.</param>
        /// <param name="cancellationToken">Used to cancel the asynchronous operation.</param>
        /// <returns>
        /// The first element of the input sequence, or the default value of T if the sequence
        /// contains no elements.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when source is null.
        /// </exception>
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

        /// <summary>
        /// Returns the only element of a sequence, or a default value if the sequence contains
        /// no elements. Throws an exception if the sequence contains more than one element.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the source.</typeparam>
        /// <param name="source">The source to return the single element from.</param>
        /// <param name="cancellationToken">Used to cancel the asynchronous operation.</param>
        /// <returns>
        /// The single element of the input sequence, or the default value of T if the sequence
        /// contains no elements.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when source is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the sequence contains more than one element.
        /// </exception>
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

        /// <summary>
        /// Returns the count value
        /// </summary>
        /// <typeparam name="T">The type of the elements of the source.</typeparam>
        /// <param name="source">The source to return the single element from.</param>
        /// <param name="cancellationToken">Used to cancel the asynchronous operation.</param>
        /// <returns>
        /// The single element of the input sequence, or the default value of T if the sequence
        /// contains no elements.
        /// </returns>
        public static async Task<int> CountAsync<T>(this IQueryable<T> source, CancellationToken cs = default)
        {
            try
            {
                cs.ThrowIfCancellationRequested();

                if (source.Provider is IQueryProvider provider)
                {
                    var result = provider.Execute<IEnumerable<T>>(source.Expression);
                    return await Task.FromResult(result.Count());
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
