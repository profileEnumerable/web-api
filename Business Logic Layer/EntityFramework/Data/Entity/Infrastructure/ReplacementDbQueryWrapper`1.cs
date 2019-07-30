// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.ReplacementDbQueryWrapper`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Objects;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// Instances of this class are used internally to create constant expressions for <see cref="T:System.Data.Entity.Core.Objects.ObjectQuery`1" />
  /// that are inserted into the expression tree to  replace references to <see cref="T:System.Data.Entity.Infrastructure.DbQuery`1" />
  /// and <see cref="T:System.Data.Entity.Infrastructure.DbQuery" />.
  /// </summary>
  /// <typeparam name="TElement"> The type of the element. </typeparam>
  public sealed class ReplacementDbQueryWrapper<TElement>
  {
    private readonly ObjectQuery<TElement> _query;

    private ReplacementDbQueryWrapper(ObjectQuery<TElement> query)
    {
      this._query = query;
    }

    internal static ReplacementDbQueryWrapper<TElement> Create(
      ObjectQuery query)
    {
      return new ReplacementDbQueryWrapper<TElement>((ObjectQuery<TElement>) query);
    }

    /// <summary>
    /// The public property expected in the LINQ expression tree.
    /// </summary>
    /// <value> The query. </value>
    public ObjectQuery<TElement> Query
    {
      get
      {
        return this._query;
      }
    }
  }
}
