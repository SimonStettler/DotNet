using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.SqlServer.Management.Smo;

public static class Arguments
{
    public static IEnumerable<Option> FindProperties(string[] args)
    {
        return typeof(ScriptingOptions)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty)
            .Where(property => args.Any(arg => arg.StartsWith(property.Name)))
            .Select(property => new Option(
                property,
                args.Get(property.Name)));
    }

    public static string Get(this IEnumerable<string> args, string key)
    {
        return args.SingleOrDefault(arg => arg.StartsWith(key + "="))?.Substring(1 + key.Length) ?? string.Empty;
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
                return Encoding.GetEncoding(value);
            } 
            if (property.PropertyType.IsEnum)
            {
                return Enum.Parse(property.PropertyType, value);
            }
        
            throw new ArgumentException($"property {property.Name} has unknown type {property.PropertyType.Name}");
        }
    }
}
