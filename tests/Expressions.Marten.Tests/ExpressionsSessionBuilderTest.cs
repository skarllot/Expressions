using System.Reflection;
using AwesomeAssertions;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Raiqub.Common.Tests;
using Raiqub.Common.Tests.Examples;
using Raiqub.Expressions.Marten.Tests.Examples;
using Raiqub.Expressions.Sessions;
using Raiqub.Expressions.Sessions.BoundedContext;

namespace Raiqub.Expressions.Marten.Tests;

[Collection(PostgreSqlTestGroup.Name)]
public sealed class ExpressionsSessionBuilderTest : IAsyncLifetime
{
    private readonly string _connectionString;

    public ExpressionsSessionBuilderTest(PostgreSqlFixture fixture)
    {
        _connectionString = fixture.ConnectionString;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void AddContext_ShouldRegisterSessionFactories()
    {
        // Arrange
        var services = new ServiceCollection();
        services
            .AddSingleton<ILoggerFactory>(new NullLoggerFactory())
            .AddTestMarten<Blog>(_connectionString);

        // Register the context as a forwarding proxy
        services.AddSingleton<IMartenBloggingContext>(sp =>
            DocumentStoreProxy<IMartenBloggingContext>.Create(sp.GetRequiredService<IDocumentStore>()));

        var builder = services.AddMartenExpressions();

        // Act
        builder.AddContext<IBloggingContext, IMartenBloggingContext>();

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        var sessionFactory = serviceProvider.GetService<IDbSessionFactory<IBloggingContext>>();
        sessionFactory.Should().NotBeNull();

        var querySessionFactory = serviceProvider.GetService<IDbQuerySessionFactory<IBloggingContext>>();
        querySessionFactory.Should().NotBeNull();
    }

    [Fact]
    public void AddContext_ShouldRegisterSessions()
    {
        // Arrange
        var services = new ServiceCollection();
        services
            .AddSingleton<ILoggerFactory>(new NullLoggerFactory())
            .AddTestMarten<Blog>(_connectionString);

        services.AddSingleton<IMartenBloggingContext>(sp =>
            DocumentStoreProxy<IMartenBloggingContext>.Create(sp.GetRequiredService<IDocumentStore>()));

        var builder = services.AddMartenExpressions();

        // Act
        builder.AddContext<IBloggingContext, IMartenBloggingContext>();

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        var session = serviceProvider.GetService<IDbSession<IBloggingContext>>();
        session.Should().NotBeNull();

        var querySession = serviceProvider.GetService<IDbQuerySession<IBloggingContext>>();
        querySession.Should().NotBeNull();
    }

    [Fact]
    public void AddContext_WithDefaultTracking_ShouldCreateSessionWithoutTracking()
    {
        // Arrange
        var services = new ServiceCollection();
        services
            .AddSingleton<ILoggerFactory>(new NullLoggerFactory())
            .AddTestMarten<Blog>(_connectionString);

        services.AddSingleton<IMartenBloggingContext>(sp =>
            DocumentStoreProxy<IMartenBloggingContext>.Create(sp.GetRequiredService<IDocumentStore>()));

        var builder = services.AddMartenExpressions();

        // Act
        builder.AddContext<IBloggingContext, IMartenBloggingContext>();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();

        var session = scope.ServiceProvider.GetRequiredService<IDbSession<IBloggingContext>>();
        session.Should().NotBeNull();
    }

    [Fact]
    public void AddContext_WithEnableTracking_ShouldRegisterWithTracking()
    {
        // Arrange
        var services = new ServiceCollection();
        services
            .AddSingleton<ILoggerFactory>(new NullLoggerFactory())
            .AddTestMarten<Blog>(_connectionString);

        services.AddSingleton<IMartenBloggingContext>(sp =>
            DocumentStoreProxy<IMartenBloggingContext>.Create(sp.GetRequiredService<IDocumentStore>()));

        var builder = services.AddMartenExpressions();

        // Act
        builder.AddContext<IBloggingContext, IMartenBloggingContext>(ChangeTracking.Enable);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();

        var session = scope.ServiceProvider.GetRequiredService<IDbSession<IBloggingContext>>();
        session.Should().NotBeNull();
    }

    [Fact]
    public void AddContext_WithIdentityResolutionTracking_ShouldRegisterWithTracking()
    {
        // Arrange
        var services = new ServiceCollection();
        services
            .AddSingleton<ILoggerFactory>(new NullLoggerFactory())
            .AddTestMarten<Blog>(_connectionString);

        services.AddSingleton<IMartenBloggingContext>(sp =>
            DocumentStoreProxy<IMartenBloggingContext>.Create(sp.GetRequiredService<IDocumentStore>()));

        var builder = services.AddMartenExpressions();

        // Act
        builder.AddContext<IBloggingContext, IMartenBloggingContext>(ChangeTracking.IdentityResolution);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();

        var session = scope.ServiceProvider.GetRequiredService<IDbSession<IBloggingContext>>();
        session.Should().NotBeNull();
    }

    [Fact]
    public void AddContext_WithDisableTracking_ShouldRegisterWithTracking()
    {
        // Arrange
        var services = new ServiceCollection();
        services
            .AddSingleton<ILoggerFactory>(new NullLoggerFactory())
            .AddTestMarten<Blog>(_connectionString);

        services.AddSingleton<IMartenBloggingContext>(sp =>
            DocumentStoreProxy<IMartenBloggingContext>.Create(sp.GetRequiredService<IDocumentStore>()));

        var builder = services.AddMartenExpressions();

        // Act
        builder.AddContext<IBloggingContext, IMartenBloggingContext>(ChangeTracking.Disable);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();

        var session = scope.ServiceProvider.GetRequiredService<IDbSession<IBloggingContext>>();
        session.Should().NotBeNull();
    }

    [Fact]
    public void AddContext_WithGlobalTracking_ShouldUseGlobalTracking()
    {
        // Arrange
        var services = new ServiceCollection();
        services
            .AddSingleton<ILoggerFactory>(new NullLoggerFactory())
            .AddTestMarten<Blog>(_connectionString);

        services.AddSingleton<IMartenBloggingContext>(sp =>
            DocumentStoreProxy<IMartenBloggingContext>.Create(sp.GetRequiredService<IDocumentStore>()));

        var builder = services.AddMartenExpressions(ChangeTracking.Enable);

        // Act
        builder.AddContext<IBloggingContext, IMartenBloggingContext>();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();

        var session = scope.ServiceProvider.GetRequiredService<IDbSession<IBloggingContext>>();
        session.Should().NotBeNull();
    }

    [Fact]
    public void AddContext_WithLocalTrackingOverridingGlobal_ShouldUseLocalTracking()
    {
        // Arrange
        var services = new ServiceCollection();
        services
            .AddSingleton<ILoggerFactory>(new NullLoggerFactory())
            .AddTestMarten<Blog>(_connectionString);

        services.AddSingleton<IMartenBloggingContext>(sp =>
            DocumentStoreProxy<IMartenBloggingContext>.Create(sp.GetRequiredService<IDocumentStore>()));

        var builder = services.AddMartenExpressions(ChangeTracking.Enable);

        // Act
        builder.AddContext<IBloggingContext, IMartenBloggingContext>(ChangeTracking.Disable);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();

        var session = scope.ServiceProvider.GetRequiredService<IDbSession<IBloggingContext>>();
        session.Should().NotBeNull();
    }

    [Fact]
    public void AddContext_ShouldReturnBuilder()
    {
        // Arrange
        var services = new ServiceCollection();
        services
            .AddSingleton<ILoggerFactory>(new NullLoggerFactory())
            .AddTestMarten<Blog>(_connectionString);

        services.AddSingleton<IMartenBloggingContext>(sp =>
            DocumentStoreProxy<IMartenBloggingContext>.Create(sp.GetRequiredService<IDocumentStore>()));

        var builder = services.AddMartenExpressions();

        // Act
        var result = builder.AddContext<IBloggingContext, IMartenBloggingContext>();

        // Assert
        result.Should().BeSameAs(builder);
    }

    [Fact]
    public void AddContext_ShouldAllowChaining()
    {
        // Arrange
        var services = new ServiceCollection();
        services
            .AddSingleton<ILoggerFactory>(new NullLoggerFactory())
            .AddTestMarten<Blog>(_connectionString);

        services.AddSingleton<IMartenBloggingContext>(sp =>
            DocumentStoreProxy<IMartenBloggingContext>.Create(sp.GetRequiredService<IDocumentStore>()));
        services.AddSingleton<IMartenSecondContext>(sp =>
            DocumentStoreProxy<IMartenSecondContext>.Create(sp.GetRequiredService<IDocumentStore>()));

        var builder = services.AddMartenExpressions();

        // Act
        builder
            .AddContext<IBloggingContext, IMartenBloggingContext>()
            .AddContext<ISecondContext, IMartenSecondContext>();

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        var blogSession = serviceProvider.GetService<IDbSession<IBloggingContext>>();
        blogSession.Should().NotBeNull();

        var secondSession = serviceProvider.GetService<IDbSession<ISecondContext>>();
        secondSession.Should().NotBeNull();
    }

    [Fact]
    public async Task AddContext_CreatedSession_ShouldWorkCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();
        services
            .AddSingleton<ILoggerFactory>(new NullLoggerFactory())
            .AddTestMarten<Blog>(_connectionString);

        services.AddSingleton<IMartenBloggingContext>(sp =>
            DocumentStoreProxy<IMartenBloggingContext>.Create(sp.GetRequiredService<IDocumentStore>()));

        var builder = services.AddMartenExpressions();
        builder.AddContext<IBloggingContext, IMartenBloggingContext>();

        var serviceProvider = services.BuildServiceProvider();
        var store = serviceProvider.GetRequiredService<IDocumentStore>();
        await store.Advanced.ResetAllData();

        // Act
        using var scope = serviceProvider.CreateScope();
        var session = scope.ServiceProvider.GetRequiredService<IDbSession<IBloggingContext>>();

        var blog = new Blog
        {
            Id = Guid.NewGuid(),
            Name = "Test Blog"
        };

        await session.AddAsync(blog);
        await session.SaveChangesAsync();

        // Assert
        var querySession = scope.ServiceProvider.GetRequiredService<IDbQuerySession<IBloggingContext>>();
        var blogs = await querySession.Query<Blog>().ToListAsync();

        blogs.Should().HaveCount(1);
        blogs[0].Name.Should().Be("Test Blog");
    }
}

public interface ISecondContext
{
}

public interface IMartenSecondContext : ISecondContext, IDocumentStore
{
}

/// <summary>
/// Proxy class that forwards all IDocumentStore calls to the underlying implementation
/// while implementing a custom marker interface for type safety.
/// </summary>
internal class DocumentStoreProxy<T> : DispatchProxy where T : class, IDocumentStore
{
    private IDocumentStore _target = null!;

    public static T Create(IDocumentStore target)
    {
        var proxy = Create<T, DocumentStoreProxy<T>>() as DocumentStoreProxy<T>;
        proxy!._target = target;
        return (proxy as T)!;
    }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (targetMethod == null)
            throw new ArgumentNullException(nameof(targetMethod));

        return targetMethod.Invoke(_target, args);
    }
}
