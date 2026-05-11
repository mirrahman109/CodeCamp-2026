using MiniOrm.Attributes;

namespace MiniOrm.Models;

[Table("products")]
public class Product
{
    [PrimaryKey]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = "";

    [Column("price")]
    public decimal Price { get; set; }

    [Column("description")]   // nullable
    public string? Description { get; set; }

    [Column("stock")]         // nullable int
    public int? Stock { get; set; }
}