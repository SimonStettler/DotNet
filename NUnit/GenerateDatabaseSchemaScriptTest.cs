using NUnit.Framework;
using System;
using System.IO;

namespace NUnit
{
    [TestFixture]
    public class GenerateDatabaseSchemaScriptTest
    {
        [Test]
        public void TestArgumentsGet()
        {
            var args = new [] {"master", "Encoding=utf-8", "FileName=master.sql"};
            Assert.AreEqual("utf-8", args.Get("Encoding"));
            Assert.AreEqual(string.Empty, args.Get("master"));
            Assert.AreEqual("master.sql", args.Get("FileName"));
        }

        [Test]
        public void TestMain()
        {
            var name = "master";
            Assert.DoesNotThrow(() => Program.Main(new [] {name, "Encoding=utf-8", $"FileName={name}.sql"}));
            Assert.That(File.Exists($"{name}.sql"));
        }

        [Test]
        public void TestMain2()
        {
            var name = "master";
            Assert.DoesNotThrow(() => Program.Main(new [] {$"{name} Encoding=utf-8 FileName={name}.sql"}));
            Assert.That(File.Exists($"{name}.sql"));
        }

        [Test]
        public void TestFailure()
        {
            var name = "slave";
            Assert.Throws<Exception>(() => Program.Main(new [] {name}));
        }
    }
}
