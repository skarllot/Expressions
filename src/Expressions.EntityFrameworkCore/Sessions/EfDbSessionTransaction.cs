using Microsoft.EntityFrameworkCore.Storage;
using Raiqub.Expressions.Sessions;

namespace Raiqub.Expressions.EntityFrameworkCore.Sessions;

/// <summary>Entity Framework-based implementation of started database transaction.</summary>
public sealed class EfDbSessionTransaction : IDbSessionTransaction
{
    private readonly IDbContextTransaction _contextTransaction;

    /// <summary>Initializes a new instance of the <see cref="EfDbSessionTransaction"/> class.</summary>
    /// <param name="contextTransaction">The Entity Framework transaction to wrap.</param>
    public EfDbSessionTransaction(IDbContextTransaction contextTransaction) =>
        _contextTransaction = contextTransaction;

    /// <inheritdoc />
    public Task CommitAsync(CancellationToken cancellationToken = default) =>
        _contextTransaction.CommitAsync(cancellationToken);

    /// <inheritdoc />
    public ValueTask DisposeAsync() =>
        _contextTransaction.DisposeAsync();

    /// <inheritdoc />
    public void Dispose() =>
        _contextTransaction.Dispose();
}
