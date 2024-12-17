namespace Dbarone.Net.JsonDataStore;

[Flags]
public enum ConstraintType
{
    NONE = 0,
    REQUIRED = 1,
    UNIQUE = 2,
    REFERENCE = 4
}