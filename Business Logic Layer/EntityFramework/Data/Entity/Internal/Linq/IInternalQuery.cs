// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.Linq.IInternalQuery
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.ELinq;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;

namespace System.Data.Entity.Internal.Linq
{
  internal interface IInternalQuery
  {
    void ResetQuery();

    InternalContext InternalContext { get; }

    ObjectQuery ObjectQuery { get; }

    Type ElementType { get; }

    Expression Expression { get; }

    ObjectQueryProvider ObjectQueryProvider { get; }

    IDbAsyncEnumerator GetAsyncEnumerator();

    IEnumerator GetEnumerator();
  }
}
