using MiniOrm.Migrations.Commands;

var connStr = Environment.GetEnvironmentVariable("MINIORM_CONN")
    ?? throw new Exception("Set MINIORM_CONN environment variable first.");

var runner = new MigrationRunner(connStr);

var command = args.ElementAtOrDefault(0)?.ToLower();
var param   = args.ElementAtOrDefault(1);

switch (command)
{
    case "add"      when param != null: runner.Add(param);   break;
    case "apply":                       runner.Apply();       break;
    case "list":                        runner.List();        break;
    case "rollback":                    runner.Rollback();    break;
    default:
        Console.WriteLine("""
            MiniOrm Migration CLI
            Usage:
              add <MigrationName>   — Scaffold a new migration file
              apply                 — Apply all pending migrations
              list                  — List all migrations and their status
              rollback              — Undo the last applied migration
            """);
        break;
}