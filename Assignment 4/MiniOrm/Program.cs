using MiniOrm.Data;
using MiniOrm.Models;
using Npgsql;

// ── Step 1: Connect ───────────────────────────────────────────────────────
var connStr = Environment.GetEnvironmentVariable("MINIORM_CONN")
    ?? throw new Exception("MINIORM_CONN environment variable not set.");

Console.WriteLine("=== MiniOrm Demo ===\n");
Console.WriteLine("Step 1: Opening connection to PostgreSQL...");
using var conn = new NpgsqlConnection(connStr);
conn.Open();
Console.WriteLine($"  Connected to: {conn.Host}/{conn.Database}\n");

// ── Step 2: Create tables (run migrations first in real usage) ─────────────
Console.WriteLine("Step 2: Ensuring tables exist...");
var createProducts = """
    CREATE TABLE IF NOT EXISTS products (
        id          SERIAL PRIMARY KEY,
        name        TEXT NOT NULL,
        price       NUMERIC NOT NULL,
        description TEXT,
        stock       INTEGER
    );
    """;
var createOrders = """
    CREATE TABLE IF NOT EXISTS orders (
        id          SERIAL PRIMARY KEY,
        product_id  INTEGER NOT NULL,
        quantity    INTEGER NOT NULL,
        ordered_at  TIMESTAMP NOT NULL,
        note        TEXT
    );
    """;
new NpgsqlCommand(createProducts, conn).ExecuteNonQuery();
new NpgsqlCommand(createOrders,   conn).ExecuteNonQuery();
Console.WriteLine("  Tables ready.\n");

// ── Step 3: INSERT ─────────────────────────────────────────────────────────
Console.WriteLine("Step 3: Inserting products (including nullable fields)...");
var products = new DbSet<Product>(conn);

products.Insert(new Product { Name = "Laptop",    Price = 999.99m,  Description = "High-end laptop", Stock = 10 });
products.Insert(new Product { Name = "Mouse",     Price = 29.99m,   Description = null,              Stock = null });
products.Insert(new Product { Name = "Keyboard",  Price = 59.99m,   Description = "Mechanical",      Stock = 50 });
Console.WriteLine("  3 products inserted (one with null description & stock).\n");

// ── Step 4: SELECT ALL ─────────────────────────────────────────────────────
Console.WriteLine("Step 4: Reading all products...");
var allProducts = products.GetAll();
foreach (var p in allProducts)
    Console.WriteLine($"  [{p.Id}] {p.Name} — ${p.Price} | Desc: {p.Description ?? "NULL"} | Stock: {p.Stock?.ToString() ?? "NULL"}");
Console.WriteLine();

// ── Step 5: UPDATE ─────────────────────────────────────────────────────────
Console.WriteLine("Step 5: Updating Laptop price...");
var laptop = allProducts.First(p => p.Name == "Laptop");
laptop.Price = 849.99m;
laptop.Stock = 8;
products.Update(laptop);
var updated = products.GetById(laptop.Id);
Console.WriteLine($"  Updated: [{updated!.Id}] {updated.Name} — ${updated.Price} | Stock: {updated.Stock}\n");

// ── Step 6: DELETE ─────────────────────────────────────────────────────────
Console.WriteLine("Step 6: Deleting Mouse...");
var mouse = allProducts.First(p => p.Name == "Mouse");
products.Delete(mouse.Id);
Console.WriteLine($"  Deleted product ID {mouse.Id}");
Console.WriteLine($"  Products remaining: {products.GetAll().Count}\n");

// ── Step 7: Orders with nullable Note ─────────────────────────────────────
Console.WriteLine("Step 7: Inserting orders...");
var orders = new DbSet<Order>(conn);
orders.Insert(new Order { ProductId = laptop.Id, Quantity = 2, OrderedAt = DateTime.UtcNow, Note = "Rush order" });
orders.Insert(new Order { ProductId = laptop.Id, Quantity = 1, OrderedAt = DateTime.UtcNow, Note = null });

foreach (var o in orders.GetAll())
    Console.WriteLine($"  Order #{o.Id}: Product {o.ProductId} x{o.Quantity} | Note: {o.Note ?? "NULL"}");

Console.WriteLine("\n=== Demo Complete ===");