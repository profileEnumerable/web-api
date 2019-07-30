// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.ComplexTypeDiscoveryConvention
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Configuration.Types;
using System.Data.Entity.ModelConfiguration.Edm;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.ModelConfiguration.Conventions
{
  /// <summary>
  /// Convention to configure a type as a complex type if it has no primary key, no mapped base type and no navigation properties.
  /// </summary>
  public class ComplexTypeDiscoveryConvention : IConceptualModelConvention<EdmModel>, IConvention
  {
    /// <inheritdoc />
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    public virtual void Apply(EdmModel item, DbModel model)
    {
      Check.NotNull<EdmModel>(item, nameof (item));
      Check.NotNull<DbModel>(model, nameof (model));
      foreach (var data in item.EntityTypes.Where<EntityType>((Func<EntityType, bool>) (entityType =>
      {
        if (entityType.KeyProperties.Count == 0)
          return entityType.BaseType == null;
        return false;
      })).Select(entityType => new
      {
        entityType = entityType,
        entityTypeConfiguration = entityType.GetConfiguration() as EntityTypeConfiguration
      }).Where(_param0 =>
      {
        if (_param0.entityTypeConfiguration == null || !_param0.entityTypeConfiguration.IsExplicitEntity && _param0.entityTypeConfiguration.IsStructuralConfigurationOnly)
          return !_param0.entityType.Members.Where<EdmMember>(new Func<EdmMember, bool>(Helper.IsNavigationProperty)).Any<EdmMember>();
        return false;
      }).Select(_param1 => new
      {
        \u003C\u003Eh__TransparentIdentifier0 = _param1,
        matchingAssociations = item.AssociationTypes.Where<AssociationType>((Func<AssociationType, bool>) (associationType =>
        {
          if (associationType.SourceEnd.GetEntityType() != _param1.entityType)
            return associationType.TargetEnd.GetEntityType() == _param1.entityType;
          return true;
        })).Select(associationType => new
        {
          associationType = associationType,
          declaringEnd = associationType.SourceEnd.GetEntityType() == _param1.entityType ? associationType.SourceEnd : associationType.TargetEnd
        }).Select(_param0 => new
        {
          \u003C\u003Eh__TransparentIdentifier2 = _param0,
          declaringEntity = _param0.associationType.GetOtherEnd(_param0.declaringEnd).GetEntityType()
        }).Select(_param1 => new
        {
          \u003C\u003Eh__TransparentIdentifier3 = _param1,
          navigationProperties = _param1.declaringEntity.Members.Where<EdmMember>(new Func<EdmMember, bool>(Helper.IsNavigationProperty)).Cast<NavigationProperty>().Where<NavigationProperty>((Func<NavigationProperty, bool>) (n => n.ResultEnd.GetEntityType() == _param1.entityType))
        }).Select(_param0 => new
        {
          DeclaringEnd = _param0.\u003C\u003Eh__TransparentIdentifier3.\u003C\u003Eh__TransparentIdentifier2.declaringEnd,
          AssociationType = _param0.\u003C\u003Eh__TransparentIdentifier3.\u003C\u003Eh__TransparentIdentifier2.associationType,
          DeclaringEntityType = _param0.\u003C\u003Eh__TransparentIdentifier3.declaringEntity,
          NavigationProperties = _param0.navigationProperties.ToList<NavigationProperty>()
        })
      }).Where(_param0 => _param0.matchingAssociations.All(a =>
      {
        if (a.AssociationType.Constraint == null && a.AssociationType.GetConfiguration() == null && (!a.AssociationType.IsSelfReferencing() && a.DeclaringEnd.IsOptional()))
          return a.NavigationProperties.All<NavigationProperty>((Func<NavigationProperty, bool>) (n => n.GetConfiguration() == null));
        return false;
      })).Select(_param0 => new
      {
        EntityType = _param0.\u003C\u003Eh__TransparentIdentifier0.entityType,
        MatchingAssociations = _param0.matchingAssociations.ToList()
      }).ToList())
      {
        ComplexType complexType = item.AddComplexType(data.EntityType.Name, data.EntityType.NamespaceName);
        foreach (EdmProperty declaredProperty in data.EntityType.DeclaredProperties)
          complexType.AddMember((EdmMember) declaredProperty);
        foreach (MetadataProperty annotation in data.EntityType.Annotations)
          complexType.GetMetadataProperties().Add(annotation);
        foreach (var matchingAssociation in data.MatchingAssociations)
        {
          foreach (NavigationProperty navigationProperty in matchingAssociation.NavigationProperties)
          {
            if (matchingAssociation.DeclaringEntityType.Members.Where<EdmMember>(new Func<EdmMember, bool>(Helper.IsNavigationProperty)).Contains<EdmMember>((EdmMember) navigationProperty))
            {
              matchingAssociation.DeclaringEntityType.RemoveMember((EdmMember) navigationProperty);
              EdmProperty edmProperty = matchingAssociation.DeclaringEntityType.AddComplexProperty(navigationProperty.Name, complexType);
              foreach (MetadataProperty annotation in navigationProperty.Annotations)
                edmProperty.GetMetadataProperties().Add(annotation);
            }
          }
          item.RemoveAssociationType(matchingAssociation.AssociationType);
        }
        item.RemoveEntityType(data.EntityType);
      }
    }
  }
}
