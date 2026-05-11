using System.Reflection;
using MiniOrm.Attributes;

namespace MiniOrm.Data;

public class EntityMetadata
{
    public string TableName   { get; }
    public PropertyInfo PrimaryKey { get; }
    public List<PropertyInfo> Columns { get; }  // Only [Column]-decorated props

    public EntityMetadata(Type entityType)
    {
        // Table name from [Table] attribute
        var tableAttr = entityType.GetCustomAttribute<TableAttribute>()
            ?? throw new InvalidOperationException($"[Table] attribute missing on {entityType.Name}");
        TableName = tableAttr.Name;

        var props = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        // Only include properties that have [Column] or [PrimaryKey]
        Columns = props
            .Where(p => p.GetCustomAttribute<ColumnAttribute>() != null
                     || p.GetCustomAttribute<PrimaryKeyAttribute>() != null)
            .ToList();

        PrimaryKey = Columns.FirstOrDefault(p => p.GetCustomAttribute<PrimaryKeyAttribute>() != null)
            ?? throw new InvalidOperationException($"[PrimaryKey] attribute missing on {entityType.Name}");
    }

    public string GetColumnName(PropertyInfo prop)
    {
        return prop.GetCustomAttribute<ColumnAttribute>()?.Name ?? prop.Name.ToLower();
    }

    public string GetPrimaryKeyColumnName() => GetColumnName(PrimaryKey);
}