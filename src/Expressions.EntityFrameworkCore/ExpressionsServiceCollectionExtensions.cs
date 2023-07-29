using Microsoft.Extensions.DependencyInjection;
using Raiqub.Expressions.Sessions;

namespace Raiqub.Expressions.EntityFrameworkCore;

public static class ExpressionsServiceCollectionExtensions
{
    /// <summary>Gets a builder for registering sessions and session factories using Entity Framework Core.</summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="tracking">The change tracking mode of injected sessions.</param>
    /// <returns>The <see cref="ExpressionsSessionBuilder"/> so that add context calls can be chained in it.</returns>
    public static ExpressionsSessionBuilder AddEntityFrameworkExpressions(
        this IServiceCollection services,
        ChangeTracking? tracking = null)
    {
        return new ExpressionsSessionBuilder(services, tracking);
    }
}
