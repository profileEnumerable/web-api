// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.Linq.IInternalSet`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity.Internal.Linq
{
  internal interface IInternalSet<TEntity> : IInternalSet, IInternalQuery<TEntity>, IInternalQuery
    where TEntity : class
  {
    TEntity Find(params object[] keyValues);

    Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues);

    TEntity Create();

    TEntity Create(Type derivedEntityType);

    ObservableCollection<TEntity> Local { get; }
  }
}
