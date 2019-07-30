// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.AST.CreateRefExpr
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Common.EntitySql.AST
{
  internal sealed class CreateRefExpr : Node
  {
    private readonly Node _entitySet;
    private readonly Node _keys;
    private readonly Node _typeIdentifier;

    internal CreateRefExpr(Node entitySet, Node keys)
      : this(entitySet, keys, (Node) null)
    {
    }

    internal CreateRefExpr(Node entitySet, Node keys, Node typeIdentifier)
    {
      this._entitySet = entitySet;
      this._keys = keys;
      this._typeIdentifier = typeIdentifier;
    }

    internal Node EntitySet
    {
      get
      {
        return this._entitySet;
      }
    }

    internal Node Keys
    {
      get
      {
        return this._keys;
      }
    }

    internal Node TypeIdentifier
    {
      get
      {
        return this._typeIdentifier;
      }
    }
  }
}
