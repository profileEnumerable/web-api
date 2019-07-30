// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.InternalSqlNonSetQuery
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Data.Entity.Infrastructure;

namespace System.Data.Entity.Internal
{
  internal class InternalSqlNonSetQuery : InternalSqlQuery
  {
    private readonly InternalContext _internalContext;
    private readonly Type _elementType;

    internal InternalSqlNonSetQuery(
      InternalContext internalContext,
      Type elementType,
      string sql,
      object[] parameters)
      : this(internalContext, elementType, sql, new bool?(), parameters)
    {
    }

    private InternalSqlNonSetQuery(
      InternalContext internalContext,
      Type elementType,
      string sql,
      bool? streaming,
      object[] parameters)
      : base(sql, streaming, parameters)
    {
      this._internalContext = internalContext;
      this._elementType = elementType;
    }

    public override InternalSqlQuery AsNoTracking()
    {
      return (InternalSqlQuery) this;
    }

    public override InternalSqlQuery AsStreaming()
    {
      if (!this.Streaming.HasValue || !this.Streaming.Value)
        return (InternalSqlQuery) new InternalSqlNonSetQuery(this._internalContext, this._elementType, this.Sql, new bool?(true), this.Parameters);
      return (InternalSqlQuery) this;
    }

    public override IEnumerator GetEnumerator()
    {
      return this._internalContext.ExecuteSqlQuery(this._elementType, this.Sql, this.Streaming, this.Parameters);
    }

    public override IDbAsyncEnumerator GetAsyncEnumerator()
    {
      return this._internalContext.ExecuteSqlQueryAsync(this._elementType, this.Sql, this.Streaming, this.Parameters);
    }
  }
}
