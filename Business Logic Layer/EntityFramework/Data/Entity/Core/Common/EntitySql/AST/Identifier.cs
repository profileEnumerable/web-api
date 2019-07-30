// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.AST.Identifier
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Resources;

namespace System.Data.Entity.Core.Common.EntitySql.AST
{
  internal sealed class Identifier : Node
  {
    private readonly string _name;
    private readonly bool _isEscaped;

    internal Identifier(string name, bool isEscaped, string query, int inputPos)
      : base(query, inputPos)
    {
      if (!isEscaped)
      {
        bool isIdentifierASCII = true;
        if (!CqlLexer.IsLetterOrDigitOrUnderscore(name, out isIdentifierASCII))
        {
          if (isIdentifierASCII)
            throw EntitySqlException.Create(this.ErrCtx, Strings.InvalidSimpleIdentifier((object) name), (Exception) null);
          throw EntitySqlException.Create(this.ErrCtx, Strings.InvalidSimpleIdentifierNonASCII((object) name), (Exception) null);
        }
      }
      this._name = name;
      this._isEscaped = isEscaped;
    }

    internal string Name
    {
      get
      {
        return this._name;
      }
    }

    internal bool IsEscaped
    {
      get
      {
        return this._isEscaped;
      }
    }
  }
}
