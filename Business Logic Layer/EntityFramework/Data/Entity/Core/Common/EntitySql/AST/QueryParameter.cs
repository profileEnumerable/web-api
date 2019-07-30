// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.AST.QueryParameter
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Resources;

namespace System.Data.Entity.Core.Common.EntitySql.AST
{
  internal sealed class QueryParameter : Node
  {
    private readonly string _name;

    internal QueryParameter(string parameterName, string query, int inputPos)
      : base(query, inputPos)
    {
      this._name = parameterName.Substring(1);
      if (this._name.StartsWith("_", StringComparison.OrdinalIgnoreCase) || char.IsDigit(this._name, 0))
        throw EntitySqlException.Create(this.ErrCtx, Strings.InvalidParameterFormat((object) this._name), (Exception) null);
    }

    internal string Name
    {
      get
      {
        return this._name;
      }
    }
  }
}
