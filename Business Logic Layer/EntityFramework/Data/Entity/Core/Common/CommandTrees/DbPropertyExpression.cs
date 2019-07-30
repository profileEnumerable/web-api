// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.CommandTrees.DbPropertyExpression
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Common.CommandTrees
{
  /// <summary>Provides methods and properties for retrieving an instance property. This class cannot be inherited.</summary>
  public class DbPropertyExpression : DbExpression
  {
    private readonly EdmMember _property;
    private readonly DbExpression _instance;

    internal DbPropertyExpression()
    {
    }

    internal DbPropertyExpression(TypeUsage resultType, EdmMember property, DbExpression instance)
      : base(DbExpressionKind.Property, resultType, true)
    {
      this._property = property;
      this._instance = instance;
    }

    /// <summary>Gets the property metadata for the property to retrieve.</summary>
    /// <returns>The property metadata for the property to retrieve.</returns>
    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Property")]
    public virtual EdmMember Property
    {
      get
      {
        return this._property;
      }
    }

    /// <summary>
    /// Gets a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that defines the instance from which the property should be retrieved.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that defines the instance from which the property should be retrieved.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">The expression is null.</exception>
    /// <exception cref="T:System.ArgumentException">
    /// The expression is not associated with the command tree of the
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbPropertyExpression" />
    /// , or its result type is not equal or promotable to the type that defines the property.
    /// </exception>
    public virtual DbExpression Instance
    {
      get
      {
        return this._instance;
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

    /// <summary>Creates a new key/value pair based on this property expression.</summary>
    /// <returns>
    /// A new key/value pair with the key and value derived from the
    /// <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbPropertyExpression" />
    /// .
    /// </returns>
    public KeyValuePair<string, DbExpression> ToKeyValuePair()
    {
      return new KeyValuePair<string, DbExpression>(this.Property.Name, (DbExpression) this);
    }

    /// <summary>
    /// Enables implicit casting to <see cref="T:System.Collections.Generic.KeyValuePair`2" />.
    /// </summary>
    /// <param name="value">The expression to be converted.</param>
    /// <returns>The converted value.</returns>
    public static implicit operator KeyValuePair<string, DbExpression>(
      DbPropertyExpression value)
    {
      Check.NotNull<DbPropertyExpression>(value, nameof (value));
      return value.ToKeyValuePair();
    }
  }
}
