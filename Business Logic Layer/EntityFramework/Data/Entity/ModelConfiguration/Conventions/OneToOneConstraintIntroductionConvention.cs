// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.OneToOneConstraintIntroductionConvention
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Edm;
using System.Data.Entity.Utilities;
using System.Linq;

namespace System.Data.Entity.ModelConfiguration.Conventions
{
  /// <summary>
  /// Convention to configure the primary key(s) of the dependent entity type as foreign key(s) in a one:one relationship.
  /// </summary>
  public class OneToOneConstraintIntroductionConvention : IConceptualModelConvention<AssociationType>, IConvention
  {
    /// <inheritdoc />
    public virtual void Apply(AssociationType item, DbModel model)
    {
      Check.NotNull<AssociationType>(item, nameof (item));
      Check.NotNull<DbModel>(model, nameof (model));
      if (!item.IsOneToOne() || item.IsSelfReferencing() || (item.IsIndependent() || item.Constraint != null))
        return;
      IEnumerable<EdmProperty> source1 = item.SourceEnd.GetEntityType().KeyProperties();
      IEnumerable<EdmProperty> source2 = item.TargetEnd.GetEntityType().KeyProperties();
      AssociationEndMember principalEnd;
      AssociationEndMember dependentEnd;
      if (source1.Count<EdmProperty>() != source2.Count<EdmProperty>() || (!source1.Select<EdmProperty, PrimitiveType>((Func<EdmProperty, PrimitiveType>) (p => p.UnderlyingPrimitiveType)).SequenceEqual<PrimitiveType>(source2.Select<EdmProperty, PrimitiveType>((Func<EdmProperty, PrimitiveType>) (p => p.UnderlyingPrimitiveType))) || !item.TryGuessPrincipalAndDependentEnds(out principalEnd, out dependentEnd) && !item.IsPrincipalConfigured()))
        return;
      AssociationEndMember associationEnd = dependentEnd ?? item.TargetEnd;
      AssociationEndMember otherEnd = item.GetOtherEnd(associationEnd);
      ReferentialConstraint referentialConstraint = new ReferentialConstraint((RelationshipEndMember) otherEnd, (RelationshipEndMember) associationEnd, (IEnumerable<EdmProperty>) otherEnd.GetEntityType().KeyProperties().ToList<EdmProperty>(), (IEnumerable<EdmProperty>) associationEnd.GetEntityType().KeyProperties().ToList<EdmProperty>());
      item.Constraint = referentialConstraint;
    }
  }
}
