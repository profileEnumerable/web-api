// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.CommandTrees.DbCaseExpression
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
  /// Represents the When, Then, and Else clauses of the
  /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbCaseExpression" />
  /// . This class cannot be inherited.
  /// </summary>
  public sealed class DbCaseExpression : DbExpression
  {
    private readonly DbExpressionList _when;
    private readonly DbExpressionList _then;
    private readonly DbExpression _else;

    internal DbCaseExpression(
      TypeUsage commonResultType,
      DbExpressionList whens,
      DbExpressionList thens,
      DbExpression elseExpr)
      : base(DbExpressionKind.Case, commonResultType, true)
    {
      this._when = whens;
      this._then = thens;
      this._else = elseExpr;
    }

    /// <summary>
    /// Gets the When clauses of this <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbCaseExpression" />.
    /// </summary>
    /// <returns>
    /// The When clauses of this <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbCaseExpression" />.
    /// </returns>
    public IList<DbExpression> When
    {
      get
      {
        return (IList<DbExpression>) this._when;
      }
    }

    /// <summary>
    /// Gets the Then clauses of this <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbCaseExpression" />.
    /// </summary>
    /// <returns>
    /// The Then clauses of this <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbCaseExpression" />.
    /// </returns>
    public IList<DbExpression> Then
    {
      get
      {
        return (IList<DbExpression>) this._then;
      }
    }

    /// <summary>
    /// Gets the Else clause of this <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbCaseExpression" />.
    /// </summary>
    /// <returns>
    /// The Else clause of this <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbCaseExpression" />.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">The expression is null.</exception>
    /// <exception cref="T:System.ArgumentException">
    /// The expression is not associated with the command tree of the
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbCaseExpression" />
    /// ,or its result type is not equal or promotable to the result type of the
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbCaseExpression" />
    /// .
    /// </exception>
    public DbExpression Else
    {
      get
      {
        return this._else;
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
    /// <typeparam name="TResultType">The type of the result produced by  visitor. </typeparam>
    /// <exception cref="T:System.ArgumentNullException"> visitor  is null.</exception>
    public override TResultType Accept<TResultType>(DbExpressionVisitor<TResultType> visitor)
    {
      Check.NotNull<DbExpressionVisitor<TResultType>>(visitor, nameof (visitor));
      return visitor.Visit(this);
    }
  }
}
