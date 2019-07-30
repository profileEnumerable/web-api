// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.ForeignKeyNavigationPropertyAttributeConvention
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Edm;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Linq;

namespace System.Data.Entity.ModelConfiguration.Conventions
{
  /// <summary>
  /// Convention to process instances of <see cref="T:System.ComponentModel.DataAnnotations.Schema.ForeignKeyAttribute" /> found on navigation properties in the model.
  /// </summary>
  public class ForeignKeyNavigationPropertyAttributeConvention : IConceptualModelConvention<NavigationProperty>, IConvention
  {
    /// <inheritdoc />
    public virtual void Apply(NavigationProperty item, DbModel model)
    {
      Check.NotNull<NavigationProperty>(item, nameof (item));
      Check.NotNull<DbModel>(model, nameof (model));
      AssociationType association = item.Association;
      if (association.Constraint != null)
        return;
      ForeignKeyAttribute foreignKeyAttribute = item.GetClrAttributes<ForeignKeyAttribute>().SingleOrDefault<ForeignKeyAttribute>();
      AssociationEndMember principalEnd;
      AssociationEndMember dependentEnd;
      if (foreignKeyAttribute == null || !association.TryGuessPrincipalAndDependentEnds(out principalEnd, out dependentEnd) && !association.IsPrincipalConfigured())
        return;
      AssociationEndMember associationEndMember = dependentEnd ?? association.TargetEnd;
      principalEnd = principalEnd ?? association.SourceEnd;
      IEnumerable<string> dependentPropertyNames = ((IEnumerable<string>) foreignKeyAttribute.Name.Split(',')).Select<string, string>((Func<string, string>) (p => p.Trim()));
      EntityType declaringEntityType = model.ConceptualModel.EntityTypes.Single<EntityType>((Func<EntityType, bool>) (e => e.DeclaredNavigationProperties.Contains(item)));
      List<EdmProperty> list = ForeignKeyNavigationPropertyAttributeConvention.GetDependentProperties(associationEndMember.GetEntityType(), dependentPropertyNames, declaringEntityType, item).ToList<EdmProperty>();
      ReferentialConstraint constraint = new ReferentialConstraint((RelationshipEndMember) principalEnd, (RelationshipEndMember) associationEndMember, (IEnumerable<EdmProperty>) principalEnd.GetEntityType().KeyProperties().ToList<EdmProperty>(), (IEnumerable<EdmProperty>) list);
      IEnumerable<EdmProperty> source = associationEndMember.GetEntityType().KeyProperties();
      if (source.Count<EdmProperty>() == constraint.ToProperties.Count<EdmProperty>() && source.All<EdmProperty>((Func<EdmProperty, bool>) (kp => constraint.ToProperties.Contains(kp))))
      {
        principalEnd.RelationshipMultiplicity = RelationshipMultiplicity.One;
        if (associationEndMember.RelationshipMultiplicity.IsMany())
          associationEndMember.RelationshipMultiplicity = RelationshipMultiplicity.ZeroOrOne;
      }
      if (principalEnd.IsRequired())
        constraint.ToProperties.Each<EdmProperty, bool>((Func<EdmProperty, bool>) (p => p.Nullable = false));
      association.Constraint = constraint;
    }

    private static IEnumerable<EdmProperty> GetDependentProperties(
      EntityType dependentType,
      IEnumerable<string> dependentPropertyNames,
      EntityType declaringEntityType,
      NavigationProperty navigationProperty)
    {
      foreach (string dependentPropertyName1 in dependentPropertyNames)
      {
        string dependentPropertyName = dependentPropertyName1;
        if (string.IsNullOrWhiteSpace(dependentPropertyName))
          throw Error.ForeignKeyAttributeConvention_EmptyKey((object) navigationProperty.Name, (object) EntityTypeExtensions.GetClrType(declaringEntityType));
        EdmProperty dependentProperty = dependentType.Properties.SingleOrDefault<EdmProperty>((Func<EdmProperty, bool>) (p => p.Name.Equals(dependentPropertyName, StringComparison.Ordinal)));
        if (dependentProperty == null)
          throw Error.ForeignKeyAttributeConvention_InvalidKey((object) navigationProperty.Name, (object) EntityTypeExtensions.GetClrType(declaringEntityType), (object) dependentPropertyName, (object) EntityTypeExtensions.GetClrType(dependentType));
        yield return dependentProperty;
      }
    }
  }
}
