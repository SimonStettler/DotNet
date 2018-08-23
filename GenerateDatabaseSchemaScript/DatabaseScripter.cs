using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Linq;

class DatabaseScripter
{
    private readonly Scripter scripter;
    private readonly Database database;

    public DatabaseScripter(Database database)
    {
        this.database = database;
        this.scripter = new Scripter(database.Parent);
        var options = scripter.Options;
        options.ScriptDrops = false;
        options.WithDependencies = true;
        options.IncludeHeaders = true;
        options.ScriptSchema = true;
        options.AllowSystemObjects = false;
        options.Indexes = true;
        options.ScriptBatchTerminator = true;
        options.IncludeHeaders = false;
        options.WithDependencies = false;
        options.Triggers = true;
        options.Indexes = true;
    }

    public void Script()
    {
        this.ScriptEachObject(this.database.Tables);
        this.ScriptEachObject(this.database.Views);
        this.ScriptEachObject(this.database.StoredProcedures);
        this.ScriptEachObject(this.database.UserDefinedFunctions);
    }

    private void ScriptEachObject(SchemaCollectionBase collection)
    {
        foreach(var o in collection.Cast<ScriptSchemaObjectBase>())
        {
            var script = this.scripter.Script(new Urn[]{o.Urn});
            foreach (var statement in script)
            {
                if (statement.Contains("CREATE VIEW") || 
                    statement.Contains("CREATE PROCEDURE") || 
                    statement.Contains("create procedure"))
                {
                    Console.WriteLine($"GO{statement}{Environment.NewLine}GO");
                }
                else
                {
                    Console.WriteLine(statement);
                }
            }                
        }
    }
}
