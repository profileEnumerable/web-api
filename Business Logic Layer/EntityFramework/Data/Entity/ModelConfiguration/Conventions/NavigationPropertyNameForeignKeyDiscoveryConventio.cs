// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.NavigationPropertyNameForeignKeyDiscoveryConvention
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.ModelConfiguration.Edm;
using System.Data.Entity.Utilities;
using System.Linq;

namespace System.Data.Entity.ModelConfiguration.Conventions
{
  /// <summary>
  /// Convention to discover foreign key properties whose names are a combination
  /// of the dependent navigation property name and the principal type primary key property name(s).
  /// </summary>
  public class NavigationPropertyNameForeignKeyDiscoveryConvention : ForeignKeyDiscoveryConvention
  {
    /// <inheritdoc />
    protected override bool MatchDependentKeyProperty(
      AssociationType associationType,
      AssociationEndMember dependentAssociationEnd,
      EdmProperty dependentProperty,
      EntityType principalEntityType,
      EdmProperty principalKeyProperty)
    {
      Check.NotNull<AssociationType>(associationType, nameof (associationType));
      Check.NotNull<AssociationEndMember>(dependentAssociationEnd, nameof (dependentAssociationEnd));
      Check.NotNull<EdmProperty>(dependentProperty, nameof (dependentProperty));
      Check.NotNull<EntityType>(principalEntityType, nameof (principalEntityType));
      Check.NotNull<EdmProperty>(principalKeyProperty, nameof (principalKeyProperty));
      AssociationEndMember otherEnd = associationType.GetOtherEnd(dependentAssociationEnd);
      NavigationProperty navigationProperty = dependentAssociationEnd.GetEntityType().NavigationProperties.SingleOrDefault<NavigationProperty>((Func<NavigationProperty, bool>) (n => n.ResultEnd == otherEnd));
      if (navigationProperty == null)
        return false;
      return string.Equals(dependentProperty.Name, navigationProperty.Name + principalKeyProperty.Name, StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    protected override bool SupportsMultipleAssociations
    {
      get
      {
        return true;
      }
    }
  }
}
