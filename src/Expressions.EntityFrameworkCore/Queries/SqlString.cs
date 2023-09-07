using System.Runtime.CompilerServices;

namespace Raiqub.Expressions.EntityFrameworkCore.Queries;

/// <summary>Represents a SQL query string that can be either raw SQL or interpolated SQL.</summary>
public readonly struct SqlString
{
    /// <summary>Gets a value indicating whether the SQL string is raw SQL.</summary>
    public readonly bool IsRaw;

    /// <summary>Gets the SQL string as a <see cref="FormattableString"/>.</summary>
    public readonly FormattableString Sql;

    private SqlString(FormattableString sql)
    {
        Sql = sql;
        IsRaw = false;
    }

    private SqlString(string sql, object?[] parameters)
    {
        Sql = FormattableStringFactory.Create(sql, parameters);
        IsRaw = true;
    }

    /// <summary>Creates a new <see cref="SqlString"/> instance from an interpolated SQL query.</summary>
    /// <param name="sql">The interpolated string representing a SQL query with parameters.</param>
    /// <returns>A new <see cref="SqlString"/> instance representing the interpolated SQL query.</returns>
    public static SqlString FromSqlInterpolated(FormattableString sql) => new(sql);

    /// <summary>Creates a new <see cref="SqlString"/> instance from a raw SQL query.</summary>
    /// <param name="sql">The raw SQL query.</param>
    /// <param name="parameters">The parameters to interpolate into the SQL query.</param>
    /// <returns>A new <see cref="SqlString"/> instance representing the raw SQL query.</returns>
    public static SqlString FromSqlRaw(string sql, params object?[] parameters) => new(sql, parameters);
}
