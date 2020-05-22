# ElevenDb
ElevenDb is an extremely-simple key-value database consisting of eleven different components. 
It is in alpha state, so it is not recommended to use it with valuable data. 

## Features:
 - ElevenDb uses native strings for keys and values.
 - An index for keys is created using binary tree.
 - Supported operations are : Read, Write, Delete, ReadAll, WriteAll and Iterate
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
string value = database.Read("key").Data;
database.Close();
```

Each method in db returns results for checking. Read method also returns a Result object, which consists of a message and data. Therefore, you could use methods like below, you could use result.Data (given above), or you could just use ReadKey method  

```csharp
DB database = new DB(path-to-db-file);
database.Open();
Result<string> result = database.Read("key");
if(result.Message = ResultType.Success)
{
    string value = result.Data;
}
string directValue = database.ReadKey("key");
database.Close();
```

