namespace Raiqub.Expressions.Sessions;

public interface IReadSession<out TContext> : IAsyncDisposable, IDisposable
{
    TContext Context { get; }

    ChangeTracking? Tracking { get; }
}
