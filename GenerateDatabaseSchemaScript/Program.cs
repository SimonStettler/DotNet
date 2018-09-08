using Microsoft.SqlServer.Management.Smo;
using System;
using System.IO;
using System.Linq;

class Program
{
    internal static void Main(string[] args)
    {
        if (0 == args.Length)
        {
            throw new ArgumentException("Give a database name in the command line argument.");
        }
        
        Console.WriteLine("/** ");
        if (1 == args.Length)
        {
            var split = args[0].Split(' ');
            if (1 < split.Length)
            {
                Console.WriteLine("split");
                args = split;
            }
        }

        var name = args.Get("Database");
        var instance = args.Get("Instance");
        Console.WriteLine($"Database = {name}");
        Console.WriteLine($"Instance = {instance}");
        var server = new Server(instance);
        var databases = server.Databases.Cast<Database>();
        var matches = databases.Where((db) => String.Equals(db.Name, name));
        if (!matches.Any())
        {
            var names = string.Join(", ", databases.Select((db) => db.Name));
            throw new Exception($"Name {name} is not in {names}.");
        }
        var database = matches.Single();
        var scripter = new DatabaseScripter(database);
        foreach (var option in Arguments.FindProperties(args))
        {
            var value = option.ParseValue();
            option.property.SetValue(scripter.Options, value);
            Console.WriteLine($"{option.property.Name} = {value}");
        }
        Console.WriteLine("**/");
        foreach (var statement in scripter.GenerateScript())
        {
            Console.WriteLine(statement);
        }
    }
}
