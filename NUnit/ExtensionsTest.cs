using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using NUnit.Framework;
using GenerateMetaRunner;

namespace NUnit
{
    [TestFixture]
    public class ExtensionsTest
    {
        [Test]
        public void TestWriteParamElement()
        {
            var stream = new MemoryStream();
            var writer = XmlWriter.Create(
                stream, 
                new XmlWriterSettings
                {
                    OmitXmlDeclaration = true, 
                    ConformanceLevel = ConformanceLevel.Fragment, 
                    CloseOutput = false });

            Assert.DoesNotThrow(
                ()=> writer.WriteParamElement("name", "value", "spec"));

            writer.Flush();
            
            Assert.Greater(stream.Length, 0);
            Assert.That(stream.Length > 0);

            stream.Flush();
            stream.Position = 0;
            var xml = new StreamReader(stream).ReadToEnd();
            
            Assert.That(
                xml.StartsWith("<") &&
                xml.Contains("name") &&
                xml.Contains("value") &&
                xml.Contains("spec"));
        }
        
        [Test]
        public void TestBuildSpec()
        {
            var boolean = (new {Property = true}).GetType().GetProperties().Single().BuildSpec();
            Assert.That(boolean.StartsWith("checkbox"));

            var integer = (new {Property = 42}).GetType().GetProperties().Single().BuildSpec();
            Assert.That(integer.StartsWith("text"));

            var fileaccess = (new {Property = FileAccess.ReadWrite}).GetType().GetProperties().Single().BuildSpec();
            Assert.That(fileaccess.StartsWith("select"));

            var emptystring = (new {Property = string.Empty}).GetType().GetProperties().Single().BuildSpec();
            Assert.That(emptystring.StartsWith("text"));

            var encoding = (new {Encoding = Encoding.UTF8}).GetType().GetProperties().Single().BuildSpec();
            Assert.That(encoding.Contains(Encoding.UTF8.WebName));
        }
        
        [Test]
        public void TestWriteShallowNode()
        {
            var memory = new MemoryStream();
            var reader = XmlReader.Create(Resources.TemplateXml);
            var writer = XmlWriter.Create(
                new StreamWriter(memory), 
                new XmlWriterSettings
                {
                    OmitXmlDeclaration = false,
                    WriteEndDocumentOnClose = true,
                    ConformanceLevel = ConformanceLevel.Auto, 
                    Indent = true,
                    CloseOutput = true });
            
            while (reader.Read())
            {
                writer.WriteShallowNode(reader);
                if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "parameters")
                {
                    Assert.IsTrue(true);
                }

            }
            writer.Flush();
            memory.Position = 0;

            Assert.AreEqual(
                Regex.Replace(new StreamReader(Resources.TemplateXml).ReadToEnd(), @"\s+"," "), 
                Regex.Replace(new StreamReader(memory).ReadToEnd(), @"\s+"," "));
        }
    }
}