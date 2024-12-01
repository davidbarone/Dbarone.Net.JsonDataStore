using System.Reflection;

public class BaseTests
{
    protected string GetJsonString(string resourceName)
    {
        var stream = GetJsonStream(resourceName);
        StreamReader sr = new StreamReader(stream);
        return sr.ReadToEnd();
    }
    protected Stream GetJsonStream(string resourceName)
    {
        var assembly = this.GetType().GetTypeInfo().Assembly;
        var path = GetResources().First(r => r.Contains(resourceName));
        Stream stream = assembly.GetManifestResourceStream(path)!;
        return stream;
    }

    /// <summary>
    /// Gets a list of all the resources available.
    /// </summary>
    /// <returns>A string array of all the dataset names.</returns>
    public string[] GetResources()
    {
        var assembly = this.GetType().GetTypeInfo().Assembly;
        return assembly.GetManifestResourceNames();
    }
}