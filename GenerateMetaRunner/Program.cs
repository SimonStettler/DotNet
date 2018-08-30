using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using Microsoft.SqlServer.Management.Smo;

namespace GenerateMetaRunner
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var memory = new MemoryStream();
            var xml = XmlWriter.Create(
                memory,
                new XmlWriterSettings
                {
                    OmitXmlDeclaration = false,
                    Encoding = Encoding.UTF8,
                    WriteEndDocumentOnClose = true,
                    ConformanceLevel = ConformanceLevel.Document, 
                    Indent = true,
                    CloseOutput = false });

            var settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationFlags = 
                settings.ValidationFlags | 
                XmlSchemaValidationFlags.ProcessInlineSchema | 
                XmlSchemaValidationFlags.ProcessSchemaLocation | 
                XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationEventHandler += (sender, e) =>
            {
                throw e.Exception;
            };
                
            var schema = XmlSchema.Read(Resources.MetaRunnerXsd, (sender, e) =>
            {
                throw e.Exception;
            });
            settings.Schemas.Add(schema);
            
            var properties = typeof(ScriptingOptions).GetProperties()
                .Where(p => p.CanRead && p.CanWrite);
            
            var reader = XmlReader.Create(Resources.TemplateXml, settings);
            reader.Pipe(xml, (r, w) => !(r.NodeType == XmlNodeType.Comment || r.Value == " params "));
            
            var options = new ScriptingOptions();
            foreach (var property in properties)
            {
                var propertyValue = property.GetValue(options);
                var argumentValue = (propertyValue is Encoding) ? ((Encoding)propertyValue).BodyName : propertyValue.ToString();
                xml.WriteParamElement(property.Name, argumentValue, property.BuildSpec());
                xml.WriteRaw(Environment.NewLine + "            ");
            }

            reader.Pipe(xml, (r, w) => !(r.NodeType == XmlNodeType.Comment || r.Value == " proc_additional_commandline "));

            var arguments = string.Join(" ", properties.Select(property => $"{property.Name}=\"%{property.Name}%\""));
            xml.WriteParamElement("proc_additional_commandline", $"%database% {arguments}");

            reader.Pipe(xml);
            xml.Flush();
                        
            var output = args.Any() 
                ? new StreamWriter(File.OpenWrite(
                    args.Skip(1).Any() 
                        ? string.Join(" ", args) 
                        : args.Single())) 
                : Console.Out;

            memory.Position = 0;
            var writer = XmlWriter.Create(output);
            XmlReader.Create(memory, settings).Pipe(writer);
            writer.Flush();
        }
    }
}
