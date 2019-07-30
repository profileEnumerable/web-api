// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.ParseResult
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.CommandTrees;

namespace System.Data.Entity.Core.Common.EntitySql
{
  /// <summary>Entity SQL Parser result information.</summary>
  public sealed class ParseResult
  {
    private readonly DbCommandTree _commandTree;
    private readonly ReadOnlyCollection<FunctionDefinition> _functionDefs;

    internal ParseResult(DbCommandTree commandTree, List<FunctionDefinition> functionDefs)
    {
      this._commandTree = commandTree;
      this._functionDefs = new ReadOnlyCollection<FunctionDefinition>((IList<FunctionDefinition>) functionDefs);
    }

    /// <summary> A command tree produced during parsing. </summary>
    public DbCommandTree CommandTree
    {
      get
      {
        return this._commandTree;
      }
    }

    /// <summary>
    /// List of <see cref="T:System.Data.Entity.Core.Common.EntitySql.FunctionDefinition" /> objects describing query inline function definitions.
    /// </summary>
    public ReadOnlyCollection<FunctionDefinition> FunctionDefinitions
    {
      get
      {
        return this._functionDefs;
      }
    }
  }
}
