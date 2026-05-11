namespace MiniOrm.Data;

public static class TypeMapper
{
    public static string ToPostgresType(Type clrType)
    {
        // Unwrap nullable types e.g. int? -> int
        var underlying = Nullable.GetUnderlyingType(clrType) ?? clrType;

        return underlying switch
        {
            _ when underlying == typeof(int)      => "INTEGER",
            _ when underlying == typeof(long)     => "BIGINT",
            _ when underlying == typeof(short)    => "SMALLINT",
            _ when underlying == typeof(bool)     => "BOOLEAN",
            _ when underlying == typeof(decimal)  => "NUMERIC",
            _ when underlying == typeof(double)   => "DOUBLE PRECISION",
            _ when underlying == typeof(float)    => "REAL",
            _ when underlying == typeof(string)   => "TEXT",
            _ when underlying == typeof(DateTime) => "TIMESTAMP",
            _ when underlying == typeof(Guid)     => "UUID",
            _ => throw new NotSupportedException($"No Postgres mapping for CLR type '{clrType.Name}'")
        };
    }
}