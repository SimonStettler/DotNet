using System.IO;

namespace GenerateMetaRunner
{
    public class Resources
    {
        public static Stream TemplateXml => typeof(Resources).Assembly.GetManifestResourceStream($"{typeof(Resources).Namespace}.template.xml");
    }
}