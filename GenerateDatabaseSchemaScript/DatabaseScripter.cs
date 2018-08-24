using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
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

    public ScriptingOptions Options
    {
        get
        {
            return this.scripter.Options;
        }
    }

    public IEnumerable<string> GenerateScript()
    {
        return ForEachScriptSchemaObject().SelectMany(ForEachStatement).SelectMany(InjectBatchTerminator);
    }

    private IEnumerable<ScriptSchemaObjectBase> ForEachScriptSchemaObject()
    {
        foreach(ScriptSchemaObjectBase table in database.Tables)
        {
            yield return table;
        }
        foreach(ScriptSchemaObjectBase view in database.Views)
        {
            yield return view;
        }
        foreach (ScriptSchemaObjectBase storedProcedure in database.StoredProcedures)
        {
            yield return storedProcedure;
        }
        foreach (ScriptSchemaObjectBase userDefinedFunction in database.UserDefinedFunctions)
        {
            yield return userDefinedFunction;
        }
    }

    private IEnumerable<string> ForEachStatement(ScriptSchemaObjectBase o)
    {
        var script = this.scripter.Script(new Urn[]{o.Urn});
        foreach (var statement in script)
        {
            yield return statement;
        }                
    }
    
    private IEnumerable<string> InjectBatchTerminator(string statement)
    {
        if (statement.Contains("CREATE VIEW") || 
            statement.Contains("CREATE PROCEDURE") || 
            statement.Contains("create procedure"))
        {
            yield return "GO";
            yield return statement;
            yield return "GO";
        }
        else
        {
            yield return statement;
        }
    }
}
