using MysticMind.PostgresEmbed;

namespace Raiqub.Common.Tests.Commons;

public static class PgServerExtensions
{
    public static string GetConnectionString(this PgServer pgServer) =>
        $"Server=localhost;Port={pgServer.PgPort};User Id={pgServer.PgUser};Password=test;Database={pgServer.PgDbName};Pooling=false";
}
