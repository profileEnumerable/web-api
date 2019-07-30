// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.CommandTrees.DbConstantExpression
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Core.Common.CommandTrees
{
  /// <summary>Represents different kinds of constants (literals). This class cannot be inherited.</summary>
  public class DbConstantExpression : DbExpression
  {
    private readonly bool _shouldCloneValue;
    private readonly object _value;

    internal DbConstantExpression()
    {
    }

    internal DbConstantExpression(TypeUsage resultType, object value)
      : base(DbExpressionKind.Constant, resultType, true)
    {
      PrimitiveType type;
      this._shouldCloneValue = TypeHelpers.TryGetEdmType<PrimitiveType>(resultType, out type) && type.PrimitiveTypeKind == PrimitiveTypeKind.Binary;
      if (this._shouldCloneValue)
        this._value = ((Array) value).Clone();
      else
        this._value = value;
    }

    internal object GetValue()
    {
      return this._value;
    }

    /// <summary>Gets the constant value.</summary>
    /// <returns>The constant value.</returns>
    public virtual object Value
    {
      get
      {
        if (this._shouldCloneValue)
          return ((Array) this._value).Clone();
        return this._value;
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
