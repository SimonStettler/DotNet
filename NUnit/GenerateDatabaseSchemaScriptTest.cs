using NUnit.Framework;

namespace NUnit
{
    [TestFixture]
    public class GenerateDatabaseSchemaScriptTest
    {
        [Test]
        public void TestMain()
        {
            var name = "master";
            Assert.DoesNotThrow(() => Program.Main(new [] {name}));
        }
    }
}
