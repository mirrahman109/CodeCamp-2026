# MiniOrm - Lightweight ORM for PostgreSQL

A minimal Object-Relational Mapping (ORM) framework for .NET demonstrating core ORM concepts: attribute-based entity mapping, type mapping, and database operations.

## Quick Start

### 1. PostgreSQL Setup

#### Windows Installation
1. Download PostgreSQL from https://www.postgresql.org/download/windows/
2. Run the installer and follow the setup wizard
3. **Remember the password** you set for the `postgres` user
4. Default port is `5432`
5. Ensure PostgreSQL service is running

#### Verify Installation
```bash
psql --version
```

#### Create Database
```bash
# Login to PostgreSQL as admin
psql -U postgres

# At the psql prompt, create a database
CREATE DATABASE miniorm;

# List databases to verify
\l

# Exit
\q
```

### 2. Set Environment Variable

The `MINIORM_CONN` environment variable stores your PostgreSQL connection string.

**Windows PowerShell:**
```powershell
$env:MINIORM_CONN = "Host=localhost;Database=miniorm;Username=postgres;Password=your_password;Port=5432"
```

**Windows Command Prompt (cmd):**
```batch
set MINIORM_CONN=Host=localhost;Database=miniorm;Username=postgres;Password=your_password;Port=5432
```

**Verify it's set:**
```powershell
# PowerShell
$env:MINIORM_CONN

# cmd
echo %MINIORM_CONN%
```

**Connection String Format:**
- `Host` — PostgreSQL server (usually `localhost` for local development)
- `Database` — Database name (we created `miniorm`)
- `Username` — PostgreSQL user (default is `postgres`)
- `Password` — Password you set during installation
- `Port` — PostgreSQL port (default is `5432`)

### 3. Run Migrations

Migrations create and manage your database schema.

```bash
cd MiniOrm.Migrations

# Create a new migration file
dotnet run -- add CreateProductsTable

# Apply all pending migrations
dotnet run -- apply

# List migration status
dotnet run -- list

# Rollback the last applied migration
dotnet run -- rollback
```

### 4. Run the Demo

```bash
cd MiniOrm

# Run the demo (requires MINIORM_CONN environment variable set)
dotnet run
```

**Output shows:**
- ✓ Connection to PostgreSQL
- ✓ Table creation (if needed)
- ✓ INSERT operations (3 products, 2 orders)
- ✓ SELECT operations (retrieve all records)
- ✓ UPDATE operations (modify existing records)
- ✓ DELETE operations (remove records)

---

## Architecture

### Project Structure

```
MiniOrm/                          # Main ORM library
├── Attributes/                   # Entity mapping decorators
│   ├── TableAttribute            # [Table] - maps class to database table
│   ├── ColumnAttribute           # [Column] - maps property to column
│   └── PrimaryKeyAttribute       # [PrimaryKey] - marks primary key
├── Data/                         # Core ORM logic
│   ├── DbSet<T>                  # Generic repository for CRUD operations
│   ├── DbContext                 # Base class for database context
│   ├── EntityMetadata            # Reflection-based entity analysis
│   └── TypeMapper                # C# type → PostgreSQL type mapping
├── Models/                       # Entity classes
│   ├── Product                   # Example: products table
│   └── Order                     # Example: orders table
└── Program.cs                    # Demo application

MiniOrm.Migrations/               # Migration management tool
├── Commands/
│   └── MigrationRunner           # Migration CLI: add, apply, list, rollback
├── Migrations/                   # SQL migration files (auto-created)
└── Program.cs                    # Migration CLI entry point
```

### Core Components

#### 1. **Attributes** — Entity Configuration

Define how your C# classes map to database tables.

```csharp
[Table("products")]                    // Maps class to table name
public class Product
{
    [PrimaryKey]                       // Marks as primary key
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]                   // Maps property to column name
    public string Name { get; set; } = "";

    [Column("price")]
    public decimal Price { get; set; }

    [Column("description")]            // Can be nullable
    public string? Description { get; set; }

    [Column("stock")]                  // Nullable int
    public int? Stock { get; set; }
}
```

**Attributes:**
- `[Table("name")]` — Class-level: specifies database table name
- `[Column("name")]` — Property-level: specifies column name (can differ from property name)
- `[PrimaryKey]` — Property-level: marks as primary key (exactly one per entity required)

#### 2. **Type Mapping** — C# to PostgreSQL

The `TypeMapper` class converts C# types to PostgreSQL types automatically.

```csharp
public static class TypeMapper
{
    public static string ToPostgresType(Type clrType)
    {
        // Unwrap nullable: int? -> int
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
            _ => throw new NotSupportedException(...)
        };
    }
}
```

