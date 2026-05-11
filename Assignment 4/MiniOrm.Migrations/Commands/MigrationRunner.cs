using Npgsql;

namespace MiniOrm.Migrations.Commands;

public class MigrationRunner
{
    private readonly string _connStr;
    private readonly string _migrationsDir;

    public MigrationRunner(string connStr, string? dir = null)
    {
        _connStr = connStr;
        _migrationsDir = dir ?? Path.Combine(AppContext.BaseDirectory, "Migrations");
        Directory.CreateDirectory(_migrationsDir);
        EnsureHistoryTable();
    }

    // ── Ensure tracking table exists ───────────────────────────────────────
    private void EnsureHistoryTable()
    {
        using var conn = Open();
        var sql = """
            CREATE TABLE IF NOT EXISTS __migrations (
                id          SERIAL PRIMARY KEY,
                name        TEXT NOT NULL UNIQUE,
                applied_at  TIMESTAMP NOT NULL DEFAULT NOW()
            );
            """;
        new NpgsqlCommand(sql, conn).ExecuteNonQuery();
    }

    // ── ADD: scaffold a new migration file ────────────────────────────────
    public void Add(string name)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var fileName  = $"{timestamp}_{name}.sql";
        var filePath  = Path.Combine(_migrationsDir, fileName);

        File.WriteAllText(filePath, $"""
            -- Migration: {name}
            -- Created:   {DateTime.UtcNow:u}

            -- Write your UP SQL below:


            """);

        Console.WriteLine($"Migration created: {filePath}");
    }

    // ── APPLY: run all unapplied migrations ───────────────────────────────
    public void Apply()
    {
        var applied  = GetApplied();
        var files    = Directory.GetFiles(_migrationsDir, "*.sql").OrderBy(f => f).ToList();
        var pending  = files.Where(f => !applied.Contains(Path.GetFileName(f))).ToList();

        if (!pending.Any()) { Console.WriteLine("No pending migrations."); return; }

        using var conn = Open();
        foreach (var file in pending)
        {
            var migName = Path.GetFileName(file);
            var sql     = File.ReadAllText(file);
            Console.WriteLine($"Applying: {migName}");
            new NpgsqlCommand(sql, conn).ExecuteNonQuery();

            var record = $"INSERT INTO __migrations (name) VALUES (@n)";
            var cmd    = new NpgsqlCommand(record, conn);
            cmd.Parameters.AddWithValue("@n", migName);
            cmd.ExecuteNonQuery();
            Console.WriteLine($"  ✓ Applied");
        }
    }

    // ── LIST: show all migrations and status ──────────────────────────────
    public void List()
    {
        var applied = GetApplied();
        var files   = Directory.GetFiles(_migrationsDir, "*.sql").OrderBy(f => f).ToList();

        Console.WriteLine("\n{0,-50} {1}", "Migration", "Status");
        Console.WriteLine(new string('-', 60));
        foreach (var f in files)
        {
            var name   = Path.GetFileName(f);
            var status = applied.Contains(name) ? "✓ Applied" : "✗ Pending";
            Console.WriteLine("{0,-50} {1}", name, status);
        }
        Console.WriteLine();
    }

    // ── ROLLBACK: undo the last applied migration ──────────────────────────
    public void Rollback()
    {
        var applied = GetApplied();
        if (!applied.Any()) { Console.WriteLine("Nothing to roll back."); return; }

        var last    = applied.Last();
        Console.WriteLine($"Rolling back: {last}");

        // Look for a matching .down.sql file
        var downFile = Path.Combine(_migrationsDir, last.Replace(".sql", ".down.sql"));
        if (File.Exists(downFile))
        {
            using var conn = Open();
            new NpgsqlCommand(File.ReadAllText(downFile), conn).ExecuteNonQuery();
        }
        else
        {
            Console.WriteLine("  (No .down.sql found — removing from history only)");
        }

        using var connDel = Open();
        var cmd = new NpgsqlCommand("DELETE FROM __migrations WHERE name = @n", connDel);
        cmd.Parameters.AddWithValue("@n", last);
        cmd.ExecuteNonQuery();
        Console.WriteLine($"  ✓ Rolled back: {last}");
    }

    // ── Helpers ───────────────────────────────────────────────────────────
    private HashSet<string> GetApplied()
    {
        using var conn   = Open();
        using var cmd    = new NpgsqlCommand("SELECT name FROM __migrations ORDER BY id", conn);
        using var reader = cmd.ExecuteReader();
        var result = new HashSet<string>();
        while (reader.Read()) result.Add(reader.GetString(0));
        return result;
    }

    private NpgsqlConnection Open()
    {
        var conn = new NpgsqlConnection(_connStr);
        conn.Open();
        return conn;
    }
}