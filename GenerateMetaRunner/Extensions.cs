using System;
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
                return "checkbox uncheckedValue='false' display='normal' checkedValue='true'";
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
                return $"select display='normal' data_1='${Encoding.UTF8.BodyName}'";
            } 
            if (property.PropertyType.IsEnum) {
                var spec = "select display='normal'";
                var index = 1;
                foreach (var name in Enum.GetNames(property.PropertyType))
                {
                    spec += $" data_{index++}='{name}'";
                }
                return spec;
            }

            return string.Empty;
        }
    }
}