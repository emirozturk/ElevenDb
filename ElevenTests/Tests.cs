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
            DB database = new DB(@"C:\Users\emiro\Desktop\Test\test.db");
            Result openResult = database.Open();
            if (openResult.IsSuccess)
            {
                Result writeResult = database.Write("Key", "Value");
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

            database.Close();
        }
        [Test]
        public void MultiWriteTest()
        {
            int testSize = 1000;
            DB database = new DB(@"C:\Users\emiro\Desktop\Test\test.db");
            Result openResult = database.Open();
            if (openResult.IsSuccess)
            {
                for (int i = 0; i < testSize; i++)
                {
                    int rnd = new Random().Next(0, testSize);
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
            DB database = new DB(@"C:\Users\emiro\Desktop\Test\test.db");
            Result openResult = database.Open();
            if (openResult.IsSuccess)
            {
                Result writeResult = database.Write("Key", "Value");
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

            database.Close();
        }
        [Test]
        public void ReadTest()
        {
            DB database = new DB(@"C:\Users\emiro\Desktop\Test\test.db");
            Result openResult = database.Open();
            if (openResult.IsSuccess)
            {
                Result readResult = database.Read("Key" + 215);
                if (readResult.IsSuccess)
                {
                    Console.WriteLine(readResult.Data);
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

            database.Close();
        }
        [Test]
        public void MultiReadTest()
        {
            DB database = new DB(@"C:\Users\emiro\Desktop\Test\test.db");
            Result openResult = database.Open();
            if (openResult.IsSuccess)
            {
                for (int i = 0; i < 10000; i++)
                {
                    Result readResult = database.Read("Key" + new Random().Next(0, 1000));
                    if (!readResult.IsSuccess)
                    {
                        Assert.Fail();
                    }
                }
                Assert.Pass();
                database.Close();
            }
        }
        [Test]
        public void DeleteTest()
        {
            DB database = new DB(@"C:\Users\emiro\Desktop\Test\test.db");
            Result openResult = database.Open();
            if (openResult.IsSuccess)
            {
                Result writeResult = database.Delete("Key" + new Random().Next(0, 1000));
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

            database.Close();
        }
        [Test]
        public void IterateTest()
        {
            DB database = new DB(@"C:\Users\emiro\Desktop\Test\test.db");
            Result openResult = database.Open();
            if (openResult.IsSuccess)
            {
                Iterator iterator = database.GetIterator();
                while (iterator.HasRecord)
                {
                    Result result = iterator.GetNext();
                    Console.WriteLine(result.Data);
                }
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }

            database.Close();
        }
        [Test]
        public void ReadAllTest()
        {
            DB database = new DB(@"C:\Users\emiro\Desktop\Test\test.db");
            Result openResult = database.Open();
            if (openResult.IsSuccess)
            {
                Result result = database.ReadAll();
                if (result.IsSuccess)
                {
                    foreach (dynamic kvp in result.Data)
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

            database.Close();
        }
        [Test]
        public void WriteBatchTest()
        {
            int testSize = 1000;
            DB database = new DB(@"C:\Users\emiro\Desktop\Test\test.db");
            Result openResult = database.Open();
            if (openResult.IsSuccess)
            {
                List<KeyValuePair<string, string>> kvpList = new List<KeyValuePair<string, string>>();
                for (int i = 0; i < testSize; i++)
                {
                    int rnd = new Random().Next(0, testSize);
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
    }
}