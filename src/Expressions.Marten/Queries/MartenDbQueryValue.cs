using Marten;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions.Marten.Queries;

/// <summary>
/// Marten-based implementation of a query that can be executed to retrieve value type instances of type <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TResult">The value type of the result returned.</typeparam>
public class MartenDbQueryValue<TResult> : MartenDbQueryBase<TResult>, IDbQueryValue<TResult>
    where TResult : struct
{
    /// <summary>Initializes a new instance of the <see cref="MartenDbQueryValue{TResult}"/> class.</summary>
    /// <param name="logger">The <see cref="ILogger"/> to log to.</param>
    /// <param name="dbQueryScope">The query scope information for logging context.</param>
    /// <param name="dataSource">The data source to query from.</param>
    public MartenDbQueryValue(ILogger logger, DbQueryScope dbQueryScope, IQueryable<TResult> dataSource)
        : base(logger, dbQueryScope, dataSource)
    {
    }

    /// <inheritdoc />
    public async Task<TResult?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        using (BeginLogScope())
        {
            var enumerable = DataSource.ToAsyncEnumerable(cancellationToken);
            var enumerator = enumerable.GetAsyncEnumerator(cancellationToken);
            await using (enumerator.ConfigureAwait(false))
            {
                if (await enumerator.MoveNextAsync().ConfigureAwait(false))
                {
                    return enumerator.Current;
                }
            }

            return null;
        }
    }

    /// <inheritdoc />
    public async Task<TResult?> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        using (BeginLogScope())
        {
            var enumerable = DataSource.ToAsyncEnumerable(cancellationToken);
            var enumerator = enumerable.GetAsyncEnumerator(cancellationToken);
            await using (enumerator.ConfigureAwait(false))
            {
                if (!await enumerator.MoveNextAsync().ConfigureAwait(false))
                {
                    return null;
                }

                var result = enumerator.Current;
                if (!await enumerator.MoveNextAsync().ConfigureAwait(false))
                {
                    return result;
                }
            }

            ThrowHelper.ThrowMoreThanOneElementException();
            return null;
        }
    }
}
