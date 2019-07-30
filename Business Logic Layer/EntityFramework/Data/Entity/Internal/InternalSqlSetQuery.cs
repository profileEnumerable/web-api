// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.InternalSqlSetQuery
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Internal.Linq;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Internal
{
  internal class InternalSqlSetQuery : InternalSqlQuery
  {
    private readonly IInternalSet _set;
    private readonly bool _isNoTracking;

    internal InternalSqlSetQuery(
      IInternalSet set,
      string sql,
      bool isNoTracking,
      object[] parameters)
      : this(set, sql, isNoTracking, new bool?(), parameters)
    {
    }

    private InternalSqlSetQuery(
      IInternalSet set,
      string sql,
      bool isNoTracking,
      bool? streaming,
      object[] parameters)
      : base(sql, streaming, parameters)
    {
      this._set = set;
      this._isNoTracking = isNoTracking;
    }

    public override InternalSqlQuery AsNoTracking()
    {
      if (!this._isNoTracking)
        return (InternalSqlQuery) new InternalSqlSetQuery(this._set, this.Sql, true, this.Streaming, this.Parameters);
      return (InternalSqlQuery) this;
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used by test code.")]
    public bool IsNoTracking
    {
      get
      {
        return this._isNoTracking;
      }
    }

    public override InternalSqlQuery AsStreaming()
    {
      if (!this.Streaming.HasValue || !this.Streaming.Value)
        return (InternalSqlQuery) new InternalSqlSetQuery(this._set, this.Sql, this._isNoTracking, new bool?(true), this.Parameters);
      return (InternalSqlQuery) this;
    }

    public override IEnumerator GetEnumerator()
    {
      return this._set.ExecuteSqlQuery(this.Sql, this._isNoTracking, this.Streaming, this.Parameters);
    }

    public override IDbAsyncEnumerator GetAsyncEnumerator()
    {
      return this._set.ExecuteSqlQueryAsync(this.Sql, this._isNoTracking, this.Streaming, this.Parameters);
    }
  }
}
