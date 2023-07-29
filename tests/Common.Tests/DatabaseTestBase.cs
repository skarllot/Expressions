using Microsoft.Extensions.DependencyInjection;

namespace Raiqub.Common.Tests;

public class DatabaseTestBase : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly IServiceScope _serviceScope;

    public DatabaseTestBase(Action<IServiceCollection> registerServices)
    {
        var serviceCollection = new ServiceCollection();
        registerServices(serviceCollection);

        _serviceProvider = serviceCollection.BuildServiceProvider();
        _serviceScope = _serviceProvider.CreateScope();
    }

    protected IServiceProvider ServiceProvider => _serviceScope.ServiceProvider;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _serviceScope.Dispose();
            _serviceProvider.Dispose();
        }
    }
}
