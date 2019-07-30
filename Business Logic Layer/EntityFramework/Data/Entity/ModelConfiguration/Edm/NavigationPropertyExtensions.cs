// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Edm.NavigationPropertyExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.ModelConfiguration.Edm
{
  internal static class NavigationPropertyExtensions
  {
    public static object GetConfiguration(this NavigationProperty navigationProperty)
    {
      return navigationProperty.Annotations.GetConfiguration();
    }

    public static void SetConfiguration(
      this NavigationProperty navigationProperty,
      object configuration)
    {
      navigationProperty.GetMetadataProperties().SetConfiguration(configuration);
    }

    public static AssociationEndMember GetFromEnd(
      this NavigationProperty navProp)
    {
      if (navProp.Association.SourceEnd != navProp.ResultEnd)
        return navProp.Association.SourceEnd;
      return navProp.Association.TargetEnd;
    }
  }
}
