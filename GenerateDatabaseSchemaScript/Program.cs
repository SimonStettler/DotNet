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
        var server = new Server(@"(localdb)\v11.0");
        var database = server.Databases.Cast<Database>().Where((db) => String.Equals(db.Name, args[0])).Single();
        var databaseScripter = new DatabaseScripter(database);
        using (var file = File.OpenWrite($"{args[0]}.sql"))
        using (var writer = new StreamWriter(file))
        {
            databaseScripter.Script(writer);
        }
    }
}
