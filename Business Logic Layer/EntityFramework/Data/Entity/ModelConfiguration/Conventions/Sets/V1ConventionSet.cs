// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Conventions.Sets.V1ConventionSet
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.ModelConfiguration.Conventions.Sets
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
  internal static class V1ConventionSet
  {
    private static readonly ConventionSet _conventions = new ConventionSet(((IEnumerable<IConvention>) new IConvention[16]
    {
      (IConvention) new NotMappedTypeAttributeConvention(),
      (IConvention) new ComplexTypeAttributeConvention(),
      (IConvention) new TableAttributeConvention(),
      (IConvention) new NotMappedPropertyAttributeConvention(),
      (IConvention) new KeyAttributeConvention(),
      (IConvention) new RequiredPrimitivePropertyAttributeConvention(),
      (IConvention) new RequiredNavigationPropertyAttributeConvention(),
      (IConvention) new TimestampAttributeConvention(),
      (IConvention) new ConcurrencyCheckAttributeConvention(),
      (IConvention) new DatabaseGeneratedAttributeConvention(),
      (IConvention) new MaxLengthAttributeConvention(),
      (IConvention) new StringLengthAttributeConvention(),
      (IConvention) new ColumnAttributeConvention(),
      (IConvention) new IndexAttributeConvention(),
      (IConvention) new InversePropertyAttributeConvention(),
      (IConvention) new ForeignKeyPrimitivePropertyAttributeConvention()
    }).Reverse<IConvention>(), (IEnumerable<IConvention>) new IConvention[16]
    {
      (IConvention) new IdKeyDiscoveryConvention(),
      (IConvention) new AssociationInverseDiscoveryConvention(),
      (IConvention) new ForeignKeyNavigationPropertyAttributeConvention(),
      (IConvention) new OneToOneConstraintIntroductionConvention(),
      (IConvention) new NavigationPropertyNameForeignKeyDiscoveryConvention(),
      (IConvention) new PrimaryKeyNameForeignKeyDiscoveryConvention(),
      (IConvention) new TypeNameForeignKeyDiscoveryConvention(),
      (IConvention) new ForeignKeyAssociationMultiplicityConvention(),
      (IConvention) new OneToManyCascadeDeleteConvention(),
      (IConvention) new ComplexTypeDiscoveryConvention(),
      (IConvention) new StoreGeneratedIdentityKeyConvention(),
      (IConvention) new PluralizingEntitySetNameConvention(),
      (IConvention) new DeclaredPropertyOrderingConvention(),
      (IConvention) new SqlCePropertyMaxLengthConvention(),
      (IConvention) new PropertyMaxLengthConvention(),
      (IConvention) new DecimalPropertyConvention()
    }, (IEnumerable<IConvention>) new IConvention[2]
    {
      (IConvention) new ManyToManyCascadeDeleteConvention(),
      (IConvention) new MappingInheritedPropertiesSupportConvention()
    }, (IEnumerable<IConvention>) new IConvention[3]
    {
      (IConvention) new PluralizingTableNameConvention(),
      (IConvention) new ColumnOrderingConvention(),
      (IConvention) new ForeignKeyIndexConvention()
    });

    public static ConventionSet Conventions
    {
      get
      {
        return V1ConventionSet._conventions;
      }
    }
  }
}
