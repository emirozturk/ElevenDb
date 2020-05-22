using NUnit.Framework;
using ElevenDb;
using System;

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
            int testSize = 10000;
            DB database = new DB(@"C:\Users\emiro\Desktop\Test\test.db");
            Result<string> openResult = database.Open();
            if (openResult.Message == ResultType.Success)
            {
                for (int i = 0; i < testSize; i++)
                {
                    int rnd = new Random().Next(0, testSize);
                    Result<string> writeResult = database.Write("Key" + rnd , "Value" + rnd);
                    if (writeResult.Message != ResultType.Success && writeResult.Message!=ResultType.Overwritten) Assert.Fail();
                }
                Assert.Pass();
                database.Close();
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
                Result<string> readResult = database.Read("Key"+215);
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
                    Result<string> readResult = database.Read("Key" + new Random().Next(0,1000));
                    if (readResult.Message != ResultType.Success && readResult.Message != ResultType.NotFound)
                        Assert.Fail();
                }
                Assert.Pass();
                database.Close();
            }
        }
        /*
        public void IterateTest()
        {
            DB database = new DB(@"C:\Users\emiro\Desktop\Test\test.db");
            Result<string> openResult = database.Open();
            if (openResult.Message == ResultType.Success)
            {
                Iterator iterator = database.GetIterator();
                while (iterator.HasRecord)
                    Console.WriteLine(iterator.GetNext().Value);
                Assert.Pass();
            }
            else
                Assert.Fail();
            database.Close();
        }
        /*
        public void ReadAllTest()
        {
            DB database = new DB(@"C:\Users\emiro\Desktop\Test\test.db");
            Result<string> openResult = database.Open();
            if (openResult.Message == ResultType.Success)
            {
                IEnumerable<Result> records = database.ReadAll();
                Assert.Pass();
            }
            else
                Assert.Fail();

        }
        */
    }
}