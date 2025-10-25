using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions.EntityFrameworkCore.Queries;

/// <summary>
/// Entity Framework-based implementation of a query that can be executed to retrieve value type instances of type <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TResult">The type of the result returned.</typeparam>
public class EfDbQueryValue<TResult> : EfDbQueryBase<TResult>, IDbQueryValue<TResult>
    where TResult : struct
{
    /// <summary>Initializes a new instance of the <see cref="EfDbQueryValue{TResult}"/> class.</summary>
    /// <param name="logger">The <see cref="ILogger"/> to log to.</param>
    /// <param name="dbQueryScope">The query scope information for logging context.</param>
    /// <param name="dataSource">The data source to query from.</param>
    public EfDbQueryValue(ILogger logger, DbQueryScope dbQueryScope, IQueryable<TResult> dataSource)
        : base(logger, dbQueryScope, dataSource)
    {
    }

    /// <inheritdoc />
    public async Task<TResult?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        using (BeginLogScope())
        {
            var wrappedResult = await DataSource
                .Select(x => new StrongBox<TResult>(x))
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
            return wrappedResult?.Value;
        }
    }

    /// <inheritdoc />
    public async Task<TResult?> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        using (BeginLogScope())
        {
            var wrappedResult = await DataSource
                .Select(x => new StrongBox<TResult>(x))
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
            return wrappedResult?.Value;
        }
    }
}
