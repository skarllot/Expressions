using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.EntityFrameworkCore.Options;
using Raiqub.Expressions.EntityFrameworkCore.Queries;
using Raiqub.Expressions.Sessions;
using Raiqub.Expressions.Sessions.BoundedContext;

namespace Raiqub.Expressions.EntityFrameworkCore.Sessions;

/// <summary>Entity Framework-based implementation of a database session for querying and saving instances.</summary>
/// <typeparam name="TContext">The type of the bounded context.</typeparam>
public class EfDbSession<TContext> : EfDbSession, IDbSession<TContext>
    where TContext : DbContext
{
    /// <summary>Initializes a new instance of the <see cref="EfDbSession{TContext}"/> class.</summary>
    /// <param name="logger">The <see cref="ILogger"/> to log to.</param>
    /// <param name="context">The <see cref="Microsoft.EntityFrameworkCore.DbContext"/> to read/write.</param>
    /// <param name="sqlProviderSelector">A selector to retrieve custom SQL for querying entities.</param>
    /// <param name="optionsSelector">A selector to retrieve options for handling entities.</param>
    /// <param name="tracking">The change tracking mode of the session.</param>
    public EfDbSession(
        ILogger<EfDbSession<TContext>> logger,
        TContext context,
        ISqlProviderSelector sqlProviderSelector,
        EntityOptionsSelector optionsSelector,
        ChangeTracking tracking)
        : base(logger, context, sqlProviderSelector, optionsSelector, tracking)
    {
        Context = context;
    }

    /// <inheritdoc />
    public TContext Context { get; }
}
