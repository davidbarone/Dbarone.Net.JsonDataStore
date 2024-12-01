namespace Dbarone.Net.JsonDataStore;

/// <summary>
/// Annotate on a property to denote the key column.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class KeyAttribute : Attribute
{

}