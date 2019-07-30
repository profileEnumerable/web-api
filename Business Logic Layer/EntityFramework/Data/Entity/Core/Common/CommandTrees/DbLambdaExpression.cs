// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.CommandTrees.DbLambdaExpression
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
  /// Allows the application of a lambda function to arguments represented by
  /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" />
  /// objects.
  /// </summary>
  public sealed class DbLambdaExpression : DbExpression
  {
    private readonly DbLambda _lambda;
    private readonly DbExpressionList _arguments;

    internal DbLambdaExpression(TypeUsage resultType, DbLambda lambda, DbExpressionList args)
      : base(DbExpressionKind.Lambda, resultType, true)
    {
      this._lambda = lambda;
      this._arguments = args;
    }

    /// <summary>
    /// Gets the <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbLambda" /> representing the Lambda function applied by this expression.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbLambda" /> representing the Lambda function applied by this expression.
    /// </returns>
    public DbLambda Lambda
    {
      get
      {
        return this._lambda;
      }
    }

    /// <summary>
    /// Gets a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> list that provides the arguments to which the Lambda function should be applied.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> list.
    /// </returns>
    public IList<DbExpression> Arguments
    {
      get
      {
        return (IList<DbExpression>) this._arguments;
      }
    }

    /// <summary>The visitor pattern method for expression visitors that do not produce a result value.</summary>
    /// <param name="visitor">
    /// An instance of <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionVisitor" />.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException"> visitor  is null</exception>
    public override void Accept(DbExpressionVisitor visitor)
    {
      Check.NotNull<DbExpressionVisitor>(visitor, nameof (visitor));
      visitor.Visit(this);
    }

    /// <summary>The visitor pattern method for expression visitors that produce a result value of a specific type.</summary>
    /// <returns>The type of the result produced by the expression visitor.</returns>
    /// <param name="visitor">
    /// An instance of a typed <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionVisitor" /> that produces a result value of type TResultType.
    /// </param>
    /// <typeparam name="TResultType">The type of the result produced by  visitor </typeparam>
    /// <exception cref="T:System.ArgumentNullException"> visitor  is null</exception>
    public override TResultType Accept<TResultType>(DbExpressionVisitor<TResultType> visitor)
    {
      Check.NotNull<DbExpressionVisitor<TResultType>>(visitor, nameof (visitor));
      return visitor.Visit(this);
    }
  }
}
