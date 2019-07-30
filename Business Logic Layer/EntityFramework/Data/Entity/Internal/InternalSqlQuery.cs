// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.InternalSqlQuery
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Data.Entity.Infrastructure;

namespace System.Data.Entity.Internal
{
  internal abstract class InternalSqlQuery : IEnumerable, IDbAsyncEnumerable
  {
    private readonly string _sql;
    private readonly object[] _parameters;
    private readonly bool? _streaming;

    internal InternalSqlQuery(string sql, bool? streaming, object[] parameters)
    {
      this._sql = sql;
      this._parameters = parameters;
      this._streaming = streaming;
    }

    public string Sql
    {
      get
      {
        return this._sql;
      }
    }

    internal bool? Streaming
    {
      get
      {
        return this._streaming;
      }
    }

    public object[] Parameters
    {
      get
      {
        return this._parameters;
      }
    }

    public abstract InternalSqlQuery AsNoTracking();

    public abstract InternalSqlQuery AsStreaming();

    public abstract IEnumerator GetEnumerator();

    public abstract IDbAsyncEnumerator GetAsyncEnumerator();

    public override string ToString()
    {
      return this.Sql;
    }
  }
}
