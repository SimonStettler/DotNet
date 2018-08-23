using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.IO;
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

    public void Script(StreamWriter target)
    {
        this.ScriptEachObject(this.database.Tables, target);
        this.ScriptEachObject(this.database.Views, target);
        this.ScriptEachObject(this.database.StoredProcedures, target);
        this.ScriptEachObject(this.database.UserDefinedFunctions, target);
    }

    private void ScriptEachObject(SchemaCollectionBase collection, StreamWriter target)
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
                    target.WriteLine($"GO{statement}{Environment.NewLine}GO");
                }
                else
                {
                    target.WriteLine(statement);
                }
            }                
        }
    }
}
