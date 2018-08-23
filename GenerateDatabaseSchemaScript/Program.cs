using Microsoft.SqlServer.Management.Smo;
using System;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        if (0 == args.Length)
        {
            System.Console.WriteLine("Give a database name in the command line argument.");
            return;
        }
        var server = new Server(@"(localdb)\v11.0");
        var database = server.Databases.Cast<Database>().Where((db) => String.Equals(db.Name, args[0])).Single();

        var databaseScripter = new DatabaseScripter(database);
        databaseScripter.Script();
    }
}
