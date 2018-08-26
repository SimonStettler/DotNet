using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace GenerateMetaRunner
{
    public static class Extensions
    {
        public static void WriteParamElement(this XmlWriter xml, string name, string value, string spec = null)
        {
            xml.WriteStartElement("param");
            xml.WriteAttributeString("name", name);
            xml.WriteAttributeString("value", value);
            if (null != spec)
            {
                xml.WriteAttributeString("spec", spec);
            }
            xml.WriteEndElement();
        }

        public static string BuildSpec(this PropertyInfo property)
        {
            if (property.PropertyType == typeof(bool))
            {
                return $"checkbox uncheckedValue='{bool.FalseString}' display='normal' checkedValue='{bool.TrueString}'";
            }
            if (property.PropertyType == typeof(int)) 
            {
                return "text regexp='^\\d+$' display='normal' validationMode='regex'";
            } 
            if (property.PropertyType == typeof(string)) 
            {
                return "text regexp='^.+$' display='normal' validationMode='regex'";
            } 
            if (property.PropertyType == typeof(Encoding)) 
            {
                var index = 1;
                return "select display='normal' " + string.Join(" ", Encoding.GetEncodings().Select(encoding => $"data_{index++}='{encoding.Name}'"));
            } 
            if (property.PropertyType.IsEnum)
            {
                var index = 1;
                return "select display='normal' " + string.Join(" ", Enum.GetNames(property.PropertyType).Select(name => $"data_{index++}='{name}'"));
            }

            return string.Empty;
        }

        public static void WriteShallowNode(this XmlWriter writer, XmlReader reader)
        {
            switch (reader.NodeType)
            {
                case XmlNodeType.Element:
                    writer.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
                    writer.WriteAttributes(reader, true);
                    if (reader.IsEmptyElement)
                    {
                        writer.WriteEndElement();
                    }
                    break;
                case XmlNodeType.Text:
                    writer.WriteString(reader.Value);
                    break;
                case XmlNodeType.Whitespace:
                case XmlNodeType.SignificantWhitespace:
                    writer.WriteWhitespace(reader.Value);
                    break;
                case XmlNodeType.CDATA:
                    writer.WriteCData(reader.Value);
                    break;
                case XmlNodeType.EntityReference:
                    writer.WriteEntityRef(reader.Name);
                    break;
                case XmlNodeType.XmlDeclaration:
                case XmlNodeType.ProcessingInstruction:
                    writer.WriteProcessingInstruction(reader.Name, reader.Value);
                    break;
                case XmlNodeType.DocumentType:
                    writer.WriteDocType(reader.Name, reader.GetAttribute( "PUBLIC" ), reader.GetAttribute( "SYSTEM" ), reader.Value);
                    break;
                case XmlNodeType.Comment:
                    writer.WriteComment(reader.Value);
                    break;
                case XmlNodeType.EndElement:
                    writer.WriteFullEndElement();
                    break;
            }
        }
    }
}