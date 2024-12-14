namespace Dbarone.Net.JsonDataStore;

public class Constraint
{
    public string CollectionName { get; set; } = default!;
    public string AttributeName { get; set; } = default!;
    public bool? IsNotNull { get; set; }
    public bool? IsUnique { get; set; }
    public string? ForeignKeyCollectionName { get; set; } = default!;
    public string? ForeignKeyAttributeName { get; set; } = default!;
}