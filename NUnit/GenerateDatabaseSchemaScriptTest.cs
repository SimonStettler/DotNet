using NUnit.Framework;
using System;
using System.IO;
using System.Text;

namespace NUnit
{
    [TestFixture]
    public class GenerateDatabaseSchemaScriptTest
    {
        [Test]
        public void TestMain()
        {
            var name = "master";
            Assert.DoesNotThrow(() => Program.Main(new [] {name, "Encoding=utf-8"}));
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
