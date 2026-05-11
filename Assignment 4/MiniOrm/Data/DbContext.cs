using Npgsql;
using MiniOrm.Data;

namespace MiniOrm.Data;

public abstract class DbContext : IDisposable
{
    protected readonly NpgsqlConnection Connection;

    protected DbContext(string connectionString)
    {
        Connection = new NpgsqlConnection(connectionString);
        Connection.Open();
        RegisterSets();
    }

    // Override in subclass to register DbSets via reflection
    private void RegisterSets()
    {
        var setType = typeof(DbSet<>);
        foreach (var prop in GetType().GetProperties())
        {
            if (!prop.PropertyType.IsGenericType) continue;
            if (prop.PropertyType.GetGenericTypeDefinition() != setType) continue;

            var entityType   = prop.PropertyType.GetGenericArguments()[0];
            var dbSetInstance = Activator.CreateInstance(
                typeof(DbSet<>).MakeGenericType(entityType), Connection);
            prop.SetValue(this, dbSetInstance);
        }
    }

    public void Dispose() => Connection.Dispose();
}