**Type Mappings:**
| C# Type | PostgreSQL Type | Example |
|---------|-----------------|---------|
| `int` | `INTEGER` | Product Id |
| `long` | `BIGINT` | Large counters |
| `short` | `SMALLINT` | Small integers |
| `bool` | `BOOLEAN` | Status flags |
| `decimal` | `NUMERIC` | Prices, amounts |
| `double` | `DOUBLE PRECISION` | Scientific data |
| `float` | `REAL` | Lightweight floats |
| `string` | `TEXT` | Names, descriptions |
| `DateTime` | `TIMESTAMP` | Timestamps |
| `Guid` | `UUID` | Unique identifiers |
| `T?` (nullable) | Same as `T` | Nullable types |

**Key Feature:** The mapper automatically handles **nullable types** by unwrapping them. For example:
- `int?` → `INTEGER` (column allows NULL)
- `string?` → `TEXT` (column allows NULL)
- `decimal` → `NUMERIC NOT NULL` (column doesn't allow NULL)

#### 3. **Attribute Filtering** — Reflection-Based Property Selection

The `EntityMetadata` class uses reflection to identify which properties map to database columns.

```csharp
public class EntityMetadata
{
    public EntityMetadata(Type entityType)
    {
        // Only include properties decorated with [Column] or [PrimaryKey]
        Columns = entityType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetCustomAttribute<ColumnAttribute>() != null
                     || p.GetCustomAttribute<PrimaryKeyAttribute>() != null)
            .ToList();

        PrimaryKey = Columns
            .FirstOrDefault(p => p.GetCustomAttribute<PrimaryKeyAttribute>() != null)
            ?? throw new InvalidOperationException(...);
    }
}
```

**How It Works:**

1. **Reflection Scan** — Uses `GetProperties()` to examine all public instance properties
2. **Attribute Check** — Filters properties that have `[Column]` or `[PrimaryKey]` attributes
3. **Primary Key Detection** — Finds the property with `[PrimaryKey]` (required, must be unique)
4. **Column Mapping** — Uses `[Column("name")]` to get database column name, or falls back to property name

**Example:**

```csharp
[Table("products")]
public class Product
{
    [PrimaryKey]
    [Column("id")]
    public int Id { get; set; }           // ✓ Included (has [PrimaryKey])

    [Column("name")]
    public string Name { get; set; }      // ✓ Included (has [Column])

    [Column("price")]
    public decimal Price { get; set; }    // ✓ Included (has [Column])

    public string Ignored { get; set; }   // ✗ Ignored (no attribute)
}
```

**Result:**
- Only `Id`, `Name`, `Price` are mapped to database columns
- `Ignored` property is skipped by the ORM
- This allows you to have computed properties or helper fields that don't persist

#### 4. **DbSet<T>** — Repository Pattern

Generic data access class for CRUD operations.

```csharp
public class DbSet<T> where T : class, new()
{
    // CREATE
    public void Insert(T entity)
    
    // READ
    public List<T> GetAll()
    public T? GetById(object id)
    
    // UPDATE
    public void Update(T entity)
    
    // DELETE
    public void Delete(object id)
}
```

**Usage Example:**

```csharp
using var conn = new NpgsqlConnection(connStr);
conn.Open();

var products = new DbSet<Product>(conn);

// INSERT
products.Insert(new Product { Name = "Laptop", Price = 999.99m });

// SELECT ALL
foreach (var p in products.GetAll())
    Console.WriteLine($"{p.Name}: ${p.Price}");

// SELECT BY ID
var product = products.GetById(1);

// UPDATE
product.Price = 899.99m;
products.Update(product);

// DELETE
products.Delete(1);
```

---

## Data Types and Nullability

### Non-Nullable Properties

```csharp
public decimal Price { get; set; }  // NOT NULL in database
```
- PostgreSQL: `NUMERIC NOT NULL`
- Must always have a value
- Constraints ensure data integrity

### Nullable Properties

```csharp
public string? Description { get; set; }  // NULLABLE in database
public int? Stock { get; set; }           // NULLABLE in database
```
- PostgreSQL: `TEXT` and `INTEGER` (without NOT NULL)
- Can be null
- Use nullable reference types (`string?`) and nullable value types (`int?`)

### Handling Nulls in Code

```csharp
var product = products.GetById(1);

// Safe null handling
Console.WriteLine(product?.Description ?? "No description");
Console.WriteLine(product?.Stock?.ToString() ?? "No stock");
```

---

## Migration System

### Migration CLI Commands

```bash
# Add a new migration
dotnet run -- add MigrationName

# Apply all pending migrations
dotnet run -- apply

# Show migration status
dotnet run -- list

# Undo last migration
dotnet run -- rollback
```

### Migration Files

Migrations are SQL files with timestamp prefixes: `20260512143022_MigrationName.sql`

**Example migration file:**
```sql
-- Migration: CreateProductsTable
-- Created:   2026-05-12T14:30:22.1234567Z

CREATE TABLE products (
    id          SERIAL PRIMARY KEY,
    name        TEXT NOT NULL,
    price       NUMERIC NOT NULL,
    description TEXT,
    stock       INTEGER
);
```

### Rollback Files

To support rollback, create `.down.sql` files:

**20260512143022_CreateProductsTable.down.sql:**
```sql
DROP TABLE products;
```

---

## Complete Example: Adding a New Entity

### 1. Create the Model

```csharp
// Models/Customer.cs
using MiniOrm.Attributes;

[Table("customers")]
public class Customer
{
    [PrimaryKey]
    [Column("id")]
    public int Id { get; set; }

    [Column("email")]
    public string Email { get; set; } = "";

    [Column("phone")]
    public string? Phone { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
```

### 2. Create Migration

```bash
cd MiniOrm.Migrations
dotnet run -- add CreateCustomersTable
```

### 3. Write Migration SQL

Edit `Migrations/[timestamp]_CreateCustomersTable.sql`:

```sql
CREATE TABLE customers (
    id         SERIAL PRIMARY KEY,
    email      TEXT NOT NULL,
    phone      TEXT,
    created_at TIMESTAMP NOT NULL
);
```

### 4. Apply Migration

```bash
dotnet run -- apply
```

### 5. Use in Code

```csharp
var customers = new DbSet<Customer>(conn);

customers.Insert(new Customer 
{ 
    Email = "john@example.com",
    Phone = "555-1234",
    CreatedAt = DateTime.UtcNow
});

var all = customers.GetAll();
```

---

## Troubleshooting

### "MINIORM_CONN environment variable not set"
- **Fix:** Set the environment variable (see section 2 above)
- **Check:** Run `$env:MINIORM_CONN` (PowerShell) or `echo %MINIORM_CONN%` (cmd)

### "Format of the initialization string does not conform to specification"
- **Cause:** Connection string is malformed
- **Fix:** Verify format: `Host=localhost;Database=miniorm;Username=postgres;Password=xxx;Port=5432`
- **Common issues:**
  - Passwords with special characters need escaping
  - Missing semicolons between parameters
  - Invalid hostname or port

### "Database 'miniorm' does not exist"
- **Fix:** Create the database:
  ```bash
  psql -U postgres -c "CREATE DATABASE miniorm;"
  ```

### "[Table] attribute missing on [EntityName]"
- **Fix:** Add `[Table("table_name")]` to your entity class

### "[PrimaryKey] attribute missing on [EntityName]"
- **Fix:** Mark exactly one property with `[PrimaryKey]` attribute

### "No Postgres mapping for CLR type 'XXX'"
- **Cause:** You used an unsupported type
- **Fix:** Use only supported types listed in the Type Mapping table above
- **Alternative:** Convert to a supported type (e.g., `DateTime` instead of `TimeSpan`)

---

## Key Concepts

### ORM (Object-Relational Mapping)
Bridges the gap between object-oriented code and relational databases by automatically converting between C# objects and SQL records.

### Reflection
Used to inspect entity classes at runtime, discovering attributes and properties without hardcoding column mappings.

### Attribute-Based Configuration
Decorators (`[Table]`, `[Column]`, `[PrimaryKey]`) specify how C# properties map to database columns.

### Type Mapping
Automatic conversion between C# types and PostgreSQL types ensures proper data representation.

### Nullable Types
Uses C# nullable reference types (`string?`) and nullable value types (`int?`) to represent columns that allow NULL.

---

## Project Status

This is a **learning project** demonstrating:
- ✓ Attribute-based entity mapping
- ✓ Reflection-based metadata extraction
- ✓ Type mapping (C# ↔ PostgreSQL)
- ✓ Generic repository pattern
- ✓ CRUD operations
- ✓ Migration management
- ✓ Nullable field handling

**Not intended for production use.** For production applications, use mature ORMs like **Entity Framework Core** or **Dapper**.

---

## References

- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [Npgsql - .NET PostgreSQL Data Provider](https://www.npgsql.org/)
- [C# Reflection](https://docs.microsoft.com/en-us/dotnet/fundamentals/reflection/reflection)
- [C# Attributes](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/attributes/)
