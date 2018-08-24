using System;
using System.Linq;
using System.Xml;
using Microsoft.SqlServer.Management.Smo;

namespace GenerateMetaRunner
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var xml = XmlWriter.Create(
                Console.Out, 
                new XmlWriterSettings
                {
                    OmitXmlDeclaration = true, 
                    ConformanceLevel = ConformanceLevel.Document, 
                    CloseOutput = false });

            xml.WriteStartDocument();
            xml.WriteStartElement("meta-runner");
            xml.WriteAttributeString("name", typeof(ScriptingOptions).Name);
            xml.WriteElementString("description", typeof(ScriptingOptions).FullName ?? "");
            xml.WriteStartElement("settings");
            xml.WriteStartElement("parameters");
            
            var properties = typeof(ScriptingOptions).GetProperties()
                .Where(p => p.CanRead && p.CanWrite);
            
            xml.WriteParamElement("database", string.Empty, string.Empty);

            foreach (var property in properties)
            {
                xml.WriteParamElement(property.Name, string.Empty, property.BuildSpec());
            }
            xml.WriteEndElement();

            xml.WriteStartElement("build-runners");
            xml.WriteStartElement("runner");
            xml.WriteAttributeString("name", "");
            xml.WriteAttributeString("type", "jetbrains.dotNetGenericRunner");
            xml.WriteStartElement("parameters");
            
            xml.WriteParamElement("dotNetCoverage.NCover.HTMLReport.File.Sort", "0");
            xml.WriteParamElement("dotNetCoverage.NCover.HTMLReport.File.Type", "1");
            xml.WriteParamElement("dotNetCoverage.NCover.Reg", "selected");
            xml.WriteParamElement("dotNetCoverage.NCover.platformBitness", "x86");
            xml.WriteParamElement("dotNetCoverage.NCover.platformVersion", "v2.0");
            xml.WriteParamElement("dotNetCoverage.NCover3.Reg", "selected");
            xml.WriteParamElement("dotNetCoverage.NCover3.args", "//ias .*");
            xml.WriteParamElement("dotNetCoverage.NCover3.platformBitness", "x86");
            xml.WriteParamElement("dotNetCoverage.NCover3.platformVersion", "v2.0");
            xml.WriteParamElement("dotNetCoverage.NCover3.reporter.executable.args", "//or FullCoverageReport:Html:{teamcity.report.path}");
            xml.WriteParamElement("dotNetCoverage.PartCover.Reg", "selected");
            xml.WriteParamElement("dotNetCoverage.PartCover.includes", "[*]*");
            xml.WriteParamElement("dotNetCoverage.PartCover.platformBitness", "x86");
            xml.WriteParamElement("dotNetCoverage.PartCover.platformVersion", "v2.0");
            xml.WriteParamElement("dotNetCoverage.dotCover.home.path", "%teamcity.tool.JetBrains.dotCover.CommandLineTools.DEFAULT%");
            xml.WriteParamElement("dotNetTestRunner.Type", "GenericProcess");
            xml.WriteParamElement("teamcity.step.mode", "default");
            xml.WriteParamElement("proc_bit", "x86");
            xml.WriteParamElement("proc_runtime_version", "v4.0");
            xml.WriteParamElement("proc_path", "GenerateScript");
            xml.WriteParamElement("proc_additional_commandline", "%database% " + string.Join(" ", properties.Select(property => $"{property.Name}=\"%{property.Name}%\"")));
            
            xml.WriteEndElement();
            xml.WriteEndElement();
            xml.WriteEndElement();
            
            xml.WriteEndElement();
            xml.WriteEndElement();
            xml.WriteEndDocument();
            
            xml.Flush();
            xml.Close();
        }
    }
}
