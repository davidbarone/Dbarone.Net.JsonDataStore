using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Dbarone.Net.JsonDataStore;

public class DocumentCollection<T> : IDocumentCollection<T>
{
    private readonly string _jsonPath;

    private readonly List<T> _data;

    public DocumentCollection(string jsonPath, List<T> data)
    {
        this._jsonPath = jsonPath;
        this._data = data;
    }

    public int Count => _data.Count();

    public int Delete(Predicate<T> where)
    {
        var rows = _data.RemoveAll(where);
        return rows;
    }

    public IEnumerable<T> Find(Predicate<T> where)
    {
        return _data.Where(i => where(i));
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
        _data.Add(item);
        return 1;
    }

    public int Insert(IEnumerable<T> items)
    {
        _data.AddRange(items);
        return items.Count();
    }

    public int Update(Predicate<T> where, Func<T, T> transform)
    {
        var data = _data.Where(i => where(i)).ToList();
        var rowsAffected = data.Count();
        data.ForEach(i => i = transform(i));
        return rowsAffected;
    }

    public int Upsert(Predicate<T> where, T item)
    {
        var data = _data.Where(i => where(i)).ToList();
        var rowsAffected = data.Count();
        if (rowsAffected > 0)
        {
            data.ForEach(i => i = item);
        }
        else
        {
            _data.Add(item);
        }
        return 1;
    }
}
