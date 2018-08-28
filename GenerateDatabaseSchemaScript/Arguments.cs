using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.SqlServer.Management.Smo;

public class Arguments
{
    public static IEnumerable<Option> FindOptions(string[] args)
    {
        var properties = typeof(ScriptingOptions).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            var split = arg.Split('=').ToArray();            
            var property = typeof(ScriptingOptions).GetProperty(split.First());
            if (null == property)
            {
                Console.WriteLine(arg);
                continue;
            }
            var value = split.Skip(1).FirstOrDefault();
            if (null == value)
            {
                Console.WriteLine(arg);
                continue;
            }
            var option = new Option(property, value);
            yield return option;
        }
    }

    public struct Option
    {
        public Option(PropertyInfo property, string value)
        {
            this.property = property;
            this.value = value;
        }

        public readonly PropertyInfo property;
        public readonly string value;
        
        public object ParseValue()
        {
            if (property.PropertyType == typeof(bool))
            {
                return bool.Parse(value);
            }
            if (property.PropertyType == typeof(int)) 
            {
                return int.Parse(value);
            } 
            if (property.PropertyType == typeof(string)) 
            {
                return value;
            } 
            if (property.PropertyType == typeof(Encoding)) 
            {
                try
                {
                    return Encoding.GetEncoding(value);
                }
                catch (Exception e)
                {
                    return Encoding.UTF8;
                }
            } 
            if (property.PropertyType.IsEnum)
            {
                return Enum.Parse(property.PropertyType, value);
            }
        
            throw new ArgumentException($"property {property.Name} has unknown type {property.PropertyType.Name}");
        }
    }
}
