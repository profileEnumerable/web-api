// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.AST.FunctionDefinition
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Common.EntitySql.AST
{
  internal sealed class FunctionDefinition : Node
  {
    private readonly Identifier _name;
    private readonly NodeList<PropDefinition> _paramDefList;
    private readonly Node _body;
    private readonly int _startPosition;
    private readonly int _endPosition;

    internal FunctionDefinition(
      Identifier name,
      NodeList<PropDefinition> argDefList,
      Node body,
      int startPosition,
      int endPosition)
    {
      this._name = name;
      this._paramDefList = argDefList;
      this._body = body;
      this._startPosition = startPosition;
      this._endPosition = endPosition;
    }

    internal string Name
    {
      get
      {
        return this._name.Name;
      }
    }

    internal NodeList<PropDefinition> Parameters
    {
      get
      {
        return this._paramDefList;
      }
    }

    internal Node Body
    {
      get
      {
        return this._body;
      }
    }

    internal int StartPosition
    {
      get
      {
        return this._startPosition;
      }
    }

    internal int EndPosition
    {
      get
      {
        return this._endPosition;
      }
    }
  }
}
