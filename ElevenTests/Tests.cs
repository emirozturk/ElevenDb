using NUnit.Framework;
using ElevenDb;
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
            DB database = new DB(@"C:\Users\emiro\Desktop\Test\test.db");
            Result<string> openResult = database.Open();
            if (openResult.Message == ResultType.Success)
            {
                Result<string> writeResult = database.Write("Key", "Value");
                if (writeResult.Message == ResultType.Success)
                    Assert.Pass();
                else
                    Assert.Fail();
            }
            else
                Assert.Fail();
            database.Close();
        }
        [Test]
        public void MultiWriteTest()
        {
            int testSize = 1000;
            DB database = new DB(@"C:\Users\emiro\Desktop\Test\test.db");
            Result<string> openResult = database.Open();
            if (openResult.Message == ResultType.Success)
            {
                for (int i = 0; i < testSize; i++)
                {
                    int rnd = new Random().Next(0, testSize);
                    Result<string> writeResult = database.Write("Key" + rnd, "Value" + rnd);
                    if (writeResult.Message != ResultType.Success && writeResult.Message != ResultType.Overwritten) Assert.Fail();
                }
                database.Close();
                Assert.Pass();
            }
        }
        [Test]
        public void OverwriteTest()
        {
            DB database = new DB(@"C:\Users\emiro\Desktop\Test\test.db");
            Result<string> openResult = database.Open();
            if (openResult.Message == ResultType.Success)
            {
                Result<string> writeResult = database.Write("Key", "Value");
                if (writeResult.Message == ResultType.Overwritten)
                    Assert.Pass();
                else
                    Assert.Fail();
            }
            else
                Assert.Fail();
            database.Close();
        }
        [Test]
        public void ReadTest()
        {
            DB database = new DB(@"C:\Users\emiro\Desktop\Test\test.db");
            Result<string> openResult = database.Open();
            if (openResult.Message == ResultType.Success)
            {
                Result<string> readResult = database.Read("Key" + 215);
                if (readResult.Message == ResultType.Success || readResult.Message == ResultType.NotFound)
                {
                    Console.WriteLine(readResult.Data);
                    Assert.Pass();
                }
                else
                    Assert.Fail();
            }
            else
                Assert.Fail();
            database.Close();
        }
        [Test]
        public void MultiReadTest()
        {
            DB database = new DB(@"C:\Users\emiro\Desktop\Test\test.db");
            Result<string> openResult = database.Open();
            if (openResult.Message == ResultType.Success)
            {
                for (int i = 0; i < 10000; i++)
                {
                    Result<string> readResult = database.Read("Key" + new Random().Next(0, 1000));
                    if (readResult.Message != ResultType.Success && readResult.Message != ResultType.NotFound)
                        Assert.Fail();
                }
                Assert.Pass();
                database.Close();
            }
        }
        [Test]
        public void DeleteTest()
        {
            DB database = new DB(@"C:\Users\emiro\Desktop\Test\test.db");
            Result<string> openResult = database.Open();
            if (openResult.Message == ResultType.Success)
            {
                Result<string> writeResult = database.Delete("Key" + new Random().Next(0, 1000));
                if (writeResult.Message == ResultType.Success)
                    Assert.Pass();
                else
                    Assert.Fail();
            }
            else
                Assert.Fail();
            database.Close();
        }
        [Test]
        public void IterateTest()
        {
            DB database = new DB(@"C:\Users\emiro\Desktop\Test\test.db");
            Result<string> openResult = database.Open();
            if (openResult.Message == ResultType.Success)
            {
                Iterator iterator = database.GetIterator();
                while (iterator.HasRecord)
                {
                    Result<string> result = iterator.GetNext();
                    Console.WriteLine(result.Data);
                }
                Assert.Pass();
            }
            else
                Assert.Fail();
            database.Close();
        }
        [Test]
        public void ReadAllTest()
        {
            DB database = new DB(@"C:\Users\emiro\Desktop\Test\test.db");
            Result<string> openResult = database.Open();
            if (openResult.Message == ResultType.Success)
            {
                Result<List<KeyValuePair<string, string>>> result = database.ReadAll();
                if (result.Message == ResultType.Success)
                    foreach (var kvp in result.Data)
                        Console.WriteLine(kvp.Key + "-" + kvp.Value);
                Assert.Pass();
            }
            else
                Assert.Fail();
            database.Close();
        }
        [Test]
        public void WriteBatchTest()
        {
            int testSize = 1000;
            DB database = new DB(@"C:\Users\emiro\Desktop\Test\test.db");
            Result<string> openResult = database.Open();
            if (openResult.Message == ResultType.Success)
            {
                List<KeyValuePair<string, string>> kvpList = new List<KeyValuePair<string, string>>();
                for (int i = 0; i < testSize; i++)
                {
                    int rnd = new Random().Next(0, testSize);
                    kvpList.Add("Key" + rnd, "Value" + rnd);
                }
                Result<string> writeResult = database.WriteBatch(kvpList);
                if (writeResult.Message != ResultType.Success)
                    Assert.Fail();
                database.Close();
                Assert.Pass();
            }
        }
    }
}