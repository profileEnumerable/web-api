// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Edm.StorageAssociationSetMappingExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Mapping;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.ModelConfiguration.Edm
{
  internal static class StorageAssociationSetMappingExtensions
  {
    public static AssociationSetMapping Initialize(
      this AssociationSetMapping associationSetMapping)
    {
      associationSetMapping.SourceEndMapping = new EndPropertyMapping();
      associationSetMapping.TargetEndMapping = new EndPropertyMapping();
      return associationSetMapping;
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    public static object GetConfiguration(this AssociationSetMapping associationSetMapping)
    {
      return associationSetMapping.Annotations.GetConfiguration();
    }

    public static void SetConfiguration(
      this AssociationSetMapping associationSetMapping,
      object configuration)
    {
      associationSetMapping.Annotations.SetConfiguration(configuration);
    }
  }
}
