using Microsoft.EntityFrameworkCore.Storage;
using Raiqub.Expressions.Sessions;

namespace Raiqub.Expressions.EntityFrameworkCore.Sessions;

public sealed class EfDbSessionTransaction : IDbSessionTransaction
{
    private readonly IDbContextTransaction _contextTransaction;

    public EfDbSessionTransaction(IDbContextTransaction contextTransaction) =>
        _contextTransaction = contextTransaction;

    public Task CommitAsync(CancellationToken cancellationToken = default) =>
        _contextTransaction.CommitAsync(cancellationToken);

    public ValueTask DisposeAsync() =>
        _contextTransaction.DisposeAsync();

    public void Dispose() =>
        _contextTransaction.Dispose();
}
