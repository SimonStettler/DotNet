using Microsoft.SqlServer.Management.Smo;
using System;
using System.IO;
using System.Linq;

class Program
{
    internal static void Main(string[] args)
    {
        
        Console.WriteLine("args");
        foreach (var arg in args)
        {
            Console.WriteLine(arg);
        }
        Console.WriteLine("args");

        if (0 == args.Length)
        {
            throw new ArgumentException("Give a database name in the command line argument.");
        }
        if (1 == args.Length)
        {
            var split = args[0].Split(' ');
            if (1 < split.Length)
            {
                args = split;
            }
        }
        var name = args[0];
        var server = new Server(@"(localdb)\v11.0");
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
            option.property.SetValue(scripter.Options, option.ParseValue());
        }
        using (var file = File.OpenWrite($"{args[0]}.sql"))
        using (var writer = new StreamWriter(file))
        {
            foreach (var statement in scripter.GenerateScript())
            {
                writer.WriteLine(statement);
            }
        }
    }
}
