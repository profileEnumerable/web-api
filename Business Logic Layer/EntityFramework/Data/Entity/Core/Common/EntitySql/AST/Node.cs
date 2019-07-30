// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.AST.Node
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Common.EntitySql.AST
{
  internal abstract class Node
  {
    private ErrorContext _errCtx = new ErrorContext();

    internal Node()
    {
    }

    internal Node(string commandText, int inputPosition)
    {
      this._errCtx.CommandText = commandText;
      this._errCtx.InputPosition = inputPosition;
    }

    internal ErrorContext ErrCtx
    {
      get
      {
        return this._errCtx;
      }
      set
      {
        this._errCtx = value;
      }
    }
  }
}
