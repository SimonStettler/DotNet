using Microsoft.SqlServer.Management.Smo;
using System;
using System.IO;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        if (0 == args.Length)
        {
            throw new ArgumentException("Give a database name in the command line argument.");
        }
        var name = args[0];
        var server = new Server(@"(localdb)\v11.0");
        var database = server.Databases.Cast<Database>().Where((db) => String.Equals(db.Name, name)).Single();
        var scripter = new DatabaseScripter(database);
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
