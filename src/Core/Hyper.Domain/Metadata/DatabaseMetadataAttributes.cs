namespace Hyper.Domain.Metadata;

/// <summary>
/// Maps a CLR type or member to its database name without coupling the shared
/// domain model to the AdminPanel metadata implementation.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property)]
public class DbMapAttribute(string? dbMap = null) : Attribute
{
    public string? DBName { get; set; } = dbMap;
    public string? OldDbName { get; set; }
}

public sealed class EntityFieldMapAttribute(string fieldId) : DbMapAttribute(fieldId)
{
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property)]
public sealed class OldDbMapAttribute(string oldDbName) : Attribute
{
    public string OldDbName { get; set; } = oldDbName;
}

/// <summary>
/// Describes the key columns used by a navigation property. The AdminPanel
/// translates this neutral metadata into its own runtime model when it loads
/// the legacy UI definitions.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
public sealed class AssociationMapAttribute(string sourceField, string destinationField) : Attribute
{
    public string MyField { get; } = sourceField.Trim();
    public string ObjectField { get; } = destinationField.Trim();
    public bool Bitmask { get; init; }
}
