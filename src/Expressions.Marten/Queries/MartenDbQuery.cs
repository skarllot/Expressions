using Marten;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions.Marten.Queries;

/// <summary>
/// Marten-based implementation of a query that can be executed to retrieve instances of type <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TResult">The type of the result returned.</typeparam>
public class MartenDbQuery<TResult> : MartenDbQueryBase<TResult>, IDbQuery<TResult>
    where TResult : notnull
{
    /// <summary>Initializes a new instance of the <see cref="MartenDbQuery{TResult}"/> class.</summary>
    /// <param name="logger">The <see cref="ILogger"/> to log to.</param>
    /// <param name="dbQueryScope">The query scope information for logging context.</param>
    /// <param name="dataSource">The data source to query from.</param>
    public MartenDbQuery(ILogger logger, DbQueryScope dbQueryScope, IQueryable<TResult> dataSource)
        : base(logger, dbQueryScope, dataSource)
    {
    }

    /// <inheritdoc />
    public async Task<TResult?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        using (BeginLogScope())
        {
            return await DataSource
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async Task<TResult?> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        using (BeginLogScope())
        {
            return await DataSource
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
