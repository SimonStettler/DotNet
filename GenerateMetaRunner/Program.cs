using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.SqlServer.Management.Smo;

namespace GenerateMetaRunner
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var xml = XmlWriter.Create(
                args.Any() ? new StreamWriter(File.Create(args.Single())) : Console.Out, 
                new XmlWriterSettings
                {
                    OmitXmlDeclaration = false,
                    Encoding = Encoding.UTF8,
                    WriteEndDocumentOnClose = true,
                    ConformanceLevel = ConformanceLevel.Document, 
                    Indent = true,
                    CloseOutput = false });

            var properties = typeof(ScriptingOptions).GetProperties()
                .Where(p => p.CanRead && p.CanWrite);
            
            var reader = XmlReader.Create(Resources.TemplateXml);
            while(reader.Read() && reader.LocalName != "insert_params_here")
            {
                xml.WriteShallowNode(reader);
            }
            
            var options = new ScriptingOptions();
            foreach (var property in properties)
            {
                var propertyValue = property.GetValue(options);
                var argumentValue = (propertyValue is Encoding) ? ((Encoding)propertyValue).BodyName : propertyValue.ToString();
                xml.WriteParamElement(property.Name, argumentValue, property.BuildSpec());
                xml.WriteRaw(Environment.NewLine + "            ");
            }
            
            while(reader.Read() && reader.LocalName != "insert_proc_additional_commandline_here")
            {
                xml.WriteShallowNode(reader);
            }

            var arguments = string.Join(" ", properties.Select(property => $"{property.Name}=\"%{property.Name}%\""));
            xml.WriteParamElement("proc_additional_commandline", $"%database% {arguments}");

            while (reader.Read())
            {
                xml.WriteShallowNode(reader);
            }
            xml.Flush();
            xml.Close();
        }
    }
}
