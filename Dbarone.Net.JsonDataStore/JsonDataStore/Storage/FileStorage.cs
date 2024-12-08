using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Dbarone.Net.JsonDataStore;

/// <summary>
/// Storage class. Json is always stored as UTF8. 
/// </summary>
public class FileStorage : Storage
{
    string _path;
    string _password;
    FileMode _mode;

    public FileStorage(string path, FileMode mode, string password) : base()
    {
        this._path = path;
        this._mode = mode;
        this._password = password;
    }

    protected override Stream CreateStream(StreamMode mode)
    {

        var fs = new FileStream(_path, _mode, FileAccess.ReadWrite, FileShare.None);
        if (string.IsNullOrEmpty(_password))
        {
            return fs;
        }
        else
        {
            if (mode == StreamMode.READ)
            {
                return fs.ToCryptoStream(_password, System.Security.Cryptography.CryptoStreamMode.Read);
            }
            else
            {
                return fs.ToCryptoStream(_password, System.Security.Cryptography.CryptoStreamMode.Write);
            }
        }
    }
}