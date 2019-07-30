// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.Linq.IInternalSet
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Data.Entity.Infrastructure;

namespace System.Data.Entity.Internal.Linq
{
  internal interface IInternalSet : IInternalQuery
  {
    void Attach(object entity);

    void Add(object entity);

    void AddRange(IEnumerable entities);

    void RemoveRange(IEnumerable entities);

    void Remove(object entity);

    void Initialize();

    void TryInitialize();

    IEnumerator ExecuteSqlQuery(
      string sql,
      bool asNoTracking,
      bool? streaming,
      object[] parameters);

    IDbAsyncEnumerator ExecuteSqlQueryAsync(
      string sql,
      bool asNoTracking,
      bool? streaming,
      object[] parameters);
  }
}
