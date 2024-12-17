namespace Dbarone.Net.JsonDataStore;

public class Constraint
{
    public string CollectionName { get; set; } = default!;
    public string AttributeName { get; set; } = default!;
    public ConstraintType ConstraintType { get; set; } = ConstraintType.NONE;
    public string? ReferenceCollectionName { get; set; } = default!;
    public string? ReferenceAttributeName { get; set; } = default!;
}