// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Edm.MetadataPropertyExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.ModelConfiguration.Edm
{
  internal static class MetadataPropertyExtensions
  {
    private const string ClrPropertyInfoAnnotation = "ClrPropertyInfo";
    private const string ClrAttributesAnnotation = "ClrAttributes";
    private const string ConfiguationAnnotation = "Configuration";

    public static IList<Attribute> GetClrAttributes(
      this IEnumerable<MetadataProperty> metadataProperties)
    {
      return (IList<Attribute>) metadataProperties.GetAnnotation("ClrAttributes");
    }

    public static void SetClrAttributes(
      this ICollection<MetadataProperty> metadataProperties,
      IList<Attribute> attributes)
    {
      metadataProperties.SetAnnotation("ClrAttributes", (object) attributes);
    }

    public static PropertyInfo GetClrPropertyInfo(
      this IEnumerable<MetadataProperty> metadataProperties)
    {
      return (PropertyInfo) metadataProperties.GetAnnotation("ClrPropertyInfo");
    }

    public static void SetClrPropertyInfo(
      this ICollection<MetadataProperty> metadataProperties,
      PropertyInfo propertyInfo)
    {
      metadataProperties.SetAnnotation("ClrPropertyInfo", (object) propertyInfo);
    }

    public static Type GetClrType(
      this IEnumerable<MetadataProperty> metadataProperties)
    {
      return (Type) metadataProperties.GetAnnotation("http://schemas.microsoft.com/ado/2013/11/edm/customannotation:ClrType");
    }

    public static void SetClrType(
      this ICollection<MetadataProperty> metadataProperties,
      Type type)
    {
      metadataProperties.SetAnnotation("http://schemas.microsoft.com/ado/2013/11/edm/customannotation:ClrType", (object) type);
    }

    public static object GetConfiguration(
      this IEnumerable<MetadataProperty> metadataProperties)
    {
      return metadataProperties.GetAnnotation("Configuration");
    }

    public static void SetConfiguration(
      this ICollection<MetadataProperty> metadataProperties,
      object configuration)
    {
      metadataProperties.SetAnnotation("Configuration", configuration);
    }

    public static object GetAnnotation(
      this IEnumerable<MetadataProperty> metadataProperties,
      string name)
    {
      foreach (MetadataProperty metadataProperty in metadataProperties)
      {
        if (metadataProperty.Name.Equals(name, StringComparison.Ordinal))
          return metadataProperty.Value;
      }
      return (object) null;
    }

    public static void SetAnnotation(
      this ICollection<MetadataProperty> metadataProperties,
      string name,
      object value)
    {
      MetadataProperty metadataProperty = metadataProperties.SingleOrDefault<MetadataProperty>((Func<MetadataProperty, bool>) (p => p.Name.Equals(name, StringComparison.Ordinal)));
      if (metadataProperty == null)
      {
        MetadataProperty annotation = MetadataProperty.CreateAnnotation(name, value);
        metadataProperties.Add(annotation);
      }
      else
        metadataProperty.Value = value;
    }

    public static void RemoveAnnotation(
      this ICollection<MetadataProperty> metadataProperties,
      string name)
    {
      MetadataProperty metadataProperty = metadataProperties.SingleOrDefault<MetadataProperty>((Func<MetadataProperty, bool>) (p => p.Name.Equals(name, StringComparison.Ordinal)));
      if (metadataProperty == null)
        return;
      metadataProperties.Remove(metadataProperty);
    }

    public static void Copy(
      this ICollection<MetadataProperty> sourceAnnotations,
      ICollection<MetadataProperty> targetAnnotations)
    {
      foreach (MetadataProperty sourceAnnotation in (IEnumerable<MetadataProperty>) sourceAnnotations)
        targetAnnotations.SetAnnotation(sourceAnnotation.Name, sourceAnnotation.Value);
    }
  }
}
