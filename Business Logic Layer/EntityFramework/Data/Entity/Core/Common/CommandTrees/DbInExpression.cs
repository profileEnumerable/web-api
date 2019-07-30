// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.CommandTrees.DbInExpression
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.Internal;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Core.Common.CommandTrees
{
  /// <summary>
  /// Represents a boolean expression that tests whether a specified item matches any element in a list.
  /// </summary>
  public class DbInExpression : DbExpression
  {
    private readonly DbExpression _item;
    private readonly DbExpressionList _list;

    internal DbInExpression(TypeUsage booleanResultType, DbExpression item, DbExpressionList list)
      : base(DbExpressionKind.In, booleanResultType, true)
    {
      this._item = item;
      this._list = list;
    }

    /// <summary>
    /// Gets a DbExpression that specifies the item to be matched.
    /// </summary>
    public DbExpression Item
    {
      get
      {
        return this._item;
      }
    }

    /// <summary>Gets the list of DbExpression to test for a match.</summary>
    public IList<DbExpression> List
    {
      get
      {
        return (IList<DbExpression>) this._list;
      }
    }

    /// <summary>
    /// The visitor pattern method for expression visitors that do not produce a result value.
    /// </summary>
    /// <param name="visitor"> An instance of DbExpressionVisitor. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="visitor" />
    /// is null
    /// </exception>
    public override void Accept(DbExpressionVisitor visitor)
    {
      Check.NotNull<DbExpressionVisitor>(visitor, nameof (visitor));
      visitor.Visit(this);
    }

    /// <summary>
    /// The visitor pattern method for expression visitors that produce a result value of a specific type.
    /// </summary>
    /// <param name="visitor"> An instance of a typed DbExpressionVisitor that produces a result value of type TResultType. </param>
    /// <typeparam name="TResultType">
    /// The type of the result produced by <paramref name="visitor" />
    /// </typeparam>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="visitor" />
    /// is null
    /// </exception>
    /// <returns>
    /// An instance of <typeparamref name="TResultType" /> .
    /// </returns>
    public override TResultType Accept<TResultType>(DbExpressionVisitor<TResultType> visitor)
    {
      Check.NotNull<DbExpressionVisitor<TResultType>>(visitor, nameof (visitor));
      return visitor.Visit(this);
    }
  }
}
