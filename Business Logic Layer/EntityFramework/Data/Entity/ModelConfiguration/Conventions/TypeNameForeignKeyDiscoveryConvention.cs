// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.TypeNameForeignKeyDiscoveryConvention
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.ModelConfiguration.Conventions
{
  /// <summary>
  /// Convention to discover foreign key properties whose names are a combination
  /// of the principal type name and the principal type primary key property name(s).
  /// </summary>
  public class TypeNameForeignKeyDiscoveryConvention : ForeignKeyDiscoveryConvention
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
      return string.Equals(dependentProperty.Name, principalEntityType.Name + principalKeyProperty.Name, StringComparison.OrdinalIgnoreCase);
    }
  }
}
