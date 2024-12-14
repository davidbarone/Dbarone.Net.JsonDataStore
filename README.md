# Dbarone.Net.JsonDataStore
A simple json data store loosely based on https://github.com/ttu/json-flatfile-datastore.

## Introduction
This library is a very simple json data store which can be used for creating quick prototypes, non production database and small projects. It features the following functionality:
- data stored in a single json file
- all operations synchronous
- single threaded / single user
- CRUD operations (create, read, update, delete)
- multiple tables (known as 'collections') in the single file
- supports sequences (autonumbers)
- supports simple integrity checks (not null, unique, referential)
- automatic convertion to .NET types (POCO)
- supports strong-typed entities and dictionary types
- transaction support, including nested transactions
- supports in-memory and file-based json databases
- supports plain-text and encrypted file-based json databases

## Example
A simple example is shown below:

``` C#
public class FooBarBaz
{
    public string Value { get; set; } = default!;
}

public class Main
{
    public int CreateData() {
        // Creates in-memory data store
        var store = DataStore.Create("", false);
        var coll = store.GetCollection<FooBarBaz>();
        coll.Insert(new List<FooBarBaz> { { new FooBarBaz { Value = "foo" } }, { new FooBarBaz { Value = "bar" } } });
    }
}
```