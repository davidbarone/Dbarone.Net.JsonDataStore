namespace Dbarone.Net.JsonDataStore;

public class ConstraintException : Exception
{
    public ConstraintException(string message) : base(message) { }
    public ConstraintException() : base() { }
}