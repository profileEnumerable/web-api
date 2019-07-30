// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.CommandTrees.DbNewInstanceExpression
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
  /// <summary>Represents the construction of a new instance of a given type, including set and record types. This class cannot be inherited. </summary>
  public sealed class DbNewInstanceExpression : DbExpression
  {
    private readonly DbExpressionList _elements;
    private readonly ReadOnlyCollection<DbRelatedEntityRef> _relatedEntityRefs;

    internal DbNewInstanceExpression(TypeUsage type, DbExpressionList args)
      : base(DbExpressionKind.NewInstance, type, true)
    {
      this._elements = args;
    }

    internal DbNewInstanceExpression(
      TypeUsage resultType,
      DbExpressionList attributeValues,
      ReadOnlyCollection<DbRelatedEntityRef> relationships)
      : this(resultType, attributeValues)
    {
      this._relatedEntityRefs = relationships.Count > 0 ? relationships : (ReadOnlyCollection<DbRelatedEntityRef>) null;
    }

    /// <summary>
    /// Gets an <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> list that provides the property/column values or set elements for the new instance.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> list that provides the property/column values or set elements for the new instance.
    /// </returns>
    public IList<DbExpression> Arguments
    {
      get
      {
        return (IList<DbExpression>) this._elements;
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

    internal bool HasRelatedEntityReferences
    {
      get
      {
        return this._relatedEntityRefs != null;
      }
    }

    internal ReadOnlyCollection<DbRelatedEntityRef> RelatedEntityReferences
    {
      get
      {
        return this._relatedEntityRefs;
      }
    }
  }
}
