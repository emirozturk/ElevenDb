using NUnit.Framework;
using ElevenDb;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PicoTests
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
            if (openResult.Message == ResultType.Success)
            {
                Result writeResult = database.Write("Key", "Value");
                if (writeResult.Message == ResultType.Success)
                    Assert.Pass();
                else
                    Assert.Fail();
            }
            else
                Assert.Fail();
        }
        public void OverwriteTest()
        {
            DB database = new DB(@"C:\Users\emiro\Desktop\Test\test.db");
            Result openResult = database.Open();
            if (openResult.Message == ResultType.Success)
            {
                Result writeResult = database.Write("Key", "Value");
                if (writeResult.Message == ResultType.Overwritten)
                    Assert.Pass();
                else
                    Assert.Fail();
            }
            else
                Assert.Fail();
        }
        [Test]
        public void ReadTest()
        {
            DB database = new DB(@"C:\Users\emiro\Desktop\Test\test.db");
            Result openResult = database.Open();
            if (openResult.Message == ResultType.Success)
            {
                Result readResult = database.Read("Key");
                if (readResult.Message == ResultType.Success || readResult.Message == ResultType.NotFound)
                {
                    Console.WriteLine(readResult.Value);
                    Assert.Pass();
                }
                else
                    Assert.Fail();
            }
            else
                Assert.Fail();

        }
        public void IterateTest()
        {
            DB database = new DB(@"C:\Users\emiro\Desktop\Test\test.db");
            Result openResult = database.Open();
            if (openResult.Message == ResultType.Success)
            {
                Iterator iterator = database.GetIterator();
                while (iterator.HasRecord)
                    Console.WriteLine(iterator.GetNext().Value);
                Assert.Pass();
            }
            else
                Assert.Fail();
        }
        public void ReadAllTest()
        {
            DB database = new DB(@"C:\Users\emiro\Desktop\Test\test.db");
            Result openResult = database.Open();
            if (openResult.Message == ResultType.Success)
            {
                IEnumerable<Result> records = database.ReadAll();
                Assert.Pass();
            }
            else
                Assert.Fail();

        }
    }
}