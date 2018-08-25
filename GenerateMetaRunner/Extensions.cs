using System;
using System.Collections.Generic;
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
    }
}