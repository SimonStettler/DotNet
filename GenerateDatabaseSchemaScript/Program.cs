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
        if (1 == args.Length)
        {
            var split = args[0].Split(' ');
            if (1 < split.Length)
            {
                args = split;
            }
        }

        var name = args.Get("Database");
        var server = new Server(args.Get("Instance"));
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
        foreach (var statement in scripter.GenerateScript())
        {
            Console.WriteLine(statement);
        }
    }
}
