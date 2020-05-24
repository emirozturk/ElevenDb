using ElevenDb;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace ElevenTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void WriteTest()
        {
            DB database = new DB(@"C:\Users\emiro\Desktop\Test\test.db", new Options(IsLoggingActive: true));
            Result openResult = database.Open();
            if (openResult.IsSuccess)
            {
                Result writeResult = database.Write("Key", "Value");
                database.Close();
                if (writeResult.IsSuccess)
                {
                    Assert.Pass();
                }
                else
                {
                    Assert.Fail();
                }
            }
            else
            {
                Assert.Fail();
            }
        }
        [Test]
        public void MultiWriteTest()
        {
            int testSize = 1000;
            DB database = new DB(@"C:\Users\emiro\Desktop\Test\test.db", new Options(IsLoggingActive: true));
            Result openResult = database.Open();
            if (openResult.IsSuccess)
            {
                for (int i = 0; i < testSize; i++)
                {
                    int rnd = new Random().Next();
                    Result writeResult = database.Write("Key" + rnd, "Value" + rnd);
                    if (!writeResult.IsSuccess)
                    {
                        Assert.Fail();
                    }
                }
                database.Close();
                Assert.Pass();
            }
        }
        [Test]
        public void OverwriteTest()
        {
            DB database = new DB(@"C:\Users\emiro\Desktop\Test\test.db", new Options(IsLoggingActive: true));
            Result openResult = database.Open();
            if (openResult.IsSuccess)
            {
                Result writeResult = database.Write("Key", "Value");
                database.Close();
                if (writeResult.IsSuccess)
                {
                    Assert.Pass();
                }
                else
                {
                    Assert.Fail();
                }
            }
            else
            {
                Assert.Fail();
            }
        }
        [Test]
        public void ReadTest()
        {
            DB database = new DB(@"C:\Users\emiro\Desktop\Test\test.db", new Options(IsLoggingActive: true));
            Result openResult = database.Open();
            if (openResult.IsSuccess)
            {
                Result<string> readResult = database.Read("Key");
                database.Close();
                if (readResult.IsSuccess)
                {
                    Console.WriteLine(readResult.Value);
                    Assert.Pass();
                }
                else
                {
                    Assert.Fail();
                }
            }
            else
            {
                Assert.Fail();
            }
        }
        [Test]
        public void MultiReadTest()
        {
            DB database = new DB(@"C:\Users\emiro\Desktop\Test\test.db", new Options(IsLoggingActive: true));
            Result openResult = database.Open();
            database.Close();
            if (openResult.IsSuccess)
            {
                for (int i = 0; i < 10000; i++)
                {
                    Result<string> readResult = database.Read("Key" + new Random().Next());
                    if (!readResult.IsSuccess)
                    {
                        Assert.Fail();
                    }
                    Console.WriteLine(readResult.Value);
                }
                Assert.Pass();
            }
        }
        [Test]
        public void DeleteTest()
        {
            DB database = new DB(@"C:\Users\emiro\Desktop\Test\test.db", new Options(IsLoggingActive: true));
            Result openResult = database.Open();
            if (openResult.IsSuccess)
            {
                Result result = database.Delete("Key" + new Random().Next(0, 1000));
                database.Close();
                if (result.IsSuccess)
                {
                    Assert.Pass();
                }
                else
                {
                    Assert.Fail();
                }
            }
            else
            {
                Assert.Fail();
            }

        }
        [Test]
        public void IterateTest()
        {
            DB database = new DB(@"C:\Users\emiro\Desktop\Test\test.db", new Options(IsLoggingActive: true));
            Result openResult = database.Open();
            if (openResult.IsSuccess)
            {
                Iterator iterator = database.GetIterator();
                while (iterator.HasRecord)
                {
                    Result<string> result = iterator.GetNext();
                    Console.WriteLine(result.Value);
                }
                database.Close();
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }

        }
        [Test]
        public void ReadAllTest()
        {
            DB database = new DB(@"C:\Users\emiro\Desktop\Test\test.db", new Options(IsLoggingActive: true));
            Result openResult = database.Open();
            if (openResult.IsSuccess)
            {
                Result<List<KeyValuePair<string, string>>> result = database.ReadAll();
                database.Close();
                if (result.IsSuccess)
                {
                    Console.WriteLine(result.Value.Count);
                    foreach (var kvp in result.Value)
                    {
                        Console.WriteLine(kvp.Key + "-" + kvp.Value);
                    }
                }

                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }
        }
        [Test]
        public void WriteBatchTest()
        {
            int testSize = 1000;
            DB database = new DB(@"C:\Users\emiro\Desktop\Test\test.db", new Options(IsLoggingActive: true));
            Result openResult = database.Open();
            if (openResult.IsSuccess)
            {
                List<KeyValuePair<string, string>> kvpList = new List<KeyValuePair<string, string>>();
                for (int i = 0; i < testSize; i++)
                {
                    int rnd = new Random().Next();
                    kvpList.Add(new KeyValuePair<string, string>("Key" + rnd, "Value" + rnd));
                }
                Result writeResult = database.WriteBatch(kvpList);
                if (!writeResult.IsSuccess)
                {
                    Assert.Fail();
                }
                database.Close();
                Assert.Pass();
            }
        }
        public void RepairTest()
        {
            DB database = new DB(@"C:\Users\emiro\Desktop\Test\test.db", new Options(IsLoggingActive: true));
            Result result = database.RepairDb();
            if (result.IsSuccess) Assert.Pass();
            else Assert.Fail();
        }
    }
}