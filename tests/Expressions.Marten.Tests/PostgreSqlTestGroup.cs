using Raiqub.Common.Tests;

namespace Raiqub.Expressions.Marten.Tests;

[CollectionDefinition(Name)]
public class PostgreSqlTestGroup : ICollectionFixture<PostgreSqlFixture>
{
    public const string Name = nameof(PostgreSqlTestGroup);
}
