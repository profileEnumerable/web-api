// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.Linq.NonGenericDbQueryProvider
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Data.Entity.Internal.Linq
{
  internal class NonGenericDbQueryProvider : DbQueryProvider
  {
    public NonGenericDbQueryProvider(InternalContext internalContext, IInternalQuery internalQuery)
      : base(internalContext, internalQuery)
    {
    }

    public override IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
      Check.NotNull<Expression>(expression, nameof (expression));
      ObjectQuery objectQuery = this.CreateObjectQuery(expression);
      if (typeof (TElement) != ((IQueryable) objectQuery).ElementType)
        return (IQueryable<TElement>) this.CreateQuery(objectQuery);
      return (IQueryable<TElement>) new InternalDbQuery<TElement>((IInternalQuery<TElement>) new InternalQuery<TElement>(this.InternalContext, objectQuery));
    }

    public override IQueryable CreateQuery(Expression expression)
    {
      Check.NotNull<Expression>(expression, nameof (expression));
      return this.CreateQuery(this.CreateObjectQuery(expression));
    }

    private IQueryable CreateQuery(ObjectQuery objectQuery)
    {
      IInternalQuery internalQuery = this.CreateInternalQuery(objectQuery);
      return (IQueryable) ((IEnumerable<ConstructorInfo>) typeof (InternalDbQuery<>).MakeGenericType(internalQuery.ElementType).GetConstructors(BindingFlags.Instance | BindingFlags.Public)).Single<ConstructorInfo>().Invoke(new object[1]
      {
        (object) internalQuery
      });
    }
  }
}
