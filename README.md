<p align="center">
  <img alt="ElevenDb Logo" src="Eleven.png" width="100px" />
  <h1 align="center">ElevenDb</h1>
</p>
ElevenDb is an extremely-simple key-value database consisting of eleven different components. 

## Features
 - ElevenDb uses native strings for keys and values.
 - An index for keys is created using binary tree.
 - Supported operations are : Read, Write, Delete, ReadAll, WriteBatch and Iterate
 - ElevenDb is created for storing small and simple key-value data.
 - Because of that, records are stored as 32 bit int values, so its max record capacity is 2147483647 records.

## Usage
To write a record to database instance, use:

```csharp
DB database = new DB(path-to-db-file);
database.Open();
database.Write("key","value");
database.Close();
```

Creating DB instance will create the db file, if it doesn't exist. 

```csharp
DB database = new DB(path-to-db-file);
```

To read a record:

```csharp
DB database = new DB(path-to-db-file);
database.Open();
string value = database.Read("key").Value;
database.Close();
```

Each method in db returns results for checking. Read method also returns a Result object, which consists of a message and data. Therefore, you could use methods like below, you could use result.Data (given above), or you could just use ReadKey method:

```csharp
DB database = new DB(path-to-db-file);
database.Open();
Result<string> result = database.Read("key");
if(result.IsSuccess)
{
    string value = result.Value;
}
string directValue = database.ReadKey("key");
database.Close();
```

ElevenDb uses Write method to update record, using existing key string.

To delete a record:

```csharp
Result result = database.Delete("key");
```

To read all records as a list of key value pairs:

```csharp
Result<List<KeyValuePair<string, string>>> result = database.ReadAll();
if (result.IsSuccess)
    foreach (var kvp in result.Value)
        Console.WriteLine(kvp.Key + "-" + kvp.Value);
```

To write multiple records at once (random keys and values in the example):

```csharp
List<KeyValuePair<string, string>> kvpList = new List<KeyValuePair<string, string>>();
for (int i = 0; i < testSize; i++)
{
    int rnd = new Random().Next();
    kvpList.Add(new KeyValuePair<string, string>("Key" + rnd, "Value" + rnd));
}
Result writeResult = database.WriteBatch(kvpList);
```

To iterate over all values you could get an iterator. Iterator has a property named "HasRecord" to and a method named GetNext() which could be used as in example below:

```csharp
Iterator iterator = database.GetIterator();
while (iterator.HasRecord)
{
    Result<string> result = iterator.GetNext();
    Console.WriteLine(result.Value);
}
```
