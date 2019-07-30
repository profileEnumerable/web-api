// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.CommandTrees.DbGroupByExpression
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.CommandTrees.Internal;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Core.Common.CommandTrees
{
  /// <summary>Represents a group by operation. A group by operation is a grouping of the elements in the input set based on the specified key expressions followed by the application of the specified aggregates. This class cannot be inherited. </summary>
  public sealed class DbGroupByExpression : DbExpression
  {
    private readonly DbGroupExpressionBinding _input;
    private readonly DbExpressionList _keys;
    private readonly ReadOnlyCollection<DbAggregate> _aggregates;

    internal DbGroupByExpression(
      TypeUsage collectionOfRowResultType,
      DbGroupExpressionBinding input,
      DbExpressionList groupKeys,
      ReadOnlyCollection<DbAggregate> aggregates)
      : base(DbExpressionKind.GroupBy, collectionOfRowResultType, true)
    {
      this._input = input;
      this._keys = groupKeys;
      this._aggregates = aggregates;
    }

    /// <summary>
    /// Gets the <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbGroupExpressionBinding" /> that specifies the input set and provides access to the set element and group element variables.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbGroupExpressionBinding" /> that specifies the input set and provides access to the set element and group element variables.
    /// </returns>
    public DbGroupExpressionBinding Input
    {
      get
      {
        return this._input;
      }
    }

    /// <summary>
    /// Gets a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> list that provides grouping keys.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> list that provides grouping keys.
    /// </returns>
    public IList<DbExpression> Keys
    {
      get
      {
        return (IList<DbExpression>) this._keys;
      }
    }

    /// <summary>
    /// Gets a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbAggregate" /> list that provides the aggregates to apply.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbAggregate" /> list that provides the aggregates to apply.
    /// </returns>
    public IList<DbAggregate> Aggregates
    {
      get
      {
        return (IList<DbAggregate>) this._aggregates;
      }
    }

    /// <summary>Implements the visitor pattern for expressions that do not produce a result value.</summary>
    /// <param name="visitor">
    /// An instance of <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionVisitor" />.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException"> visitor  is null.</exception>
    public override void Accept(DbExpressionVisitor visitor)
    {
      Check.NotNull<DbExpressionVisitor>(visitor, nameof (visitor));
      visitor.Visit(this);
    }

    /// <summary>Implements the visitor pattern for expressions that produce a result value of a specific type.</summary>
    /// <returns>
    /// A result value of a specific type produced by
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionVisitor" />
    /// .
    /// </returns>
    /// <param name="visitor">
    /// An instance of a typed <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionVisitor" /> that produces a result value of a specific type.
    /// </param>
    /// <typeparam name="TResultType">The type of the result produced by  visitor .</typeparam>
    /// <exception cref="T:System.ArgumentNullException"> visitor  is null.</exception>
    public override TResultType Accept<TResultType>(DbExpressionVisitor<TResultType> visitor)
    {
      Check.NotNull<DbExpressionVisitor<TResultType>>(visitor, nameof (visitor));
      return visitor.Visit(this);
    }
  }
}
