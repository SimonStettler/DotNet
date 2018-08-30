using System.Text;
using System.Xml;
using System.Collections.Generic;

namespace GenerateMetaRunner
{
    public static class AntProject
    {
        public static string BuildXml(string target, string executable, IEnumerable<string> args)
        {
            var memory = new StringBuilder();
            var xml = XmlWriter.Create(
                memory,
                new XmlWriterSettings
                {
                    OmitXmlDeclaration = true,
                    Encoding = Encoding.UTF8,
                    WriteEndDocumentOnClose = true,
                    ConformanceLevel = ConformanceLevel.Document, 
                    Indent = true,
                    CloseOutput = false });
            xml.WriteXml(target, executable, args);
            xml.Flush();
            return memory.ToString();
        }
        
        public static void WriteXml(this XmlWriter xml, string target, string executable, IEnumerable<string> args)
        {
            xml.WriteStartElement("project");
            xml.WriteAttributeString("name", "run-executable");
            xml.WriteStartElement("target");
            xml.WriteAttributeString("name", target);
            xml.WriteStartElement("exec");
            xml.WriteAttributeString("executable", executable);
            foreach (var arg in args)
            {
                xml.WriteStartElement("arg");
                xml.WriteAttributeString("value", arg);
                xml.WriteEndElement();
            }
            xml.WriteEndElement();
            xml.WriteEndElement();
            xml.WriteEndElement();
        }
    }
}
