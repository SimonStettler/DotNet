using System.IO;
using System.Xml;
using System.Xml.Schema;
using GenerateMetaRunner;
using NUnit.Framework;

namespace NUnit
{
    [TestFixture]
    public class XmlTests
    {
        [Test]
        public static void ValidateTemplateXml()
        {
            TestSchemaValidation(Resources.TemplateXml, Resources.MetaRunnerXsd);
        }

        private static void TestSchemaValidation(Stream xml, Stream xsd)
        {
            ValidationEventHandler validationEventHandler= (sender, args) =>
            {
                if (args.Severity == XmlSeverityType.Warning)
                {
                    Assert.Inconclusive(args.Message);
                }
                else
                {
                    Assert.Fail(args.Message);
                }
            };

            var settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationFlags = 
                settings.ValidationFlags | 
                XmlSchemaValidationFlags.ProcessInlineSchema | 
                XmlSchemaValidationFlags.ProcessSchemaLocation | 
                XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationEventHandler += validationEventHandler;
            
            var schema = XmlSchema.Read(xsd, validationEventHandler);
            settings.Schemas.Add(schema);
                
            var reader = XmlReader.Create(xml, settings);
            while (reader.Read())
            {
                Assert.IsTrue(true);
            };
        }
    }
}