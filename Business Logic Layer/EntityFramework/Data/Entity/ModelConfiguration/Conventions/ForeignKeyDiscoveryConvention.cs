// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.ForeignKeyDiscoveryConvention
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Edm;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.ModelConfiguration.Conventions
{
  /// <summary>
  /// Base class for conventions that discover foreign key properties.
  /// </summary>
  public abstract class ForeignKeyDiscoveryConvention : IConceptualModelConvention<AssociationType>, IConvention
  {
    /// <summary>
    /// Returns <c>true</c> if the convention supports pairs of entity types that have multiple associations defined between them.
    /// </summary>
    protected virtual bool SupportsMultipleAssociations
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// When overriden returns <c>true</c> if <paramref name="dependentProperty" /> should be part of the foreign key.
    /// </summary>
    /// <param name="associationType"> The association type being configured. </param>
    /// <param name="dependentAssociationEnd"> The dependent end. </param>
    /// <param name="dependentProperty"> The candidate property on the dependent end. </param>
    /// <param name="principalEntityType"> The principal end entity type. </param>
    /// <param name="principalKeyProperty"> A key property on the principal end that is a candidate target for the foreign key. </param>
    /// <returns>true if dependentProperty should be a part of the foreign key; otherwise, false.</returns>
    protected abstract bool MatchDependentKeyProperty(
      AssociationType associationType,
      AssociationEndMember dependentAssociationEnd,
      EdmProperty dependentProperty,
      EntityType principalEntityType,
      EdmProperty principalKeyProperty);

    /// <inheritdoc />
    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    public virtual void Apply(AssociationType item, DbModel model)
    {
      Check.NotNull<AssociationType>(item, nameof (item));
      Check.NotNull<DbModel>(model, nameof (model));
      AssociationEndMember principalEnd;
      AssociationEndMember dependentEnd;
      if (item.Constraint != null || item.IsIndependent() || item.IsOneToOne() && item.IsSelfReferencing() || !item.TryGuessPrincipalAndDependentEnds(out principalEnd, out dependentEnd))
        return;
      IEnumerable<EdmProperty> source1 = principalEnd.GetEntityType().KeyProperties();
      if (!source1.Any<EdmProperty>() || !this.SupportsMultipleAssociations && model.ConceptualModel.GetAssociationTypesBetween(principalEnd.GetEntityType(), dependentEnd.GetEntityType()).Count<AssociationType>() > 1)
        return;
      IEnumerable<EdmProperty> source2 = source1.SelectMany((Func<EdmProperty, IEnumerable<EdmProperty>>) (p => (IEnumerable<EdmProperty>) dependentEnd.GetEntityType().DeclaredProperties), (p, d) => new
      {
        p = p,
        d = d
      }).Where(_param1 =>
      {
        if (this.MatchDependentKeyProperty(item, dependentEnd, _param1.d, principalEnd.GetEntityType(), _param1.p))
          return _param1.p.UnderlyingPrimitiveType == _param1.d.UnderlyingPrimitiveType;
        return false;
      }).Select(_param0 => _param0.d);
      if (!source2.Any<EdmProperty>() || source2.Count<EdmProperty>() != source1.Count<EdmProperty>())
        return;
      IEnumerable<EdmProperty> source3 = dependentEnd.GetEntityType().KeyProperties();
      bool flag = source3.Count<EdmProperty>() == source2.Count<EdmProperty>() && source3.All<EdmProperty>(new Func<EdmProperty, bool>(((Enumerable) source2).Contains<EdmProperty>));
      if ((dependentEnd.IsMany() || item.IsSelfReferencing()) && flag || !dependentEnd.IsMany() && !flag)
        return;
      ReferentialConstraint referentialConstraint = new ReferentialConstraint((RelationshipEndMember) principalEnd, (RelationshipEndMember) dependentEnd, (IEnumerable<EdmProperty>) source1.ToList<EdmProperty>(), (IEnumerable<EdmProperty>) source2.ToList<EdmProperty>());
      item.Constraint = referentialConstraint;
      if (!principalEnd.IsRequired())
        return;
      referentialConstraint.ToProperties.Each<EdmProperty, bool>((Func<EdmProperty, bool>) (p => p.Nullable = false));
    }
  }
}
