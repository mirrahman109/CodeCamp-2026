using System.Reflection;
using Npgsql;
using MiniOrm.Attributes;

namespace MiniOrm.Data;

public class DbSet<T> where T : class, new()
{
    private readonly NpgsqlConnection _conn;
    private readonly EntityMetadata   _meta;

    public DbSet(NpgsqlConnection conn)
    {
        _conn = conn;
        _meta = new EntityMetadata(typeof(T));
    }

    // ── INSERT ────────────────────────────────────────────────────────────
    public void Insert(T entity)
    {
        var cols = _meta.Columns.Where(p => p != _meta.PrimaryKey).ToList();
        var colNames  = string.Join(", ", cols.Select(_meta.GetColumnName));
        var paramNames = string.Join(", ", cols.Select((_, i) => $"@p{i}"));

        var sql = $"INSERT INTO {_meta.TableName} ({colNames}) VALUES ({paramNames})";
        using var cmd = new NpgsqlCommand(sql, _conn);
        for (int i = 0; i < cols.Count; i++)
            cmd.Parameters.AddWithValue($"@p{i}", cols[i].GetValue(entity) ?? DBNull.Value);
        cmd.ExecuteNonQuery();
    }

    // ── SELECT ALL ────────────────────────────────────────────────────────
    public List<T> GetAll()
    {
        var sql = $"SELECT * FROM {_meta.TableName}";
        using var cmd    = new NpgsqlCommand(sql, _conn);
        using var reader = cmd.ExecuteReader();
        return ReadAll(reader);
    }

    // ── SELECT BY ID ──────────────────────────────────────────────────────
    public T? GetById(object id)
    {
        var pkCol = _meta.GetPrimaryKeyColumnName();
        var sql   = $"SELECT * FROM {_meta.TableName} WHERE {pkCol} = @id";
        using var cmd = new NpgsqlCommand(sql, _conn);
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = cmd.ExecuteReader();
        return ReadAll(reader).FirstOrDefault();
    }

    // ── UPDATE ────────────────────────────────────────────────────────────
    public void Update(T entity)
    {
        var cols = _meta.Columns.Where(p => p != _meta.PrimaryKey).ToList();
        var setClauses = string.Join(", ", cols.Select((c, i) => $"{_meta.GetColumnName(c)} = @p{i}"));
        var pkCol = _meta.GetPrimaryKeyColumnName();
        var sql   = $"UPDATE {_meta.TableName} SET {setClauses} WHERE {pkCol} = @pk";

        using var cmd = new NpgsqlCommand(sql, _conn);
        for (int i = 0; i < cols.Count; i++)
            cmd.Parameters.AddWithValue($"@p{i}", cols[i].GetValue(entity) ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@pk", _meta.PrimaryKey.GetValue(entity)!);
        cmd.ExecuteNonQuery();
    }

    // ── DELETE ────────────────────────────────────────────────────────────
    public void Delete(object id)
    {
        var pkCol = _meta.GetPrimaryKeyColumnName();
        var sql   = $"DELETE FROM {_meta.TableName} WHERE {pkCol} = @id";
        using var cmd = new NpgsqlCommand(sql, _conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    // ── HELPER ────────────────────────────────────────────────────────────
    private List<T> ReadAll(NpgsqlDataReader reader)
    {
        var results = new List<T>();
        while (reader.Read())
        {
            var obj = new T();
            foreach (var prop in _meta.Columns)
            {
                var colName = _meta.GetColumnName(prop);
                try
                {
                    var val = reader[colName];
                    if (val is DBNull) prop.SetValue(obj, null);
                    else
                    {
                        var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                        prop.SetValue(obj, Convert.ChangeType(val, targetType));
                    }
                }
                catch { /* column might not exist; skip */ }
            }
            results.Add(obj);
        }
        return results;
    }
}