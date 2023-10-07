using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.EntityFrameworkCore.Options;
using Raiqub.Expressions.EntityFrameworkCore.Queries;
using Raiqub.Expressions.Sessions;
using Raiqub.Expressions.Sessions.BoundedContext;

namespace Raiqub.Expressions.EntityFrameworkCore.Sessions;

public class EfDbSession<TContext> : EfDbSession, IDbSession<TContext>
    where TContext : DbContext
{
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

    public TContext Context { get; }
}
