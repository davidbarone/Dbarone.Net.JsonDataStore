using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Dbarone.Net.JsonDataStore;

public class DocumentCollection<T> : IDocumentCollection<T>
{
    private readonly string _jsonPath;

    private readonly Lazy<List<T>> _data;

    private Action<IDocumentCollection<T>> _modificationAction;

    public DocumentCollection(string jsonPath, Lazy<List<T>> data, Action<IDocumentCollection<T>> modificationAction)
    {
        this._jsonPath = jsonPath;
        this._data = data;
        this._modificationAction = modificationAction;
    }

    public List<T> AsList => _data.Value;

    public int Count => _data.Value.Count();

    public int Delete(Predicate<T> where)
    {
        var rows = _data.Value.RemoveAll(where);
        return rows;
    }

    public IEnumerable<T> Find(Predicate<T> where)
    {
        return _data.Value.Where(i => where(i));
    }

    public IEnumerable<T> FullTextSearch(string text, bool caseSensitive = false)
    {
        throw new NotImplementedException();
    }

    public int GetNextId()
    {
        throw new NotImplementedException();
    }

    public int Insert(T item)
    {
        _data.Value.Add(item);
        _modificationAction(this);
        return 1;
    }

    public int Insert(IEnumerable<T> items)
    {
        _data.Value.AddRange(items);
        _modificationAction(this);
        return items.Count();
    }

    public int Update(Predicate<T> where, Func<T, T> transform)
    {
        var data = _data.Value.Where(i => where(i)).ToList();
        var rowsAffected = data.Count();
        data.ForEach(i => i = transform(i));
        return rowsAffected;
    }

    public int Upsert(Predicate<T> where, T item)
    {
        var data = _data.Value.Where(i => where(i)).ToList();
        var rowsAffected = data.Count();
        if (rowsAffected > 0)
        {
            data.ForEach(i => i = item);
        }
        else
        {
            _data.Value.Add(item);
        }
        return 1;
    }
}
