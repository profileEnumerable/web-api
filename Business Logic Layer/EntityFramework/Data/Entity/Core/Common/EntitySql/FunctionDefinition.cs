// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.FunctionDefinition
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Common.CommandTrees;

namespace System.Data.Entity.Core.Common.EntitySql
{
  /// <summary>
  /// Entity SQL query inline function definition, returned as a part of <see cref="T:System.Data.Entity.Core.Common.EntitySql.ParseResult" />.
  /// </summary>
  public sealed class FunctionDefinition
  {
    private readonly string _name;
    private readonly DbLambda _lambda;
    private readonly int _startPosition;
    private readonly int _endPosition;

    internal FunctionDefinition(string name, DbLambda lambda, int startPosition, int endPosition)
    {
      this._name = name;
      this._lambda = lambda;
      this._startPosition = startPosition;
      this._endPosition = endPosition;
    }

    /// <summary> Function name. </summary>
    public string Name
    {
      get
      {
        return this._name;
      }
    }

    /// <summary> Function body and parameters. </summary>
    public DbLambda Lambda
    {
      get
      {
        return this._lambda;
      }
    }

    /// <summary> Start position of the function definition in the eSQL query text. </summary>
    public int StartPosition
    {
      get
      {
        return this._startPosition;
      }
    }

    /// <summary> End position of the function definition in the eSQL query text. </summary>
    public int EndPosition
    {
      get
      {
        return this._endPosition;
      }
    }
  }
}
